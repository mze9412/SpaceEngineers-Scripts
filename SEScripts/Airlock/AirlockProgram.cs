using System.Collections.Generic;
using mze9412.SEScripts.Airlock.States;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;

namespace mze9412.SEScripts.Airlock
{

    public sealed class AirlockProgram : MyGridProgram
    {
        /**Begin copy here**/

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
                        //start with all airlocks pressurized
                        var state = new AirlockStatePressurizing(airlock);
                        state.EnterState();
                        airlock.CurrentState = state;
                        Airlocks.Add(airlock);
                    }
                }
            }

            //run each airlock and cycle airlock into new state
            
            foreach (var airlock in Airlocks)
            {
                if (airlock.IsIntact)
                {
                    var state = airlock.CurrentState;
                    LCDHelper.WriteLine(DisplayId, string.Format("Airlock {0}: {1}", airlock.AirlockIndex, state.Describe()));
                    var newState = state.Run(argument, Runtime.TimeSinceLastRun);
                    if (newState != state)
                    {
                        state.ExitState();
                        newState.EnterState();
                        airlock.CurrentState = newState;
                    }
                }
                else
                {
                    LCDHelper.WriteLine(DisplayId, string.Format("Airlock {0}: ERROR ERROR ERROR", airlock.AirlockIndex));
                }
            }

            //print and delete LCD
            LCDHelper.PrintDisplay(DisplayId);
            LCDHelper.DeleteDisplay(DisplayId);
        }

        //#include(States/AirlockStateBase.cs,false)
        //#include(States/AirlockStateDepressurizing.cs,false)
        //#include(States/AirlockStatePressurizing.cs,false)
        //#include(States/AirlockStateInternalOpen.cs,false)
        //#include(States/AirlockStateExternalOpen.cs,false)
        //#include(Airlock.cs,false)
        //#include(../Libraries/LCDHelper.cs,false)

        /**End copy here**/
    }
}
