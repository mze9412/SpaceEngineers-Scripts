using System.Collections.Generic;
using mze9412.SEScripts.Airlock.States;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.Airlock
{
    /**Begin copy here**/

    public sealed class Airlock
    {
        public const string AirlockInternalSensorPattern = "[Airlock {0}] Sensor Internal";
        public const string AirlockExternalSensorPattern = "[Airlock {0}] Sensor External";
        public const string AirlockSensorPattern = "[Airlock {0}] Sensor Airlock";
        public const string AirlockInternalDoorPattern = "[Airlock {0}] Door Internal";
        public const string AirlockExternalDoorPattern = "[Airlock {0}] Door External";
        public const string AirlockAirVentPattern = "[Airlock {0}] AirVent";

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="airlockIndex"></param>
        /// <param name="gridProgram"></param>
        public Airlock(int airlockIndex, MyGridProgram gridProgram)
        {
            var blocks = new List<IMyTerminalBlock>(1);

            AirlockIndex = airlockIndex;

            //internal sensor
            gridProgram.GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(blocks, x => x.CubeGrid == gridProgram.Me.CubeGrid && x.CustomName == string.Format(AirlockInternalSensorPattern, AirlockIndex));
            if (blocks.Count == 1)
            {
                InternalSensor = (IMySensorBlock)blocks[0];
            }
            //external sensor
            gridProgram.GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(blocks, x => x.CubeGrid == gridProgram.Me.CubeGrid && x.CustomName == string.Format(AirlockExternalSensorPattern, AirlockIndex));
            if (blocks.Count == 1)
            {
                ExternalSensor = (IMySensorBlock)blocks[0];
            }
            //airlock sensor
            gridProgram.GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(blocks, x => x.CubeGrid == gridProgram.Me.CubeGrid && x.CustomName == string.Format(AirlockSensorPattern, AirlockIndex));
            if (blocks.Count == 1)
            {
                AirlockSensor = (IMySensorBlock)blocks[0];
            }
            //internal door
            gridProgram.GridTerminalSystem.GetBlocksOfType<IMyDoor>(blocks, x => x.CubeGrid == gridProgram.Me.CubeGrid && x.CustomName == string.Format(AirlockInternalDoorPattern, AirlockIndex));
            if (blocks.Count == 1)
            {
                InternalDoor = (IMyDoor)blocks[0];
            }
            //external door
            gridProgram.GridTerminalSystem.GetBlocksOfType<IMyDoor>(blocks, x => x.CubeGrid == gridProgram.Me.CubeGrid && x.CustomName == string.Format(AirlockExternalDoorPattern, AirlockIndex));
            if (blocks.Count == 1)
            {
                ExternalDoor = (IMyDoor)blocks[0];
            }
            //vent
            gridProgram.GridTerminalSystem.GetBlocksOfType<IMyAirVent>(blocks, x => x.CubeGrid == gridProgram.Me.CubeGrid && x.CustomName == string.Format(AirlockAirVentPattern, AirlockIndex));
            if (blocks.Count == 1)
            {
                AirVent = (IMyAirVent)blocks[0];
            }
        }

        public int AirlockIndex { get; private set; }

        public IMySensorBlock InternalSensor { get; private set; }

        public IMySensorBlock ExternalSensor { get; private set; }

        public IMySensorBlock AirlockSensor { get; private set; }

        public IMyDoor InternalDoor { get; private set; }

        public IMyDoor ExternalDoor { get; private set; }

        public IMyAirVent AirVent { get; private set; }

        public AirlockStateBase CurrentState { get; set; }

        #region Interaction with Airlock

        /// <summary>
        /// True if air vent can pressurize, vent is not depressurizing and pressure is greater than 95%
        /// </summary>
        public bool IsPressurized
        {
            get { return !AirVent.IsDepressurizing && AirVent.GetOxygenLevel() > 0.95; }
        }

        /// <summary>
        /// false if air vent can pressurize, vent is not depressurizing and pressure is smaller than 5%
        /// </summary>
        public bool IsDepressurized
        {
            get { return AirVent.IsDepressurizing && AirVent.GetOxygenLevel() < 0.05; }
        }

        /// <summary>
        /// Locks down airlock
        /// </summary>
        public void Lockdown()
        {
            CloseInternal();
            CloseExternal();
            Pressurize();
        }

        public void Pressurize()
        {
            AirVent.ApplyAction("Depressurize_Off");
        }

        public void Depressurize()
        {
            AirVent.ApplyAction("Depressurize_On");
        }

        public void OpenInternal()
        {
            InternalDoor.ApplyAction("Open_On");
        }

        public void OpenExternal()
        {
            ExternalDoor.ApplyAction("Open_On");
        }

        public void CloseInternal()
        {
            InternalDoor.ApplyAction("Open_Off");
        }

        public void CloseExternal()
        {
            ExternalDoor.ApplyAction("Open_Off");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check if airlock is still intact, if not it ceases to function until repaired
        /// </summary>
        public bool IsIntact
        {
            get { return IsIntactBlock(InternalSensor) && IsIntactBlock(ExternalSensor) && IsIntactBlock(InternalDoor) && IsIntactBlock(ExternalDoor) && IsIntactBlock(AirVent) && IsIntactBlock(AirlockSensor); }
        }

        /// <summary>
        /// Helper to check for intact blocks
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private bool IsIntactBlock(IMyTerminalBlock block)
        {
            return block != null && block.IsFunctional;
        }


        #endregion

        /**End copy here**/
    }
}
