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
            Actions.AddLast(new CollectItemsAction(GridProgram));
            Actions.AddLast(new CollectFromConnectedGridsAction(GridProgram));
            Actions.AddLast(new RefineryBalanceAction(GridProgram));

            CurrentAction = Actions.First;
        }

        #region Properties

        private const string DisplayId = "D67FEC05-0720-4C9F-89FE-634B9F73F17F";
        private const string LcdName = "LCD_Inventory";

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
            //create LCD
            var lcds = new List<IMyTerminalBlock>(10);
            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, x => x.CubeGrid == GridProgram.Me.CubeGrid && x.CustomName == LcdName);
            if (lcds.Count > 0)
            {
                LCDHelper.WriteLine(DisplayId, "Creating LCD");
                LCDHelper.CreateDisplay(DisplayId, (IMyTextPanel)lcds[0]);
            }
            else
            {
                throw new Exception("Failed to find LCD?!");
            }

            //execute action and switch to next if finished
            LCDHelper.WriteLine(DisplayId, "Running: " + CurrentAction.Value.Name);
            var finished = CurrentAction.Value.Run(DisplayId);
            LCDHelper.WriteLine(DisplayId, "Finished: " + finished);
            if (finished)
            {
                CurrentAction = CurrentAction.Next;
            }

            //reset to first action
            if (CurrentAction == null)
            {
                CurrentAction = Actions.First;
            }

            //print and delete LCD
            LCDHelper.PrintDisplay(DisplayId);
            LCDHelper.DeleteDisplay(DisplayId);
        }
        
        /**End copy here**/
    }
}
