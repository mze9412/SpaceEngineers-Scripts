using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;
using VRage.Library.Collections;

namespace mze9412.SEScripts.InventoryManager.Actions
{
    /**Begin copy here**/

    /// <summary>
    /// Collects items from connected grids
    /// </summary>
    public sealed class CollectFromConnectedGridsAction : InventoryManagerAction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="gridProgram"></param>
        /// <param name="displayId"></param>
        public CollectFromConnectedGridsAction(MyGridProgram gridProgram, string displayId) : base(gridProgram, displayId, "CollectFromConnectedGrids")
        {
            Connectors = new List<IMyShipConnector>();
        }

        /// <summary>
        /// Cached connectors
        /// </summary>
        private List<IMyShipConnector> Connectors { get; set; } 

        /// <summary>
        /// Action implementation ;)
        /// </summary>
        /// <param name="argument"></param>
        protected override bool RunCore(string argument)
        {
            //get connectors if none cached
            if (Connectors.Count == 0)
            {
                //get all own connectors from own grid
                var connectors = new List<IMyTerminalBlock>(25);
                GridProgram.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, x => x.CubeGrid == GridProgram.Me.CubeGrid);

                //cast them to correct type and add to cache
                foreach (var conn in connectors)
                {
                    Connectors.Add((IMyShipConnector)conn);
                }
            }

            //check all for connections
            foreach (var conn in Connectors)
            {
                //if one connector is broken, throw out all and redo next run
                if (!conn.IsWorking || !conn.IsFunctional)
                {
                    Connectors.Clear();
                    return false;
                }

                //get connected connector and continue if ignore tag is not set, ignore connected grid if connector has ignore tag
                var connectedConnector = conn.OtherConnector;
                if (connectedConnector != null && !connectedConnector.CustomName.Contains(InventoryManagerConfig.IgnoreContainerTag))
                {
                    var allDone = EmptyGrid(connectedConnector);

                    //we did not finish the grid because instruction limit reached -> abort
                    if (!allDone)
                    {
                        return false;
                    }
                }

                //cancel if too close to instruction count
                if (!CanContinueRun)
                {
                    return false;
                }
            }

            //all done
            return true;
        }

        /// <summary>
        /// Empties given grid and pushes all contents to own grid
        /// </summary>
        /// <param name="connectedConnector"></param>
        /// <returns></returns>
        private bool EmptyGrid(IMyShipConnector connectedConnector)
        {
            //get all blocks with inventories which are not a gun (i.e. turrets) and which do not use the Ignore keyworkd
            var sources = new List<IMyTerminalBlock>(100);
            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(sources, x => x.CubeGrid == connectedConnector.CubeGrid && x is IMyInventoryOwner && !(x is IMyUserControllableGun || x is IMyReactor) && !x.CustomName.Contains(InventoryManagerConfig.IgnoreContainerTag));

            //go through all blocks and empty them
            foreach (var source in sources)
            {
                //assemblers and refineries are only emptied from their second output inventory
                var inventoryIndex = 0;
                if (source is IMyAssembler || source is IMyRefinery)
                {
                    inventoryIndex = 1;
                }

                //get inventory and items
                var sourceInventory = source.GetInventory(inventoryIndex);
                var sourceItems = sourceInventory.GetItems();

                //iterate backwords over items (because removal of items messes up loop otherwise)
                for (int i = sourceItems.Count - 1; i >= 0; i--)
                {
                    //get item type
                    var item = sourceItems[i];
                    var type = ItemIdHelper.GetItemType(item.Content.TypeId.ToString());

                    //find cargo target and transfer
                    var target = GetTargetForItem(sourceInventory, type);
                    if (target != null)
                    {
                        TransferItem(sourceInventory, target.GetInventory(0), i);
                    }
                }

                //cancel if too close to instruction count
                if (!CanContinueRun)
                {
                    return false;
                }
            }

            //all done
            return true;
        }

        /**End copy here**/
    }
}
