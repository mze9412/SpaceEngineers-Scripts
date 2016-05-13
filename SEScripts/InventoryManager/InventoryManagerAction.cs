using System;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.InventoryManager
{
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
                throw new ArgumentNullException("gridProgram");
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
        public void Run(string argument)
        {
            RunCore(argument);
        }

        /// <summary>
        /// Internal run method
        /// </summary>
        /// <param name="argument"></param>
        protected abstract void RunCore(string argument);

        #region Helpers

        /// <summary>
        /// True of instruction count is still low enough
        /// </summary>
        protected bool CanContinueRun
        {
            get
            {
                return (double)GridProgram.Runtime.CurrentInstructionCount/GridProgram.Runtime.CurrentInstructionCount < 0.5;
            }
        }

        #region Transfer items helper

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
    }
}
