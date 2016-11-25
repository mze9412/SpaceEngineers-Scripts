using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.TickRunner
{
    public sealed class TickRunnerProgram : MyGridProgram
    {
        /**Begin copy here**/

        /// <summary>
        /// Name of the timer block
        /// </summary>
        public const string TimerName = "Timer - Tick";

        /// <summary>
        /// Counter for debugging (check if script is running)
        /// </summary>
        public int Counter;

        /// <summary>
        /// This script triggers the "TriggerNow" action from a timer block which has the name defined as TimerName.
        /// Purpose of this script is to trigger other scripts once per tick.
        /// 
        /// The timer must be set up to run a programmable block with this script in addition to all scripts which should be executed each tick.
        /// 
        /// The script only checks for timer blocks which are on the same grid as the programmable block itself!
        /// </summary>
        /// <param name="argument"></param>
        public void Main(string argument)
        {
            Echo("Counter: " + Counter++);
            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(blocks, x => x.CubeGrid == Me.CubeGrid && x.CustomName == TimerName);

            if (blocks.Count > 0)
            {
                var timer = blocks[0];
                timer.ApplyAction("TriggerNow");
            }
            else
            {
                Echo("ERROR. Block " + TimerName + " not found.");
            }
        }

        /**End copy here**/
    }
}
