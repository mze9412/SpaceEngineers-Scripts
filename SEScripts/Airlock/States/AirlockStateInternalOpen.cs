using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mze9412.SEScripts.Airlock.States
{
    public sealed class AirlockStateInternalOpen : AirlockStateBase
    {
        public AirlockStateInternalOpen(Airlock airlock)
            : base(airlock, "AirlockStateInternalOpen")
        {
            
        }

        protected override void RunCore(string argument)
        {
            //if someone stands in front of external door OR inside airlock -> cycle around
            if (Airlock.ExternalSensor.LastDetectedEntity != null || Airlock.AirlockSensor.LastDetectedEntity != null)
            {
                //go to state close + depressurize
            }
        }
    }
}
