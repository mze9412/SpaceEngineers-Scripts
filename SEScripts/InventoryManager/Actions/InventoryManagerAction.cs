using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.InventoryManager.Actions
{
    /**Begin copy here**/

    /// <summary>
    /// Base class for all inventory management actions
    /// </summary>
    public abstract class InventoryManagerAction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        protected InventoryManagerAction(MyGridProgram gridProgram, string name)
        {
            if (gridProgram == null)
            {
                throw new ArgumentNullException("gridProgram");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            GridProgram = gridProgram;
            Name = name;
        }

        #region Properties
        
        /// <summary>
        /// Name of the action
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Access to grid program
        /// </summary>
        public MyGridProgram GridProgram { get; private set; }

        #endregion

        /// <summary>
        /// Public run method
        /// </summary>
        /// <param name="argument"></param>
        public bool Run(string argument)
        {
            return RunCore(argument);
        }

        /// <summary>
        /// Internal run method
        /// </summary>
        /// <param name="argument"></param>
        protected abstract bool RunCore(string argument);

        #region Helpers
        
        /// <summary>
        /// True of instruction count is still low enough
        /// </summary>
        protected bool CanContinueRun
        {
            get
            {
                var val = (double)GridProgram.Runtime.CurrentInstructionCount/GridProgram.Runtime.MaxInstructionCount;
                return val < 0.5;
            }
        }

        #region Transfer items helper

        /// <summary>
        /// Tries to get a target block for given type
        /// </summary>
        /// <param name="sourceInventory"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IMyTerminalBlock GetTargetForItem(IMyInventory sourceInventory, int type)
        {
            //determine tag
            var tag = string.Empty;
            switch (type)
            {
                case 0:
                    tag = InventoryManagerConfig.OreContainerTag;
                    break;
                case 1:
                    tag = InventoryManagerConfig.IngotsContainerTag;
                    break;
                case 2:
                    tag = InventoryManagerConfig.ComponentsContainerTag;
                    break;
                case 3:
                    tag = InventoryManagerConfig.MiscContainerTag;
                    break;
            }

            //cancel if not found
            if (string.IsNullOrWhiteSpace(tag))
            {
                return null;
            }

            //get possible targets, must have 10% volume left and be connected to source inventory
            var targets = new List<IMyTerminalBlock>(25);
            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(targets, x => x.CubeGrid == GridProgram.Me.CubeGrid && x is IMyInventoryOwner && x.CustomName.Contains(tag) && ((double)x.GetInventory(0).CurrentVolume / (double)x.GetInventory(0).MaxVolume < 0.9) && x.GetInventory(0).IsConnectedTo(sourceInventory));

            //return first target
            if (targets.Count > 0)
            {
                return targets[0];
            }

            //nothing found
            return null;
        }

        /// <summary>
        /// Transfers given item from source to target inventory. Transfers all if no amount given
        /// </summary>
        /// <param name="sourceInventory"></param>
        /// <param name="targetInventory"></param>
        /// <param name="itemIndex"></param>
        /// <param name="amount"></param>
        protected void TransferItem(IMyInventory sourceInventory, IMyInventory targetInventory, int itemIndex, double amount = double.NaN)
        {
            var item = sourceInventory.GetItems()[itemIndex];

            //get amount
            if (double.IsNaN(amount))
            {
                amount = (double)item.Amount;
            }
            {
                amount = Math.Min(amount, (double)item.Amount);
            }

            //get target index
            var targetIndex = GetIndexForItem(targetInventory, item.Content.TypeId.ToString(), item.Content.SubtypeId.ToString());

            //transfer item
            sourceInventory.TransferItemTo(targetInventory, itemIndex, targetIndex == -1 ? 0 : targetIndex, true, (int)amount);
        }

        /// <summary>
        /// Returns index in inventory for item with given type and subtype
        /// 
        /// index -1 if not found
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="typeId"></param>
        /// <param name="subtypeId"></param>
        /// <returns></returns>
        protected int GetIndexForItem(IMyInventory inventory, string typeId, string subtypeId)
        {
            var index = 0;

            var items = inventory.GetItems();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item.Content.TypeId.ToString() == typeId && item.Content.SubtypeId.ToString() == subtypeId)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        #endregion

        #endregion

        /**End copy here**/
    }
}
