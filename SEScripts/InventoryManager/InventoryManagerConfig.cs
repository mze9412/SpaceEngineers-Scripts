namespace mze9412.SEScripts.InventoryManager
{
    /**Begin copy here**/

    public static class InventoryManagerConfig
    {
        #region Settings

        /// <summary>
        /// If true the script tries to pull from directly attached grids and consolidate their inventory into its own
        /// </summary>
        public static bool PullFromAttachedGrids { get; set; }
        
        /// <summary>
        /// If true, refineries are kept balanced (deviation of +-20% allowed)
        /// </summary>
        public static bool BalanceRefineries { get; set; }

        /// <summary>
        /// Makes sure to keep reactors on 10 ingots maximum, keepts rest in containers
        /// </summary>
        public static bool RestrictReactorIngots { get; set; }

        /// <summary>
        /// Tag for ore targets
        /// </summary>
        public static string OreContainerTag { get; set; }

        /// <summary>
        /// Tag for ingot targets
        /// </summary>
        public static string IngotsContainerTag { get; set; }

        /// <summary>
        /// Tag for components targets
        /// </summary>
        public static string ComponentsContainerTag { get; set; }

        /// <summary>
        /// Tag for ignore in sorting
        /// </summary>
        public static string IgnoreContainerTag { get; set; }

        /// <summary>
        /// Tag for misc items in sorting
        /// </summary>
        public static string MiscContainerTag { get; set; }

        #endregion

        /**End copy here**/
    }
}
