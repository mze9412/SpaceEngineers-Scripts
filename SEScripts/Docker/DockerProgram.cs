using System.Collections.Generic;
using mze9412.SEScripts.Libraries;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.Definitions;
using VRageMath;

namespace mze9412.SEScripts.Docker
{
    public sealed class DockerProgram : MyGridProgram
    {
        /**Begin copy here**/

        public const string LCDName = "LCD PB1 (Docker)";

        public const string DisplayId = "7BA61619-3162-4E22-88A4-5F7E75253239";
        
        public void Main(string argument)
        {
            //create display
            var lcds = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, x => x.CubeGrid == Me.CubeGrid && x.CustomName == LCDName);
            LCDHelper.CreateDisplay(DisplayId, (IMyTextPanel) lcds[0]);
            
            //get all connectors
            var connectors = new List<IMyTerminalBlock>(10);
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, x => x.CustomName.StartsWith("Dock") && x.CubeGrid == Me.CubeGrid);

            //check state for each connector
            foreach (var conn in connectors)
            {
                //get lights group
                var group = GridTerminalSystem.GetBlockGroupWithName(conn.CustomName + " - Lights");
                if (group != null)
                {
                    var c = (IMyShipConnector) conn;
                    var lights = new List<IMyTerminalBlock>();
                    group.GetBlocksOfType<IMyInteriorLight>(lights);
                    foreach (var l in lights)
                    {
                        SetLightState((IMyInteriorLight)l, c.OtherConnector != null);
                    }
                }
            }

            //print and delete display
            LCDHelper.PrintDisplay(DisplayId);
            LCDHelper.DeleteDisplay(DisplayId);
        }

        private void SetLightState(IMyInteriorLight light, bool b)
        {
            light.SetValue("Color", b ? Color.Red : Color.Yellow);
        }

        //#include(../Libraries/LCDHelper.cs,false)

        /**End copy here**/
    }
}
