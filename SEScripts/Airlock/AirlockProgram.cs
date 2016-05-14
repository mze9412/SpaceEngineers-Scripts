using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;

namespace mze9412.SEScripts.Airlock
{
    public sealed class AirlockProgram : MyGridProgram
    {
        /// <summary>
        /// Name of the LCD to print on
        /// </summary>
        private static string LCDName = "LCD_Airlock";

        /// <summary>
        /// DO NOT MODIFY
        /// </summary>
        private static string DisplayId = "735F5602-9B2B-412F-A2B7-9346D2C3977C";

        /// <summary>
        /// Ctor
        /// </summary>
        public AirlockProgram() //#replace(AirlockProgram,Program)
        {
            Airlocks = new List<Airlock>();
        }

        /// <summary>
        /// Cached airlocks
        /// </summary>
        public List<Airlock> Airlocks { get; set; } 

        public void Main(string argument)
        {
            //create LCD
            var lcds = new List<IMyTerminalBlock>(10);
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, x => x.CubeGrid == Me.CubeGrid && x.CustomName == LCDName);
            if (lcds.Count > 0)
            {
                LCDHelper.CreateDisplay(DisplayId, (IMyTextPanel)lcds[0]);
            }

            //detect airlocks if count is 0 or if correct argument is written
            if (argument == "DetectAirlocks" || Airlocks.Count == 0)
            {
                //empty cache
                Airlocks.Clear();

                //get all airlocks
                for (int i = 0; i < 25; i++)
                {
                    var airlock = new Airlock(i + 1, this);
                    if (airlock.IsIntact)
                    {
                        Airlocks.Add(airlock);
                    }
                }
            }

            //run each airlock and cycle airlock into new state


            //print and delete LCD
            LCDHelper.PrintDisplay(DisplayId);
            LCDHelper.DeleteDisplay(DisplayId);
        }
    }
}
