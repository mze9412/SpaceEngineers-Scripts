using System;
using System.IO;
using System.Text;

namespace mze9412.ScriptCompiler.Helpers
{
    public static class ClipCopy
    {
        //[STAThread]
        //static void Main(string[] aargh)
        //{
        //    if (!aargh.Any())
        //    {
        //        Console.WriteLine("missing argument");
        //        return;
        //    }

        //    var result = new StringBuilder();

        //    Includefile(result, new FileInfo(aargh[0]).FullName, false, first: true);

        //    Clipboard.SetText(result.ToString());
        //}


        /// <summary>
        /// Adds the files content to the given StringBuilder 
        /// </summary>
        /// <param name="addToMe">the StringBuilder to add to</param>
        /// <param name="filename">the file to include</param>
        /// <param name="makeCompact">removes all whitespaces from the file</param>
        /// <param name="linetheshold">requires makeCompact:a newline will be added after added lines exceed this many characters; -1 for off</param>
        /// <param name="first">if starting with an empty StringBuilder this has to be true</param>
        public static void Includefile(StringBuilder addToMe, string filename, bool makeCompact, int linetheshold = -1, bool first = false)
        {
            try
            {
                var addMe = new StringBuilder();
                if (!first) addMe.Append(makeCompact ? "}" : "\n}\n");

                using (var readMe = new StreamReader(filename))
                {
                    string line;
                    var started = false;
                    var stop = false;
                    var currlinelength = 0;
                    var tresholdLength = linetheshold;
                    bool linecommentErrorShown = false;
                    while ((line = readMe.ReadLine()) != null && !stop)
                    {
                        if (!started)
                        {
                            if (line.Contains("/**Begin copy here**"))
                            {
                                started = true;
                            }
                        }
                        else
                        {
                            if (line.Contains("/**End copy here**"))
                            {
                                stop = true;
                            }
                            else if (line.Trim().StartsWith("///"))
                            {
                                continue;
                            }
                            else
                            {
                                if (line.Trim().StartsWith("//#include"))
                                {
                                    var incl =
                                        line.Trim()
                                            .Substring(line.Trim().IndexOf('(') + 1, line.Trim().IndexOf(')') - line.Trim().IndexOf('(') - 1)
                                            .Split(',');

                                    try
                                    {
                                        Includefile(addMe, new FileInfo(filename).Directory.FullName + "\\" + incl[0], bool.Parse(incl[1]));
                                    }
                                    catch (Exception x)
                                    {
                                        Console.WriteLine("Inner NOPE:\n" + incl[0] + "\n" + incl[1] + "\n" + x);
                                    }
                                }
                                else
                                {
                                    if (line.Trim().Contains("//#replace")) //#replace(what,withwhat) << has to be on the end of the line
                                    {
                                        var curr = line.Trim();
                                        var start = curr.IndexOf("//#replace");
                                        var begin = curr.IndexOf('(', start);
                                        var replaceIn = curr.Substring(0, start);

                                        var repWhat =
                                            curr
                                                .Substring(begin + 1, line.Trim().IndexOf(')', begin) - begin - 1)
                                                .Split(new[] {','});

                                        var result = replaceIn.Replace(repWhat[0], repWhat[1]);

                                        if (makeCompact)
                                        {
                                            if (result.Contains("//") && !linecommentErrorShown)
                                            {
                                                Console.WriteLine(filename + "\nThere seems to be a single line comment. This will cause errors in compact mode, please replace with a delimited-comment");
                                                linecommentErrorShown = true;
                                            }
                                            addMe.Append(result);
                                            currlinelength += result.Length;
                                            if (tresholdLength >= 0 && currlinelength > tresholdLength) //TODO same as below
                                            {
                                                addMe.AppendLine();
                                                currlinelength = 0;
                                            }
                                        }
                                        else
                                        {
                                            addMe.AppendLine(result);
                                        }

                                    }
                                    else if (!string.IsNullOrEmpty(line.Trim()))
                                    {
                                        if (makeCompact)
                                        {
                                            var nextline = line.Trim();

                                            if (nextline.Contains("//") && !linecommentErrorShown)
                                            {
                                                Console.WriteLine(filename + "\nThere seems to be a single line comment. This will cause errors in compact mode, please replace with a delimited-comment");
                                                linecommentErrorShown = true;
                                            }

                                            addMe.Append(nextline);
                                            currlinelength += nextline.Length;
                                            if (tresholdLength >= 0 && currlinelength > tresholdLength) //TODO same as above
                                            {
                                                addMe.AppendLine();
                                                currlinelength = 0;
                                            }
                                        }
                                        else
                                        {
                                            addMe.AppendLine(line);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                addToMe.AppendLine();
                addToMe.Append(addMe);
            }
            catch (Exception x)
            {
                Console.WriteLine("Outer NOPE:\n" + x);
            }

        }
    }
}
