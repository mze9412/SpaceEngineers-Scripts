using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mze9412.SEScripts.Airlock.States
{
    public abstract class AirlockStateBase
    {
        protected AirlockStateBase(Airlock airlock, string name)
        {
            Name = name;
            Airlock = airlock;
        }

        public string Name { get; private set; }

        public Airlock Airlock { get; private set; }

        public void Run(string argument)
        {
            RunCore(argument);
        }

        protected abstract void RunCore(string argument);
    }
}
