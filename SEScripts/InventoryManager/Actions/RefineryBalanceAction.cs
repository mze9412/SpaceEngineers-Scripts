using System;
using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
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
        /// <param name="displayId"></param>
        public RefineryBalanceAction(MyGridProgram gridProgram, string displayId) : base(gridProgram, displayId, "Balance Refineries")
        {
            Refineries = new List<IMyRefinery>();
            OreSources = new List<IMyTerminalBlock>();
        }

        /// <summary>
        /// Cache for refineries
        /// </summary>
        private List<IMyRefinery> Refineries { get; set; } 

        /// <summary>
        /// Cache for ore sources
        /// </summary>
        private List<IMyTerminalBlock> OreSources { get; set; } 

        /// <summary>
        /// Action implementation ;)
        /// </summary>
        /// <param name="argument"></param>
        protected override bool RunCore(string argument)
        {
            //get refineries if none cached
            if (Refineries.Count == 0)
            {
                //get refineries from own grid
                var refineries = new List<IMyTerminalBlock>(25);
                GridProgram.GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineries, x => x.CubeGrid == GridProgram.Me.CubeGrid);

                //add to cache
                foreach (var r in refineries)
                {
                    Refineries.Add((IMyRefinery)r);

                    //set use conveyor correctly
                    r.SetValue("UseConveyor", !InventoryManagerConfig.ManageRefineries);
                }
            }

            if (InventoryManagerConfig.ManageRefineries)
            {
                //try to get ore to use
                IMyInventory sourceInventory;
                int itemIndex;
                bool abortScript;
                if (GetSourceOre(out sourceInventory, out itemIndex, out abortScript))
                {
                    //check all refineries
                    foreach (var r in Refineries)
                    {
                        //if one refinery is broken, throw out all and redo next run
                        if (!r.IsFunctional)
                        {
                            Refineries.Clear();
                            return false;
                        }

                        //transfer 1000 or amount (min) if ref is empty
                        var inv = r.GetInventory(0);
                        if (inv.GetItems().Count == 0)
                        {
                            var amount = Math.Min((decimal) sourceInventory.GetItems()[itemIndex].Amount, 1000);
                            TransferItem(sourceInventory, inv, itemIndex, amount);

                            //finished for now
                            return true;
                        }
                    }
                }

                //return with false if abortScript is true
                if (abortScript)
                {
                    return false;
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
        /// <param name="abortScript"></param>
        /// <returns></returns>
        private bool GetSourceOre(out IMyInventory inventory, out int itemIndex, out bool abortScript)
        {
            //default
            abortScript = false;

            //get source inventories
            if (OreSources.Count == 0)
            {
                GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(OreSources, x => x.CubeGrid == GridProgram.Me.CubeGrid && x.CustomName.Contains(InventoryManagerConfig.OreContainerTag));
            }

            //check all sources
            foreach (var source in OreSources)
            {
                //if one source is broken, throw out all and redo next run
                if (!source.IsFunctional)
                {
                    OreSources.Clear();
                    abortScript = true;
                    break;
                }

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
