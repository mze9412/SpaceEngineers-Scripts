using System;
using System.Collections.Generic;
using mze9412.SEScripts.InventoryManager.Actions;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;

namespace mze9412.SEScripts.InventoryManager
{
    /**Begin copy here**/

    public sealed class InventoryManager
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public InventoryManager(MyGridProgram program)
        {
            GridProgram = program;
            RunCounter = 0;

            Actions = new LinkedList<InventoryManagerAction>();
            Actions.AddLast(new CollectItemsAction(GridProgram, DisplayId));
            Actions.AddLast(new CollectFromConnectedGridsAction(GridProgram, DisplayId));
            Actions.AddLast(new RefineryBalanceAction(GridProgram, DisplayId));
            Actions.AddLast(new OxyGeneratorBalanceAction(GridProgram, DisplayId));
            Actions.AddLast(new ReactorBalanceAction(GridProgram, DisplayId));
            Actions.AddLast(new AmmoBalanceAction(GridProgram, DisplayId));

            CurrentAction = Actions.First;
        }

        #region Properties

        private const string DisplayId = "D67FEC05-0720-4C9F-89FE-634B9F73F17F";
        private const string LcdName = "LCD PB3 (InvMan)";

        private MyGridProgram GridProgram { get; set; }
        
        /// <summary>
        /// Helper to count executions
        /// </summary>
        private int RunCounter { get; set; }

        /// <summary>
        /// Progress for resume
        /// </summary>
        public LinkedListNode<InventoryManagerAction> CurrentAction { get; set; }

        /// <summary>
        /// Linked list of all actions
        /// </summary>
        private LinkedList<InventoryManagerAction> Actions { get; set; } 

        #endregion

        /// <summary>
        /// Runs the manager
        /// </summary>
        /// <param name="argument"></param>
        public void Run(string argument)
        {
            GridProgram.Echo("Script is alive: " + DateTime.Now);

            //create LCD
            var lcds = new List<IMyTerminalBlock>(10);
            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, x => x.CubeGrid == GridProgram.Me.CubeGrid && x.CustomName == LcdName);
            if (lcds.Count > 0)
            {
                var ret = LCDHelper.CreateDisplay(DisplayId, (IMyTextPanel)lcds[0]);
                if (ret == DisplayId)
                {
                    LCDHelper.WriteHeader(DisplayId, "InvMan Instruction Count (" + DateTime.Now.ToShortTimeString() + ")");
                }
            }
            else
            {
                throw new Exception("Failed to find LCD?!");
            }

            //execute action and switch to next if finished
            var finished = CurrentAction.Value.Run(argument);
            var perc = (double)GridProgram.Runtime.CurrentInstructionCount / (double)GridProgram.Runtime.MaxInstructionCount;
            LCDHelper.WriteLine(DisplayId, string.Format("{0} - {1:0.00}%", CurrentAction.Value.Name, perc*100));
            if (finished)
            {
                CurrentAction = CurrentAction.Next;
            }

            //reset to first action
            if (CurrentAction == null)
            {
                //print and delete LCD
                LCDHelper.PrintDisplay(DisplayId);
                LCDHelper.DeleteDisplay(DisplayId);
                CurrentAction = Actions.First;
            }
        }
    }

    /**End copy here**/
}
