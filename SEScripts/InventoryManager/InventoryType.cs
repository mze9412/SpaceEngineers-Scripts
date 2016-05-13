namespace mze9412.SEScripts.InventoryManager
{
    /**Begin copy here**/

    /// <summary>
    /// Enum to distinguish types of inventory
    /// </summary>
    public static class InventoryType
    {
        public static string Source = "Source";                 //inventory that should be emptied
        public static string TargetOre = "Ore";                 //inventory that should recieve ore
        public static string TargetIngots = "Ingots";           //inventory that should recieve ingots
        public static string TargetComponents = "Components";   //inventory that should recieve components
        public static string TargetMisc = "Misc";               //inventory that should recieve misc items (tools, bottles, etc)
        public static string Refinery = "Refinery";             //can be source and target (has two inventories)
        public static string Assembler = "Assembler";           //can be source and target
        public static string OxygenGenerator = "OxyGen";        //special inventory, only recieves ice by default and is never emptied automatically
        public static string Reactor = "Reactor";               //never empty reactors but might restrict their ingot content 
        public static string Unknown = "Unknown";               //inventories that the script finds but does not understand

        /**End copy here**/
    }

}
