using Sandbox.ModAPI.Ingame;

namespace mze9412.SEScripts.InventoryManager
{
    /**Begin copy here**/
    
    /// <summary>
    /// Represents a found inventory
    /// </summary>
    public struct Inventory
    {
        /// <summary>
        /// Type of the inventory
        /// </summary> 
        public string InventoryType { get; set; }

        /// <summary>
        /// The inventory represented by this struct instance
        /// </summary>
        public IMyTerminalBlock Block { get; set; }

        /**End copy here**/
    }

}
