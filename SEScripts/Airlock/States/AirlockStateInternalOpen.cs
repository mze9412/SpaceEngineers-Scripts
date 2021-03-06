﻿using System;

namespace mze9412.SEScripts.Airlock.States
{
    /**Begin copy here**/

    /// <summary>
    /// State:
    /// - Internal door open
    /// - External door closed
    /// - Pressurized
    /// </summary>
    public sealed class AirlockStateInternalOpen : AirlockStateBase
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="airlock"></param>
        public AirlockStateInternalOpen(Airlock airlock)
            : base(airlock, "AirlockStateInternalOpen")
        {

        }

        protected override string DescribeCore()
        {
            return "Pressurized. Internal open.";
        }

        /// <summary>
        /// State implementation
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override AirlockStateBase RunCore(string argument)
        {
            //abort if not yet 10s in this state
            if (TotalStateTime.TotalMilliseconds < 10000)
            {
                return this;
            }

            //if someone stands in front of external door OR inside airlock -> cycle around
            if (Airlock.ExternalSensor.LastDetectedEntity != null || Airlock.AirlockSensor.LastDetectedEntity != null)
            {
                //switch to new state of someone wants in or is standing outside
                return new AirlockStateDepressurizing(Airlock);
            }

            //state continues
            return this;
        }

        /// <summary>
        /// Do nothing here
        /// </summary>
        protected override void EnterStateCore()
        {
        }

        /// <summary>
        /// Do nothing here
        /// </summary>
        protected override void ExitStateCore()
        {
        }

        /**End copy here**/
    }
}
