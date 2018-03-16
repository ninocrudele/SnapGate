namespace GrabCasterTrigger.Trigger
{
    using GrabCaster.Framework.Contracts.Attributes;
    using GrabCaster.Framework.Contracts.Globals;
    using GrabCaster.Framework.Contracts.Triggers;
    //<USING>
    /// <summary>
    /// The nop trigger.
    /// </summary>
    [TriggerContract("{*ID*}", "*NAME*", "*DESCRIPTION*", false, true, false)]
    public class GrabCasterTrigger : ITriggerType
    {
        //<PROPERTIES>
        [TriggerPropertyContract("Syncronous", "Define if the action between the trigger and the remote event needs to be syncronous")]
        public bool Syncronous { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        /// Gets or sets the set event action trigger.
        /// </summary>
        public ActionTrigger ActionTrigger { get; set; }

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        [TriggerPropertyContract("DataContext", "Main data context")]
        public byte[] DataContext { get; set; }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="actionTrigger">
        /// The set event action trigger.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        [TriggerActionContract("{*CONTRACTID*}", "Main action", "Main action executed by the trigger")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            //<MAINCODE>

            actionTrigger(this, context);
            return null;
        }
        //<FUNCTIONS>
    }
}