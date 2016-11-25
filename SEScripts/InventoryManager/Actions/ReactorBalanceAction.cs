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
    /// Makes sure reactors do not gobble up all ingots and do not run empty
    /// </summary>
    public sealed class ReactorBalanceAction : InventoryManagerAction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="gridProgram"></param>
        /// <param name="displayId"></param>
        public ReactorBalanceAction(MyGridProgram gridProgram, string displayId) : base(gridProgram, displayId, "Balance Reactors")
        {
            Reactors = new List<IMyReactor>();
            IngotsSources = new List<IMyTerminalBlock>();
        }

        /// <summary>
        /// Cached reactors
        /// </summary>
        private List<IMyReactor> Reactors { get; set; }

        /// <summary>
        /// Cached ingot sources
        /// </summary>
        private List<IMyTerminalBlock> IngotsSources { get; set; }

        /// <summary>
        /// Action logic
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override bool RunCore(string argument)
        {
            //get refineries if none cached
            if (Reactors.Count == 0)
            {
                //get refineries from own grid
                var reactors = new List<IMyTerminalBlock>(25);
                GridProgram.GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors, x => x.CubeGrid == GridProgram.Me.CubeGrid);

                //add to cache
                foreach (var r in reactors)
                {
                    Reactors.Add((IMyReactor)r);

                    //set use conveyor correctly
                    r.SetValue("UseConveyor", !InventoryManagerConfig.ManageReactors);
                }
            }

            if (InventoryManagerConfig.ManageReactors)
            {
                //try to find ice in source
                IMyInventory sourceInventory;
                int itemIndex;
                bool abortScript;
                if (GetUranium(out sourceInventory, out itemIndex, out abortScript))
                {
                    //check all generators
                    foreach (var reactor in Reactors)
                    {
                        //if one generator is broken, throw out all and redo next run
                        if (!reactor.IsFunctional)
                        {
                            Reactors.Clear();
                            return false;
                        }

                        //check how much uranium is in the reactor and try to fill up to 30 if below 25
                        decimal uraniumFound = 0;
                        var items = reactor.GetInventory(0).GetItems();
                        for (int i = 0; i < items.Count; i++)
                        {
                            var item = items[i];
                            if (ItemIdHelper.IsUranium(item.Content.TypeId.ToString(), item.Content.SubtypeId.ToString()))
                            {
                                uraniumFound += (decimal)item.Amount;
                            }
                        }

                        //try to fill and end loop
                        if (uraniumFound < 25)
                        {
                            var amount = Math.Min((decimal)sourceInventory.GetItems()[itemIndex].Amount, 30 - uraniumFound);
                            TransferItem(sourceInventory, reactor.GetInventory(0), itemIndex, amount);

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
        private bool GetUranium(out IMyInventory inventory, out int itemIndex, out bool abortScript)
        {
            //default
            abortScript = false;

            //get source inventories
            if (IngotsSources.Count == 0)
            {
                GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(IngotsSources, x => x.CubeGrid == GridProgram.Me.CubeGrid && x.CustomName.Contains(InventoryManagerConfig.IngotsContainerTag));
            }

            //check all sources
            foreach (var source in IngotsSources)
            {
                //if one source is broken, throw out all and redo next run
                if (!source.IsFunctional)
                {
                    IngotsSources.Clear();
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
                    if (ItemIdHelper.IsUranium(item.Content.TypeId.ToString(), item.Content.SubtypeId.ToString()))
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

    }
    /**End copy here**/
}
