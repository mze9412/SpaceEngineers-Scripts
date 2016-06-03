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
        /// Try to keep all refineries busy
        /// </summary>
        public static bool ManageRefineries { get; set; }

        /// <summary>
        /// Try to keep oxygen generators running
        /// </summary>
        public static bool ManageOxygenGenerators { get; set; }

        /// <summary>
        /// Manage ammo in large turrets
        /// </summary>
        public static bool ManageLargeTurretAmmo { get; set; }

        /// <summary>
        /// Makes sure to keep reactors on 10 ingots maximum, keepts rest in containers
        /// </summary>
        public static bool ManageReactors { get; set; }

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
