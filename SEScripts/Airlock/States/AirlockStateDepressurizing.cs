namespace mze9412.SEScripts.Airlock.States
{
    /**Begin copy here**/

    /// <summary>
    /// State:
    /// - Closes internal door
    /// - Depressurizes room
    /// - Opens external door
    /// </summary>
    public sealed class AirlockStateDepressurizing : AirlockStateBase
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="airlock"></param>
        public AirlockStateDepressurizing(Airlock airlock) : base(airlock, "AirlockStateDepressurizing")
        {
        }

        protected override string DescribeCore()
        {
            return "Depressurizing ...";
        }

        /// <summary>
        /// State implementation
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override AirlockStateBase RunCore(string argument)
        {
            //if state was active for at least 1s
            //--> start pressurization
            if (TotalStateTime.TotalMilliseconds > 1000)
            {
                Airlock.Depressurize();
            }

            //open internal if pressure is high enough or after 10 seconds
            if (Airlock.IsDepressurized || TotalStateTime.TotalMilliseconds > 10000)
            {
                //got to new state
                return new AirlockStateExternalOpen(Airlock);
            }

            //state continues
            return this;
        }

        /// <summary>
        /// Enter state logic
        /// </summary>
        protected override void EnterStateCore()
        {
            Airlock.CloseInternal();
        }

        /// <summary>
        /// Exit state logic
        /// </summary>
        protected override void ExitStateCore()
        {
            //open door
            Airlock.OpenExternal();
        }

        /**End copy here**/
    }
}
