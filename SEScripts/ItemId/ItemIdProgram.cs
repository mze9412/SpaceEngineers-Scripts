using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.ItemId
{
    public sealed class ItemIdProgram : MyGridProgram
    {
        /**Begin copy here**/

        public void Main(string argument)
        {
            var items = new List<string>();

            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, x => x is IMyInventoryOwner);

            foreach (var block in blocks)
            {
                var b = (IMyInventoryOwner) block;
                for (int i = 0; i < b.InventoryCount; i++)
                {
                    var inv = b.GetInventory(i);
                    foreach (var item in inv.GetItems())
                    {
                        var id = item.Content.TypeId + " // " + item.Content.SubtypeId + " // " + item.Content.SubtypeName;
                        if (!items.Contains(id))
                        {
                            items.Add(id);
                        }
                    }
                }
            }

            //sort
            items.Sort();

            var lcds = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, x => x.CubeGrid == Me.CubeGrid && x.CustomName == "LCD_ItemIds");

            if (lcds.Count > 0)
            {
                var lcd = (IMyTextPanel) lcds[0];
                lcd.WritePublicText("");
                foreach (var str in items)
                {
                    lcd.WritePublicText(str + "\n", true);
                    lcd.ShowTextureOnScreen();
                    lcd.ShowPublicTextOnScreen();
                }
            }
        }

        /**End copy here**/
    }
}
