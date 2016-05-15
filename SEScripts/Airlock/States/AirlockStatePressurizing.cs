namespace mze9412.SEScripts.Airlock.States
{
    /**Begin copy here**/

    /// <summary>
    /// State:
    /// - Closes external door
    /// - Pressurizes room
    /// - Opens internal door
    /// </summary>
    public sealed class AirlockStatePressurizing : AirlockStateBase
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="airlock"></param>
        public AirlockStatePressurizing(Airlock airlock) : base(airlock, "AirlockStatePressurizing")
        {
        }

        protected override string DescribeCore()
        {
            return "Pressurizing ...";
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
                Airlock.Pressurize();
            }

            //open internal if pressure is high enough or after 10 seconds
            if (Airlock.IsPressurized || TotalStateTime.TotalMilliseconds > 10000)
            {
                //got to new state
                return new AirlockStateInternalOpen(Airlock);
            }

            //state continues
            return this;
        }

        /// <summary>
        /// Enter state logic
        /// </summary>
        protected override void EnterStateCore()
        {
            Airlock.CloseExternal();
        }

        /// <summary>
        /// Exit state logic
        /// </summary>
        protected override void ExitStateCore()
        {
            //open door
            Airlock.OpenInternal();
        }

        /**End copy here**/
    }
}
