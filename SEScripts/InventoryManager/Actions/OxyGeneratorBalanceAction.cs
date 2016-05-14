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
    /// Makes sure oxygen generators do not run out of ice
    /// </summary>
    public sealed class OxyGeneratorBalanceAction : InventoryManagerAction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="gridProgram"></param>
        /// <param name="displayId"></param>
        public OxyGeneratorBalanceAction(MyGridProgram gridProgram, string displayId) : base(gridProgram, displayId, "OxyGeneratorBalance")
        {
            Generators = new List<IMyOxygenGenerator>();
            OreSources = new List<IMyTerminalBlock>();
        }

        /// <summary>
        /// Cached oxy generators
        /// </summary>
        private List<IMyOxygenGenerator> Generators { get; set; }

        /// <summary>
        /// Cached ore sources
        /// </summary>
        private List<IMyTerminalBlock> OreSources { get; set; }

        /// <summary>
        /// Action logic
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override bool RunCore(string argument)
        {
            //get refineries if none cached
            if (Generators.Count == 0)
            {
                //get refineries from own grid
                var generators = new List<IMyTerminalBlock>(25);
                GridProgram.GridTerminalSystem.GetBlocksOfType<IMyOxygenGenerator>(generators, x => x.CubeGrid == GridProgram.Me.CubeGrid);

                //add to cache
                foreach (var gen in generators)
                {
                    Generators.Add((IMyOxygenGenerator)gen);

                    //set use conveyor correctly
                    gen.SetValue("UseConveyor", !InventoryManagerConfig.ManageOxygenGenerators);
                }
            }

            if (InventoryManagerConfig.ManageOxygenGenerators)
            {
                //try to find ice in source
                IMyInventory sourceInventory;
                int itemIndex;
                bool abortScript;
                if (GetIce(out sourceInventory, out itemIndex, out abortScript))
                {
                    //check all generators
                    foreach (var gen in Generators)
                    {
                        //if one generator is broken, throw out all and redo next run
                        if (!gen.IsWorking || !gen.IsFunctional)
                        {
                            Generators.Clear();
                            return false;
                        }

                        //check how much ice is in the generator and try to fill up to 10.000 if is below 7500
                        decimal iceFound = 0;
                        var items = gen.GetInventory(0).GetItems();
                        for (int i = 0; i < items.Count; i++)
                        {
                            var item = items[i];
                            if (ItemIdHelper.IsIce(item.Content.TypeId.ToString(), item.Content.SubtypeId.ToString()))
                            {
                                iceFound += (decimal)item.Amount;
                            }
                        }

                        //try to fill and end loop
                        if (iceFound < 7500)
                        {
                            var amount = Math.Min((decimal)sourceInventory.GetItems()[itemIndex].Amount, 10000 - iceFound);
                            TransferItem(sourceInventory, gen.GetInventory(0), itemIndex, amount);

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
        private bool GetIce(out IMyInventory inventory, out int itemIndex, out bool abortScript)
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
                if (!source.IsWorking || !source.IsFunctional)
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
                    if (ItemIdHelper.IsIce(item.Content.TypeId.ToString(), item.Content.SubtypeId.ToString()))
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
