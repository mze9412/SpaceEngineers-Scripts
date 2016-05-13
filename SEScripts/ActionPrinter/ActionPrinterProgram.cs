using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

namespace mze9412.SEScripts.ActionPrinter
{
    public sealed class ActionPrinterProgram : MyGridProgram
    {
        /**Begin copy here**/

        public void Main(string argument)
        {
            var block = GridTerminalSystem.GetBlockWithName(argument);

            if (block == null)
            {
                Echo("Block with name \'" + argument + "\' does not exist.");
            }
            else
            {
                Echo("Id // Name");
                var actions = new List<ITerminalAction>();
                block.GetActions(actions);
                foreach (var action in actions)
                {
                    Echo(action.Id + " // " + action.Name);
                }
            }
        }

        /**End copy here**/
    }
}
