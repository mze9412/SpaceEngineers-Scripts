using System;
using Sandbox.ModAPI.Ingame;

namespace mze9412.SEScripts.InventoryManager
{
    public sealed class InventoryManagerProgram : MyGridProgram
    {
        /**Begin copy here**/
            
        public InventoryManagerProgram() //#replace(InventoryManagerProgram,Program)
        {
            TimeSinceLastRun = new TimeSpan();
            Manager = new InventoryManager(this);

            //configure script
            InventoryManagerConfig.ManageRefineries         = true;
            InventoryManagerConfig.ManageReactors           = true;
            InventoryManagerConfig.ManageOxygenGenerators   = true;
            InventoryManagerConfig.PullFromAttachedGrids    = true;
            InventoryManagerConfig.OreContainerTag          = "[Ore]";
            InventoryManagerConfig.IngotsContainerTag       = "[Ingots]";
            InventoryManagerConfig.ComponentsContainerTag   = "[Components]";
            InventoryManagerConfig.IgnoreContainerTag       = "[Ignore]";
            InventoryManagerConfig.MiscContainerTag         = "[Misc]";
        }

        #region Properties

        /// <summary>
        /// Time since script ran last
        /// </summary>
        private TimeSpan TimeSinceLastRun { get; set; }

        private InventoryManager Manager { get; set; }

        #endregion

        /// <summary>
        /// Main execution method
        /// </summary>
        /// <param name="argument"></param>
        void Main(string argument)
        {
            //add up time since last run
            TimeSinceLastRun += Runtime.TimeSinceLastRun;

            //if time < 0.5s -> do not run
            if (TimeSinceLastRun.TotalMilliseconds < 250 && TimeSinceLastRun.TotalMilliseconds > 0)
            {
                return;
            }

            //run sorting logic
            Manager.Run(argument);
        }

        //#include(InventoryManagerConfig.cs,false)
        //#include(Actions/InventoryManagerAction.cs,false)
        //#include(Actions/CollectFromConnectedGridsAction.cs,false)
        //#include(Actions/CollectItemsAction.cs,false)
        //#include(Actions/RefineryBalanceAction.cs,false)
        //#include(Actions/OxyGeneratorBalanceAction.cs,false)
        //#include(Actions/ReactorBalanceAction.cs,false)
        //#include(../Libraries/LCDHelper.cs,false)
        //#include(../Libraries/ItemIdHelper.cs,false)
        //#include(InventoryManager.cs,false)

        /**End copy here**/
    }
}
