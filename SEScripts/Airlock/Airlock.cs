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
            AirlockIndex = airlockIndex;
            InternalSensor = (IMySensorBlock)gridProgram.GridTerminalSystem.GetBlockWithName(string.Format(AirlockInternalSensorPattern, AirlockIndex));
            ExternalSensor = (IMySensorBlock)gridProgram.GridTerminalSystem.GetBlockWithName(string.Format(AirlockExternalSensorPattern, AirlockIndex));
            AirlockSensor = (IMySensorBlock)gridProgram.GridTerminalSystem.GetBlockWithName(string.Format(AirlockSensorPattern, AirlockIndex));
            InternalDoor = (IMyDoor)gridProgram.GridTerminalSystem.GetBlockWithName(string.Format(AirlockInternalDoorPattern, AirlockIndex));
            ExternalDoor = (IMyDoor)gridProgram.GridTerminalSystem.GetBlockWithName(string.Format(AirlockExternalDoorPattern, AirlockIndex));
            AirVent = (IMyAirVent)gridProgram.GridTerminalSystem.GetBlockWithName(string.Format(AirlockAirVentPattern, AirlockIndex));
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
            return block != null && block.IsWorking && block.IsFunctional;
        }


        #endregion

        /**End copy here**/
    }
}
