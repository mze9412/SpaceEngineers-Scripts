using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.InventoryManager.Actions
{
    /**Begin copy here**/

    /// <summary>
    /// Handles turret ammo
    /// </summary>
    public sealed class AmmoBalanceAction : InventoryManagerAction
    {
        public const decimal MissileTargetCount = 5;
        public const decimal GatlingTargetCount = 10;

        public AmmoBalanceAction(MyGridProgram gridProgram, string displayId)
            : base(gridProgram, displayId, "AmmoBalance")
        {
            GatlingTurrets = new List<IMyLargeGatlingTurret>();
            MissileTurrets = new List<IMyLargeMissileTurret>();
        }

        private List<IMyLargeGatlingTurret> GatlingTurrets { get; set; }

        private List<IMyLargeMissileTurret> MissileTurrets { get; set; } 

        protected override bool RunCore(string argument)
        {
            #region Get Blocks

            //get gatlings if none cached
            if (GatlingTurrets.Count == 0)
            {
                //get refineries from own grid
                var gatlings = new List<IMyTerminalBlock>(25);
                GridProgram.GridTerminalSystem.GetBlocksOfType<IMyLargeGatlingTurret>(gatlings, x => x.CubeGrid == GridProgram.Me.CubeGrid);

                //add to cache
                foreach (var gat in gatlings)
                {
                    GatlingTurrets.Add((IMyLargeGatlingTurret)gat);

                    //set use conveyor correctly
                    gat.SetValue("UseConveyor", !InventoryManagerConfig.ManageLargeTurretAmmo);
                }
            }

            //get missiles if none cached
            if (MissileTurrets.Count == 0)
            {
                //get refineries from own grid
                var missileTurrets = new List<IMyTerminalBlock>(25);
                GridProgram.GridTerminalSystem.GetBlocksOfType<IMyLargeMissileTurret>(missileTurrets, x => x.CubeGrid == GridProgram.Me.CubeGrid);

                //add to cache
                foreach (var mt in missileTurrets)
                {
                    MissileTurrets.Add((IMyLargeMissileTurret)mt);

                    //set use conveyor correctly
                    mt.SetValue("UseConveyor", !InventoryManagerConfig.ManageLargeTurretAmmo);
                }
            }

            #endregion

            //only run logic if turned on
            if (InventoryManagerConfig.ManageLargeTurretAmmo)
            {
                bool allDone = ManageGatlings();
                if (!allDone)
                {
                    return false;
                }

                allDone = ManageMissiles();
                if (!allDone)
                {
                    return false;
                }
            }

            //all done
            return true;
        }

        private bool ManageGatlings()
        {
            //find ammo source
            IMyInventory sourceInventory;
            int itemIndex;
            if (GetAmmoSource(out sourceInventory, out itemIndex, true))
            {
                foreach (var gat in GatlingTurrets)
                {
                    //check execution counter
                    if (!CanContinueRun)
                    {
                        return false;
                    }

                    //check block
                    if (!gat.IsFunctional)
                    {
                        GatlingTurrets.Clear();
                        return false;
                    }

                    //check ammo level
                    var gatInventory = gat.GetInventory(0);
                    decimal ammoCount = 0;
                    foreach (var item in gatInventory.GetItems())
                    {
                        ammoCount += (decimal)item.Amount;
                    }
                    if (ammoCount < GatlingTargetCount)
                    {
                        //check how much is missing
                        var diff = GatlingTargetCount - ammoCount;

                        GridProgram.Echo("Index: " + itemIndex + " Diff: " + diff);
                        //transfer
                        TransferItem(sourceInventory, gat.GetInventory(0), itemIndex, diff);

                        //done for this run
                        return true;
                    }
                }
            }

            return true;
        }

        private bool ManageMissiles()
        {
            //find ammo source
            IMyInventory sourceInventory;
            int itemIndex;
            if (GetAmmoSource(out sourceInventory, out itemIndex, false))
            {
                foreach (var mt in MissileTurrets)
                {
                    //check execution counter
                    if (!CanContinueRun)
                    {
                        return false;
                    }

                    //check block
                    if (!mt.IsFunctional)
                    {
                        MissileTurrets.Clear();
                        return false;
                    }

                    //check ammo level
                    var mtInventory = mt.GetInventory(0);
                    decimal ammoCount = 0;
                    foreach (var item in mtInventory.GetItems())
                    {
                        ammoCount += (decimal)item.Amount;
                    }
                    if (ammoCount < MissileTargetCount)
                    {
                        //check how much is missing
                        var diff = MissileTargetCount - ammoCount;

                        //transfer
                        TransferItem(sourceInventory, mt.GetInventory(0), itemIndex, diff);

                        //done for this run
                        return true;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Try to find ammo source in sources
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="itemIndex"></param>
        /// <param name="gatling">true -> gatling; false -> missile</param>
        /// <returns></returns>
        private bool GetAmmoSource(out IMyInventory inventory, out int itemIndex, bool gatling)
        {
            var sources = new List<IMyTerminalBlock>(50);
            GridProgram.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(sources, x => x.CubeGrid == GridProgram.Me.CubeGrid && x.CustomName.Contains(InventoryManagerConfig.MiscContainerTag));

            //check all sources
            foreach (var source in sources)
            {
                //if one source is broken, skip
                if (!source.IsFunctional)
                {
                    continue;
                }

                var inv = source.GetInventory(0);
                var items = inv.GetItems();

                //check all items
                for (int i = 0; i < items.Count; i++)
                {
                    //check item, return its values if correct one found
                    var item = items[i];
                    if ((gatling && ItemIdHelper.IsGatlingAmmo(item.Content.TypeId.ToString(), item.Content.SubtypeId.ToString()))
                        || (!gatling && ItemIdHelper.IsMissileAmmo(item.Content.TypeId.ToString(), item.Content.SubtypeId.ToString())))
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
