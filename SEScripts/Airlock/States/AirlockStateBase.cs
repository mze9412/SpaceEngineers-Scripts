using System;

namespace mze9412.SEScripts.Airlock.States
{
    /**Begin copy here**/

    public abstract class AirlockStateBase
    {
        protected AirlockStateBase(Airlock airlock, string name)
        {
            Name = name;
            Airlock = airlock;
            TotalStateTime = new TimeSpan();
        }

        public string Name { get; private set; }

        public Airlock Airlock { get; private set; }

        public TimeSpan TotalStateTime { get; private set; }

        public AirlockStateBase Run(string argument, TimeSpan timeSinceLastRun)
        {
            TotalStateTime += timeSinceLastRun;
            return RunCore(argument);
        }

        public void EnterState()
        {
            EnterStateCore();
        }

        public void ExitState()
        {
            ExitStateCore();
        }

        public string Describe()
        {
            return DescribeCore();
        }

        protected abstract string DescribeCore();

        /// <summary>
        /// Runs the state (internal method)
        /// </summary>
        /// <param name="argument"></param>
        protected abstract AirlockStateBase RunCore(string argument);

        /// <summary>
        /// Enters the state (internal method)
        /// </summary>
        protected abstract void EnterStateCore();

        /// <summary>
        /// Exits the state (internal method)
        /// </summary>
        protected abstract void ExitStateCore();

        /**End copy here**/
    }
}
