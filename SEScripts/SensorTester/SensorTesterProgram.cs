using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.SensorTester
{
    public sealed class SensorTesterProgram : MyGridProgram
    {
        /**Begin copy here**/

        /// <summary>
        /// Name of the LCD to print on
        /// </summary>
        private static string LCDName = "LCD_SensorTester";

        /// <summary>
        /// DO NOT MODIFY
        /// </summary>
        private static string DisplayId = "5EAC06C8-C4F0-40F0-B67F-321318F396DF";

        /// <summary>
        /// Ctor
        /// </summary>
        public SensorTesterProgram() //#replace(SensorTesterProgram,Program)
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

            var sensor = GridTerminalSystem.GetBlockWithName(argument) as IMySensorBlock;
            LCDHelper.WriteLine(DisplayId, "Detecting entity: " + (sensor.LastDetectedEntity != null));

            //print and delete LCD
            LCDHelper.PrintDisplay(DisplayId);
            LCDHelper.DeleteDisplay(DisplayId);
        }

        //#include(../Libraries/LCDHelper.cs,false)

        /**End copy here**/
    }
}
