using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mze9412.ScriptCompiler.Helpers
{
    public static class ClipCopy
    {

        public class ParseRule
        {
            public readonly Func<string, Parser, bool> Condition; // line, parser, return

            public readonly Func<string, Parser, string> Action; // line, parser, return

            public readonly string Name;

            public readonly bool ContinueWithNextRule;

            public readonly bool UseReturnValue;

            public ParseRule(string name, Func<string, Parser, bool> condition, Func<string, Parser, string> action, bool continueWithNextRule = false, bool useReturnValue = true)
            {
                Condition = condition;
                Action = action;
                Name = name;
                ContinueWithNextRule = continueWithNextRule;
                UseReturnValue = useReturnValue;
            }
        }


        public class Parser
        {
            static readonly List<ParseRule> rules;

            static Parser()
            {
                rules = new List<ParseRule>();

                rules.Add(
                    new ParseRule(
                        "EnableDebug",
                        (line, parser) => line.Contains("//#EnableDebug"),
                        (line, parser) =>
                        {
                            parser.Options["DEBUG"] = true;
                            return string.Empty;
                        },
                        true,
                        false
                    ));

                rules.Add(
                    new ParseRule(
                        "EnableRemoveLineComment",
                        (line, parser) => line.Contains("//#EnableRemoveLineComments"),
                        (line, parser) =>
                        {
                            parser.Options["EnableRemoveLineComments"] = true;
                            return string.Empty;
                        },
                        true,
                        false
                    ));

                rules.Add(
                    new ParseRule(
                        "DisableRemoveLineComment",
                        (line, parser) => line.Contains("//#DisableRemoveLineComments"),
                        (line, parser) =>
                        {
                            parser.Options["EnableRemoveLineComments"] = false;
                            return string.Empty;
                        },
                        true,
                        false
                    ));

                rules.Add(
                    new ParseRule(
                        "Include",
                        (line, parser) => line.Trim().StartsWith("//#include"),
                        (line, parser) =>
                        {
                            var incl =
                            line.Trim()
                                .Substring(line.Trim().IndexOf('(') + 1, line.Trim().IndexOf(')') - line.Trim().IndexOf('(') - 1)
                                .Split(',');


                            return new Parser(new FileInfo(parser.fileName).Directory.FullName + "\\" + incl[0], bool.Parse(incl[1])).Parse();
                        }
                    ));

                rules.Add(
                    new ParseRule(
                        "replace",
                        (line, parser) => line.Trim().Contains("//#replace"),
                        (line, parser) =>
                        {
                            var curr = line.Trim();
                            var start = curr.IndexOf("//#replace");
                            var begin = curr.IndexOf('(', start);
                            var replaceIn = curr.Substring(0, start);

                            var repWhat =
                                curr
                                    .Substring(begin + 1, line.Trim().IndexOf(')', begin) - begin - 1)
                                    .Split(new[] { ',' });

                            var result = replaceIn.Replace(repWhat[0], repWhat[1]);
                            return result;
                        }
                    ));


                rules.Add(
                    new ParseRule(
                        "RemoveLineComments",
                        (line, parser) => {
                            if (!parser.Options.ContainsKey("EnableRemoveLineComments")) return false;
                            if (!parser.Options["EnableRemoveLineComments"]) return false;
                            return line.Trim().Contains("//");
                        },
                        (line, parser) =>
                        {
                            var pos = line.IndexOf("//");
                            return line.Substring(0, pos);
                        }
                    ));

                /**
                 * 
                 * THIS SHOULD ALWAYS BE THE LAST RULE
                 * 
                 * */
                rules.Add(
                    new ParseRule(
                        "addNonEmpty",
                        (line, parser) => !string.IsNullOrWhiteSpace(line),
                        (line, parser) =>
                        {
                            return line.Trim();
                        }
                    ));

            }

            public Dictionary<string, bool> Options = new Dictionary<string, bool>();

            StreamReader readMe;

            bool makeCompact;

            int lineThreshold;

            bool first;

            string fileName;

            public Parser(string fileName, bool makeCompact, int lineThreshold = -1, bool first = false)
            {
                this.fileName = fileName;
                this.readMe = new StreamReader(fileName);
                this.makeCompact = makeCompact;
                this.lineThreshold = lineThreshold;
                this.first = first;
            }

            public string Parse()
            {
                try
                {
                    var addMe = new StringBuilder();
                    if (!first)
                        addMe.AppendLine();


                    string line;
                    var started = false;
                    var stop = false;
                    while ((line = readMe.ReadLine()) != null && !stop)
                    {

                        if (!started)
                        {
                            if (line.Contains("/**Begin copy here**"))
                            {
                                started = true;
                            }
                            continue;
                        }

                        if (line.Contains("/**End copy here**"))
                        {
                            stop = true;
                            continue;
                        }

                        foreach (var rule in rules)
                        {
                            if (rule.Condition(line, this))
                            {
                                try
                                {
                                    var result = rule.Action(line, this);

                                    if (rule.UseReturnValue)
                                    {
                                        if (result == null)
                                        {
                                            Environment.Exit(1);
                                        }

                                        addMe.Append(result);
                                        if (!makeCompact) addMe.AppendLine();
                                    }
                                    if (!rule.ContinueWithNextRule) break;

                                }
                                catch (Exception x)
                                {
                                    Environment.Exit(1);
                                }
                            }
                        }
                    }
                    //addMe.AppendLine();
                    return addMe.ToString();
                }
                catch (Exception x)
                {
                    Environment.Exit(1);
                }
                return null;
            }
        }
    }
}
