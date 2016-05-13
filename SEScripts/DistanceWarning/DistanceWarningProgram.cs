using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;

namespace mze9412.SEScripts.DistanceWarning
{
    public sealed class DistanceWarningProgram : MyGridProgram
    {
        /**Begin copy here**/

        public const string LCDName = "LCD_DistanceWarning";

        public const string DisplayId = "B92DBFA8-EEB8-48F9-BD98-EBFA4A54EE5C";
        
        public void Main(string argument)
        {
            //create display
            var lcds = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, x => x.CubeGrid == Me.CubeGrid && x.CustomName == LCDName);
            LCDHelper.CreateDisplay(DisplayId, (IMyTextPanel) lcds[0]);

            //get sensors
            var sensors = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(sensors, x => x.CubeGrid == Me.CubeGrid);

            //get remote control
            IMyRemoteControl remote = null;
            var remotes = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(remotes, x => x.CubeGrid == Me.CubeGrid);
            if (remotes.Count > 0)
            {
                remote = (IMyRemoteControl)remotes[0];

                LCDHelper.WriteHeader(DisplayId, "Distance warnings.");
                //assign sensors
                foreach (var sensor in sensors)
                {
                    if (sensor.WorldMatrix.Forward == remote.WorldMatrix.Forward)
                    {
                        PrintSensorStatus("Front", (IMySensorBlock)sensor);
                    }
                    if (sensor.WorldMatrix.Forward == remote.WorldMatrix.Backward)
                    {
                        PrintSensorStatus("Back", (IMySensorBlock)sensor);
                    }
                    if (sensor.WorldMatrix.Forward == remote.WorldMatrix.Right)
                    {
                        PrintSensorStatus("Right", (IMySensorBlock)sensor);
                    }
                    if (sensor.WorldMatrix.Forward == remote.WorldMatrix.Left)
                    {
                        PrintSensorStatus("Left", (IMySensorBlock)sensor);
                    }
                    if (sensor.WorldMatrix.Forward == remote.WorldMatrix.Up)
                    {
                        PrintSensorStatus("Up", (IMySensorBlock)sensor);
                    }
                    if (sensor.WorldMatrix.Forward == remote.WorldMatrix.Down)
                    {
                        PrintSensorStatus("Down", (IMySensorBlock)sensor);
                    }
                }
            }
            else
            {
                LCDHelper.WriteLine(DisplayId, "No remote found. ERROR. ERROR. ERROR.");
            }

            //print and delete display
            LCDHelper.PrintDisplay(DisplayId);
            LCDHelper.DeleteDisplay(DisplayId);
        }

        private void PrintSensorStatus(string title, IMySensorBlock sensor)
        {
            LCDHelper.WriteLine(DisplayId, title + ": " + (sensor.IsActive ? "Warning" : "Ok"));
        }

        //#include(../Libraries/LCDHelper.cs,false)

        /**End copy here**/
    }
}
