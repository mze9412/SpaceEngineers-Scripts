using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.PowerState
{
    public sealed class PowerStateProgram : MyGridProgram
    {
        /**Begin copy here**/

        /// <summary>
        /// Name of the LCD to print on
        /// </summary>
        private static string LCDName = "LCD_PowerState";

        /// <summary>
        /// DO NOT MODIFY
        /// </summary>
        private static string DisplayId = "5EAC06C8-C4F0-40F0-B67F-321318F396DF";

        /// <summary>
        /// Ctor
        /// </summary>
        public PowerStateProgram() //#replace(PowerStateProgram,Program)
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

            //write reactor status
            PrintReactorStatus();
            LCDHelper.WriteLine(DisplayId, "");

            //write battery status 
            PrintBatteryStatus();
            LCDHelper.WriteLine(DisplayId, "");

            //write solar status
            PrintSolarStatus();
            LCDHelper.WriteLine(DisplayId, "");

            //print and delete LCD
            LCDHelper.PrintDisplay(DisplayId);
            LCDHelper.DeleteDisplay(DisplayId);
        }

        /// <summary>
        /// Prints a battery summary
        /// </summary>
        private void PrintBatteryStatus()
        {
            //get batteries
            var batteries = new List<IMyTerminalBlock>(10);
            GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, x => x.CubeGrid == Me.CubeGrid);

            LCDHelper.WriteHeader(DisplayId, string.Format("Batteries ({0})", batteries.Count));
            if (batteries.Count > 0)
            {
                var totalCharge = 0f;
                var maxCharge = 0f;

                var totalInput = 0f;
                var totalOutput = 0f;

                foreach (var bat in batteries)
                {
                    var b = (IMyBatteryBlock) bat;

                    totalInput += b.CurrentInput;
                    totalOutput += b.CurrentOutput;

                    maxCharge += b.MaxStoredPower;
                    totalCharge += b.CurrentStoredPower;
                }
                var percentCharged = totalCharge/maxCharge;

                //write to LCD
                LCDHelper.WriteFormattedLine(DisplayId, "Input:  {0:0.00} MW", totalInput);
                LCDHelper.WriteFormattedLine(DisplayId, "Output: {0:0.00} MW", totalOutput);
                LCDHelper.WriteProgressBar(DisplayId, "Charge:", percentCharged);
            }
            else
            {
                LCDHelper.WriteLine(DisplayId, "--- No batteries found ---");
            }
        }

        /// <summary>
        /// Prints summary of reactors
        /// </summary>
        private void PrintReactorStatus()
        {
            //get reactors
            var reactors = new List<IMyTerminalBlock>(10);
            GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors, x => x.CubeGrid == Me.CubeGrid);

            LCDHelper.WriteHeader(DisplayId, string.Format("Reactors ({0})", reactors.Count));
            if (reactors.Count > 0)
            {
                var maxOutput = 0f;
                var totalOutput = 0f;
                var totalFuel = 0f;

                foreach (var reactor in reactors)
                {
                    var r = (IMyReactor) reactor;
                    totalOutput += r.CurrentOutput;
                    maxOutput += r.MaxOutput;

                    var inv = r.GetInventory(0);
                    foreach (var item in inv.GetItems())
                    {
                        totalFuel += item.Amount.RawValue;
                    }
                }
                var percentOutput = totalOutput/maxOutput;

                //write to LCD
                LCDHelper.WriteFormattedLine(DisplayId, "Fuel: {0:0.00} kg", totalFuel / 1000 / 1000);
                LCDHelper.WriteFormattedLine(DisplayId, "Output: {0:0.00} MW", totalOutput);
                LCDHelper.WriteProgressBar(DisplayId, "Output (%):", percentOutput);
            }
            else
            {
                LCDHelper.WriteLine(DisplayId, "--- No reactors found ---");
            }
        }

        /// <summary>
        /// Prints a solar panel summary
        /// </summary>
        private void PrintSolarStatus()
        {
            //get solar
            var solars = new List<IMyTerminalBlock>(10);
            GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(solars, x => x.CubeGrid == Me.CubeGrid);

            LCDHelper.WriteHeader(DisplayId, string.Format("Solar panels ({0})", solars.Count));
            if (solars.Count > 0)
            {
                var maxOutput = 0f;
                var totalOutput = 0f;

                foreach (var solar in solars)
                {
                    var s = (IMySolarPanel)solar;
                    
                    maxOutput += s.MaxOutput;
                    totalOutput += s.CurrentOutput;
                }
                var percentOutput = totalOutput / maxOutput;

                //write to LCD
                LCDHelper.WriteFormattedLine(DisplayId, "Output:  {0:0.00} / {1:0.00} MW", totalOutput);
                LCDHelper.WriteProgressBar(DisplayId, "Output (5):", percentOutput);
            }
            else
            {
                LCDHelper.WriteLine(DisplayId, "--- No solar panels found ---");
            }
        }

        //#include(../Libraries/LCDHelper.cs,false)

        /**End copy here**/
    }
}
