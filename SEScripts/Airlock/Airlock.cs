using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace mze9412.SEScripts.Airlock
{
    public sealed class Airlock
    {
        public const string AirlockInternalSensorPattern = "[Airlock{0}] Sensor Internal";
        public const string AirlockExternalSensorPattern = "[Airlock{0}] Sensor External";
        public const string AirlockSensorPattern = "[Airlock{0}] Sensor Airlock";
        public const string AirlockInternalDoorPattern = "[Airlock{0}] Door Internal";
        public const string AirlockExternalDoorPattern = "[Airlock{0}] Door External";
        public const string AirlockAirVentPattern = "[Airlock{0}] AirVent";

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
            AirlockSensor = (IMySensorBlock)gridProgram.GridTerminalSystem.GetBlockWithName(string.Format(AirlockExternalSensorPattern, AirlockIndex));
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

        #region Helpers

        public bool IsPressurized
        {
            get { return AirVent.CanPressurize && !AirVent.IsDepressurizing; }
        }

        /// <summary>
        /// Locks down airlock
        /// </summary>
        public void Lockdown()
        {
            InternalDoor.Close();
            ExternalDoor.Close();
            AirVent.ApplyAction("Depressurize_Off");
        }

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
    }
}
