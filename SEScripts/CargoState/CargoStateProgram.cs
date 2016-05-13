using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.CargoState
{
    public sealed class CargoStateProgram : MyGridProgram
    {
        /**Begin copy here**/

        /// <summary>
        /// Name of the LCD to print on
        /// </summary>
        private static string LCDName = "LCD_CargoState";

        /// <summary>
        /// DO NOT MODIFY
        /// </summary>
        private static string DisplayId = "FE5FC12B-C6CC-4909-9F9B-841BD3E36A9A";

        /// <summary>
        /// Ctor
        /// </summary>
        public CargoStateProgram() //#replace(CargoStateProgram,Program)
        {
        }

        public void Main(string argument)
        {
            //create LCD
            var lcds = new List<IMyTerminalBlock>(10);
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, x => x.CubeGrid == Me.CubeGrid && x.CustomName == LCDName);
            if (lcds.Count > 0)
            {
                LCDHelper.CreateDisplay(DisplayId, (IMyTextPanel)lcds[0]);
            }

            //write cargo status (cargos and connectors)
            PrintOverview();
            LCDHelper.WriteLine(DisplayId, "");

            //write oxygen generator status
            PrintOxygen();
            LCDHelper.WriteLine(DisplayId, "");

            //print and delete LCD
            LCDHelper.PrintDisplay(DisplayId);
            LCDHelper.DeleteDisplay(DisplayId);
        }

        /// <summary>
        /// Prints cargo overview, includes connectors and cargo containers, excludes drills, assemblers, etc.
        /// </summary>
        private void PrintOverview()
        {
            //get cargos
            var cargos = new List<IMyTerminalBlock>(10);
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(cargos, x => x.CubeGrid == Me.CubeGrid && (x is IMyCargoContainer || x is IMyShipConnector));

            LCDHelper.WriteLine(DisplayId, "__Cargo containers / connectors");
            if (cargos.Count > 0)
            {
                var totalVolume = 0f;
                var maxVolume = 0f;

                var totalMass = 0f;
                foreach (var cargo in cargos)
                {
                    var owner = cargo as IMyInventoryOwner;

                    totalVolume += owner.GetInventory(0).CurrentVolume.RawValue;
                    maxVolume += owner.GetInventory(0).MaxVolume.RawValue;

                    totalMass += owner.GetInventory(0).CurrentMass.RawValue;
                }
                var percentFilled = totalVolume / maxVolume;

                //write to LCD
                LCDHelper.WriteFormattedLine(DisplayId, "Mass:  {0:0.00} kg", totalMass/1000/1000);
                LCDHelper.WriteProgressBar(DisplayId, "Fill:", percentFilled);
            }
            else
            {
                LCDHelper.WriteLine(DisplayId, "No cargos or connectors found.");
            }
        }

        /// <summary>
        /// Prints oxygen generator data
        /// </summary>
        private void PrintOxygen()
        {
            //get cargos
            var generators = new List<IMyTerminalBlock>(10);
            GridTerminalSystem.GetBlocksOfType<IMyOxygenGenerator>(generators, x => x.CubeGrid == Me.CubeGrid);

            LCDHelper.WriteLine(DisplayId, "__Oxygen generators__");
            if (generators.Count > 0)
            {
                var totalVolume = 0f;
                var maxVolume = 0f;

                var totalMass = 0f;
                foreach (var gen in generators)
                {
                    var g = (IMyOxygenGenerator)gen;

                    totalVolume += g.GetInventory(0).CurrentVolume.RawValue;
                    maxVolume += g.GetInventory(0).MaxVolume.RawValue;

                    totalMass += g.GetInventory(0).CurrentMass.RawValue;
                }
                var percentFilled = totalVolume / maxVolume;

                //write to LCD
                LCDHelper.WriteFormattedLine(DisplayId, "Mass:  {0:0.00} kg", totalMass/1000/1000);
                LCDHelper.WriteProgressBar(DisplayId, "Fill:", percentFilled);
            }
            else
            {
                LCDHelper.WriteLine(DisplayId, "No cargos or connectors found.");
            }
        }

        //#include(../Libraries/LCDHelper.cs,false)

        /**End copy here**/
    }
}
