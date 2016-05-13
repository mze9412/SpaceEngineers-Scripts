using System;
using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.InventoryManager.Actions
{
    /**Begin copy here**/

    /// <summary>
    /// Fills refineries
    /// </summary>
    public sealed class RefineryBalanceAction : InventoryManagerAction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="gridProgram"></param>
        public RefineryBalanceAction(MyGridProgram gridProgram) : base(gridProgram, "RefineryBalance")
        {
        }

        /// <summary>
        /// Action implementation ;)
        /// </summary>
        /// <param name="argument"></param>
        protected override bool RunCore(string argument)
        {
            //get refineries
            var refineries = new List<IMyTerminalBlock>(25);
            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineries, x => x.CubeGrid == GridProgram.Me.CubeGrid);
            
            //try to get ore to use
            IMyInventory sourceInventory;
            int itemIndex;
            if (GetSourceOre(out sourceInventory, out itemIndex))
            {
                //check all refineries
                foreach (var r in refineries)
                {
                    //transfer 1000 or amount (min) if ref is empty
                    var inv = r.GetInventory(0);
                    if (inv.GetItems().Count == 0)
                    {
                        var amount = Math.Min((long)sourceInventory.GetItems()[itemIndex].Amount, 1000);
                        TransferItem(sourceInventory, inv, itemIndex, amount);

                        //finished for now
                        return true;
                    }
                }
            }

            //all done
            return true;
        }

        /// <summary>
        /// Try to found source ore in sources
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        private bool GetSourceOre(out IMyInventory inventory, out int itemIndex)
        {
            //get source inventories
            var sources = new List<IMyTerminalBlock>(25);
            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(sources, x => x.CubeGrid == GridProgram.Me.CubeGrid && x.CustomName.Contains(InventoryManagerConfig.OreContainerTag));


            foreach (var source in sources)
            {
                var inv = source.GetInventory(0);
                var items = inv.GetItems();
                
                //check all items
                for (int i = 0; i < items.Count; i++)
                {
                    //check item, return its values if correct one found
                    var item = items[i];
                    if (ItemIdHelper.IsOre(item.Content.TypeId.ToString()) && !ItemIdHelper.IsIce(item.Content.TypeId.ToString(), item.Content.SubtypeId.ToString()))
                    {
                        inventory = inv;
                        itemIndex = i;
                        return true;
                    }
                }
            }

            //not found, set null / -1
            inventory = null;
            itemIndex = -1;
            return false;
        }

        /**End copy here**/
    }
}
