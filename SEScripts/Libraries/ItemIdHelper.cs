
using System.Collections.Generic;

namespace mze9412.SEScripts.Libraries
{
    /**Begin copy here**/

    public static class ItemIdHelper
    {
        public const int TypeOre = 0;
        public const int TypeIngot = 1;
        public const int TypeComponent = 2;
        public const int TypeMisc = 3;

        private static readonly List<ItemDefinition> ItemDefinitions;

        static ItemIdHelper()
        {
            ItemDefinitions = new List<ItemDefinition>();

            //add known ores
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Ice",            "Ice"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Iron",           "Fe"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Stone",          "Stone"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Cobalt",         "Co"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Gold",           "Au"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Magnesium",      "Mg"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Nickel",         "Ni"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Platinum",       "Pt"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Silicon",        "Si"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Silver",         "Ag"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ore", "Uranium",        "U"));

            //add known ingots
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Cobalt",       "Co"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Gold",         "Au"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Iron",         "Fe"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Magnesium",    "Mg"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Nickel",       "Ni"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Platinum",     "Pt"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Silicon",      "Si"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Silver",       "Ag"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Stone",        "Gravel"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Ingot", "Uranium",      "U"));

            //add known components
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "Computer",             "Computer"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "Construction",         "Cons. Comp."));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "Detector",             "Detector"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "Girder",               "Girder"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "GravityGenerator ",    "Grav. Gen."));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "InteriorPlate ",       "Interior Pl. "));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "LargeTube",            "Lg. Steel Tube"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "Medical",              "Med. Comp."));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "MetalGrid",            "Metal Grid"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "Motor",                "Motor"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "Reactor",              "Reactor Comp."));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "SmallTube",            "Sm. Steel Tube"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_Component", "Thrust",               "Thruster Comp."));

            //add known misc
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_GasContainerObject", "HydrogenBottle",  "H² Bottle"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_GasContainerObject", "OxygenBottle",    "O² Bottle"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_AmmoMagazine", "Missile200mm",          "Missile"));
            ItemDefinitions.Add(new ItemDefinition("MyObjectBuilder_AmmoMagazine", "NATO_25x184mm",         "NATO_25x184mm"));
        }

        public static string GetDisplayName(string typeId, string subTypeId)
        {
            foreach (var item in ItemDefinitions)
            {
                if (item.TypeId == typeId && item.SubTypeId == subTypeId)
                {
                    return item.DisplayName;
                }
            }
            return string.Empty;
        }

        public static bool IsOre(string typeId)
        {
            return typeId.EndsWith("_Ore");
        }

        public static bool IsIce(string typeId, string subTypeId)
        {
            return typeId.EndsWith("_Ore") && subTypeId == "Ice";
        }

        public static bool IsComponent(string typeId)
        {
            return typeId.EndsWith("_Component");
        }

        public static bool IsIngot(string typeId)
        {
            return typeId.EndsWith("_Ingot");
        }

        public static bool IsMisc(string typeId)
        {
            return !IsOre(typeId) && !IsIngot(typeId) && !IsComponent(typeId);
        }

        /// <summary>
        /// Returns item type (int based helper)
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static int GetItemType(string typeId)
        {
            if (IsOre(typeId)) return TypeOre;
            if (IsIngot(typeId)) return TypeIngot;
            if (IsComponent(typeId)) return TypeComponent;

            return TypeMisc;
        }

        #region Item Definition class

        /// <summary>
        /// Container for known items
        /// </summary>
        private sealed class ItemDefinition
        {
            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="typeId"></param>
            /// <param name="subTypeId"></param>
            /// <param name="displayName"></param>
            public ItemDefinition(string typeId, string subTypeId, string displayName)
            {
                TypeId = typeId;
                SubTypeId = subTypeId;
                DisplayName = displayName;
            }

            /// <summary>
            /// TypeId, something like MyObjectBuilder_Component
            /// </summary>
            public string TypeId { get; private set; }

            /// <summary>
            /// SubTypeId, usually internal name (i.e. Motor)
            /// </summary>
            public string SubTypeId { get; private set; }

            /// <summary>
            /// Display name for LCD printing
            /// </summary>
            public string DisplayName { get; private set; }
        }

        #endregion
        /**End copy here**/
    }
}
