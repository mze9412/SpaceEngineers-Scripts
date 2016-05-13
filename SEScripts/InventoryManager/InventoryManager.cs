using System;
using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage.Game.ModAPI.Ingame;

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
            Inventories = new List<Inventory>();

            Stages = new LinkedList<string>();
            Stages.AddLast("EmptySources");
            Stages.AddLast("SortOutOre");
            Stages.AddLast("SortOutIngots");
            Stages.AddLast("SortOutComponents");
            Stages.AddLast("SortOutMisc");
            Stages.AddLast("EmptyRefineries");
            Stages.AddLast("EmptyAssemblers");
            Stages.AddLast("BalanceRefineries");
            Stages.AddLast("PullFromConnectedGrids");

            CurrentStage = Stages.First;
        }

        #region Properties

        private const string DisplayId = "D67FEC05-0720-4C9F-89FE-634B9F73F17F";
        private const string LCDName = "LCD_Inventory";

        private MyGridProgram GridProgram { get; set; }

        /// <summary>
        /// List of all detected inventories
        /// </summary>
        private List<Inventory> Inventories { get; set; }

        /// <summary>
        /// Helper to count executions
        /// </summary>
        private int RunCounter { get; set; }

        /// <summary>
        /// Progress for resume
        /// </summary>
        public LinkedListNode<string> CurrentStage { get; set; }

        private LinkedList<string> Stages { get; set; } 

        #endregion

        /// <summary>
        /// Runs the manager
        /// </summary>
        /// <param name="argument"></param>
        public void Run(string argument)
        {
            //create LCD
            var lcds = new List<IMyTerminalBlock>(10);
            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, x => x.CubeGrid == GridProgram.Me.CubeGrid && x.CustomName == LCDName);
            if (lcds.Count > 0)
            {
                GridProgram.Echo("Creating LCD");
                LCDHelper.WriteLine(DisplayId, "Creating LCD");
                LCDHelper.CreateDisplay(DisplayId, (IMyTextPanel)lcds[0]);
            }
            else
            {
                throw new Exception("Failed to find LCD?!");
            }

            LCDHelper.WriteLine(DisplayId, "RunCounter: " + RunCounter);

            //Every 10th run or if no inventories have been found -> update inventories
            //otherwise: just verify, that they stille exist
            if (RunCounter%10 == 0 || Inventories.Count == 0)
            {
                DetectInventories();
            }
            else
            {
                VerifyInventories();
            }

            //try to empty all source inventories
            TryRunSorting("EmptySources", InventoryType.Source);

            //sort out targets
            TryRunSorting("SortOutOre", InventoryType.TargetOre);
            TryRunSorting("SortOutIngots", InventoryType.TargetIngots);
            TryRunSorting("SortOutComponents", InventoryType.TargetComponents);
            TryRunSorting("SortOutMisc", InventoryType.TargetMisc);

            //empty refinery & assembler output inventories
            TryRunSorting("EmptyRefineries", InventoryType.Refinery);
            TryRunSorting("EmptyAssemblers", InventoryType.Assembler);

            //manage inventory in reactors

            //manage refinery inputs
            TryRunRefineryBalance();

            //manage oxygen generators

            //pull from connected grids
            TryPullFromConnectedGrids();

            //increase runcounter
            RunCounter++;

            //reset to first stage
            if (CurrentStage == null)
            {
                CurrentStage = Stages.First;
            }

            //print and delete LCD
            LCDHelper.PrintDisplay(DisplayId);
            LCDHelper.DeleteDisplay(DisplayId);
        }

        private void TryPullFromConnectedGrids()
        {
            if (CurrentStage.Value == "PullFromConnectedGrids" && AreInstructionsAvailable)
            {
                LCDHelper.WriteLine(DisplayId, "Pulling from connected grids");
                if (InventoryManagerConfig.PullFromAttachedGrids)
                {
                    //get all connected
                    var connectors = new List<IMyTerminalBlock>(25);
                    GridProgram.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, x => x.CubeGrid == GridProgram.Me.CubeGrid);

                    LCDHelper.WriteLine(DisplayId, "Found " + connectors.Count + "connectors.");
                    //check if connector has someone connected
                    foreach (var conn in connectors)
                    {
                        if (!AreInstructionsAvailable)
                        {
                            break;
                        }
                        var c = (IMyShipConnector) conn;
                        var partner = c.OtherConnector;
                        if (partner != null && !partner.CustomName.Contains(InventoryManagerConfig.IgnoreContainerTag))
                        {
                            LCDHelper.WriteLine(DisplayId, "Pulling from partner " + partner.CustomName);
                            //get all inventories of partner grid, except reactors and oxy gens (do not pull from those!) and except all containing the ignore tag
                            var inventories = new List<IMyTerminalBlock>(25);
                            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(inventories, x => x.CubeGrid == partner.CubeGrid && x is IMyInventoryOwner && !(x is IMyReactor || x is IMyOxygenGenerator) && !x.CustomName.Contains(InventoryManagerConfig.IgnoreContainerTag));

                            LCDHelper.WriteLine(DisplayId, "Partner has " + inventories.Count + " inventories to empty.");
                            for (int i = 0; i < inventories.Count && AreInstructionsAvailable; i++)
                            {
                                var inv = new Inventory {Block = inventories[i], InventoryType = InventoryType.Source};
                                for (int j = 0; j < inv.Block.GetInventoryCount(); j++)
                                {
                                    EmptyInventory(inv, inv.Block.GetInventory(j));
                                }
                            }
                        }
                    }
                }

                CurrentStage = CurrentStage.Next;
            }
        }

        private void TryRunRefineryBalance()
        {
            if (CurrentStage.Value == "BalanceRefineries" && AreInstructionsAvailable)
            {
                LCDHelper.WriteLine(DisplayId, "Running refinery balancing");
                if (InventoryManagerConfig.BalanceRefineries)
                {
                    //get ore sources
                    var oreSources = GetInventories(InventoryType.TargetOre);

                    //get refineries
                    var refineries = GetInventories(InventoryType.Refinery);
                    
                    //split first found ore on each run
                    bool foundItem = false;
                    foreach (var src in oreSources)
                    {
                        LCDHelper.WriteLine(DisplayId, "Checking ore source " + src.Block.CustomName);
                        var inv = src.Block.GetInventory(0);
                        var items = inv.GetItems();
                        for (int i = 0; i < items.Count && AreInstructionsAvailable; i++)
                        {
                            LCDHelper.WriteLine(DisplayId, "Checking item " + i);
                            var item = items[i];
                            if (ItemIdHelper.IsOre(item.Content.TypeId.ToString()) && !ItemIdHelper.IsIce(item.Content.TypeId.ToString(), item.Content.SubtypeId.ToString()))
                            {
                                //try to add 1000 to every empty refinery on each pass if refinery is empty
                                foreach (var r in refineries)
                                {
                                    //get target index
                                    var target = r.Block.GetInventory(0);

                                    //only fill refinery if it is empty
                                    var targetItems = target.GetItems();
                                    if (targetItems.Count == 0)
                                    {
                                        //move ore to target
                                        var transferAmount = Math.Min((int)item.Amount, 1000);
                                        LCDHelper.WriteLine(DisplayId, "Item count before: " + item.Amount);
                                        inv.TransferItemTo(target, i, 0, true, transferAmount);
                                        LCDHelper.WriteLine(DisplayId, "Item count after: " + item.Amount);
                                        break;
                                    }
                                }
                                
                                foundItem = true;
                                break;
                            }
                        } 

                        //break if work is done
                        if (foundItem)
                        {
                            break;
                        }
                    }
                }

                //next stage
                CurrentStage = CurrentStage.Next;
            }
        }

        /// <summary>
        /// Trys to sort given inventory type
        /// </summary>
        /// <param name="targetStage"></param>
        /// <param name="inventoryType"></param>
        private void TryRunSorting(string targetStage, string inventoryType)
        {
            if (CurrentStage.Value == targetStage && AreInstructionsAvailable)
            {
                EmptyInventories(inventoryType);
                CurrentStage = CurrentStage.Next;
            }
        }

        /// <summary>
        /// Helper to check for instruction limit
        /// </summary>
        private bool AreInstructionsAvailable
        {
            get
            {
                var instructionsUsed = (double)GridProgram.Runtime.CurrentInstructionCount/ (double)GridProgram.Runtime.MaxInstructionCount;
                if (instructionsUsed > 0.3)
                {
                    LCDHelper.WriteLine(DisplayId, "Aborting. Too many instructions used!");
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Empties given intentory types
        /// </summary>
        /// <param name="inventoryType"></param>
        private void EmptyInventories(string inventoryType)
        {
            LCDHelper.WriteLine(DisplayId, "Emptying inventories of type " + inventoryType);
            var inventories = GetInventories(inventoryType);
            foreach (Inventory t in inventories)
            {
                var owner = (IMyInventoryOwner)t.Block;
                int inventoryIndex = 0;
                if (t.InventoryType == InventoryType.Refinery || t.InventoryType == InventoryType.Assembler)
                {
                    inventoryIndex = 1;
                }
                EmptyInventory(t, owner.GetInventory(inventoryIndex));
            }
        }

        /// <summary>
        /// Empties given inventory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceInventory"></param>
        private void EmptyInventory(Inventory source, IMyInventory sourceInventory)
        {
            var items = sourceInventory.GetItems();
            for (int i = items.Count-1; i >= 0 && AreInstructionsAvailable; i--)
            {
                var item = items[i];
                var targetType = GetTargetType(item);
                if (targetType != source.InventoryType)
                {
                    var target = GetTarget(sourceInventory, targetType);
                    if (target != null)
                    {
                        //get target index
                        var targetIndex = 0;
                        var targetItems = target.GetInventory(0).GetItems();
                        for (int k = 0; k < targetItems.Count && AreInstructionsAvailable; k++)
                        {
                            var targetItem = targetItems[k];
                            if (targetItem.Content.TypeId.ToString() == item.Content.TypeId.ToString() && targetItem.Content.SubtypeId.ToString() == item.Content.SubtypeId.ToString())
                            {
                                targetIndex = k;
                                break;
                            }
                        }

                        if (!AreInstructionsAvailable)
                        {
                            break;
                        }

                        bool success = sourceInventory.TransferItemTo(target.GetInventory(0), i, targetIndex, true, item.Amount);
                        LCDHelper.WriteLine(DisplayId, "Moving item " + item.Content.TypeId + "_" + item.Content.SubtypeId + " from " + source.Block.CustomName + " to " + target.CustomName + ". " + (success ? "Done." : "Failed!"));
                    }
                }
            }
        }

        /// <summary>
        /// Finds a target inventory to transfer the given item to. Connection must exist between source and target
        /// </summary>
        /// <param name="sourceInventory"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private IMyTerminalBlock GetTarget(IMyInventory sourceInventory, string targetType)
        {
            //return first correct target that can 
            var targets = GetInventories(targetType);
            foreach (Inventory t in targets)
            {
                var target = (IMyInventoryOwner) t.Block;
                if (target.GetInventory(0).IsConnectedTo(sourceInventory))
                {
                    return t.Block;
                }
            }
            
            return null;
        }

        private string GetTargetType(IMyInventoryItem item)
        {
            var targetType = InventoryType.Unknown;
            if (item.Content.TypeId.ToString().EndsWith("Ore"))
            {
                targetType = InventoryType.TargetOre;
            }
            else if (item.Content.TypeId.ToString().EndsWith("Ingot"))
            {
                targetType = InventoryType.TargetIngots;
            }
            else if (item.Content.TypeId.ToString().EndsWith("Component"))
            {
                targetType = InventoryType.TargetComponents;
            }
            else
            {
                targetType = InventoryType.TargetMisc;
            }
            return targetType;
        }

        #region Inventory detection & checking

        /// <summary>
        /// Detects inventories on grid
        /// </summary>
        private void DetectInventories()
        {
            LCDHelper.WriteHeader(DisplayId, "DetectInventories");

            Inventories.Clear();

            //get inventory blocks
            var blocks = new List<IMyTerminalBlock>(100);
            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, x => x.CubeGrid == GridProgram.Me.CubeGrid && x is IMyInventoryOwner && x.HasInventory() && !x.CustomName.Contains(InventoryManagerConfig.IgnoreContainerTag) && !(x is IMyUserControllableGun));


            LCDHelper.WriteLine(DisplayId, "Found blocks: " + blocks.Count);

            //classify all blocks
            foreach (var block in blocks)
            {
                var inventoryType = InventoryType.Unknown;
                if (block is IMyRefinery)
                {
                    inventoryType = InventoryType.Refinery;

                    LCDHelper.WriteLine(DisplayId, "Checking refinary UseConveyor");
                    //set UseConveyor correctly
                    var useConveyor = block.GetValue<bool>("UseConveyor");
                    if (useConveyor && InventoryManagerConfig.BalanceRefineries)
                    {
                        block.SetValue("UseConveyor", false);
                    }
                    else if (!useConveyor && !InventoryManagerConfig.BalanceRefineries)
                    {
                        block.SetValue("UseConveyor", true);
                    }
                }
                else if (block is IMyAssembler)
                {
                    inventoryType = InventoryType.Assembler;
                }
                else if (block is IMyReactor)
                {
                    inventoryType = InventoryType.Reactor;

                    LCDHelper.WriteLine(DisplayId, "Checking reactor UseConveyor");

                    //set UseConveyor correctly
                    var useConveyor = block.GetValue<bool>("UseConveyor");
                    if (useConveyor && InventoryManagerConfig.BalanceRefineries)
                    {
                        block.SetValue("UseConveyor", false);
                    }
                    else if (!useConveyor && !InventoryManagerConfig.BalanceRefineries)
                    {
                        block.SetValue("UseConveyor", true);
                    }
                }
                else if (block is IMyOxygenGenerator)
                {
                    inventoryType = InventoryType.OxygenGenerator;
                }
                else
                {
                    if (block.CustomName.Contains(InventoryManagerConfig.OreContainerTag))
                    {
                        inventoryType = InventoryType.TargetOre;
                    }
                    else if (block.CustomName.Contains(InventoryManagerConfig.IngotsContainerTag))
                    {
                        inventoryType = InventoryType.TargetIngots;
                    }
                    else if (block.CustomName.Contains(InventoryManagerConfig.ComponentsContainerTag))
                    {
                        inventoryType = InventoryType.TargetComponents;
                    }
                    else if (block.CustomName.Contains(InventoryManagerConfig.MiscContainerTag))
                    {
                        inventoryType = InventoryType.TargetMisc;
                    }
                    else
                    {
                        inventoryType = InventoryType.Source;
                    }
                }
                Inventories.Add(new Inventory {Block = block, InventoryType = inventoryType});
            }
        }

        /// <summary>
        /// Verifies that inventories are still working
        /// </summary>
        private void VerifyInventories()
        {
            //check inventories
            var inventoriesToRemove = new List<Inventory>();
            foreach (var inv in Inventories)
            {
                if (!inv.Block.IsFunctional || !inv.Block.IsWorking)
                {
                    inventoriesToRemove.Add(inv);
                }
            }

            //remove from list
            for (int i = 0; i < Inventories.Count; i++)
            {
                var inv = Inventories[i];
                Inventories.Remove(inv);
            }
        }

        #endregion

        #region Inventory helpers

        /// <summary>
        /// Returns inventories of given type via "yield return"
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<Inventory> GetInventories(string type)
        {
            var list = new List<Inventory>();
            for (int i = 0; i < Inventories.Count; i++)
            {
                var inv = Inventories[i];
                if (inv.InventoryType == type)
                {
                    list.Add(inv);
                }
            }
            return list;
        }

        #endregion

        /**End copy here**/
    }
}
