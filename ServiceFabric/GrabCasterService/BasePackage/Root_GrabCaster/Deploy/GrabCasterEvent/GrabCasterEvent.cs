namespace GrabCasterEvent.Event
{
    using GrabCaster.Framework.Contracts.Attributes;
    using GrabCaster.Framework.Contracts.Events;
    using GrabCaster.Framework.Contracts.Globals;
    //<USING>

    /// <summary>
    /// The no operation event.
    /// </summary>
    [EventContract("{*ID*}", "*NAME*", "*DESCRIPTION*", true)]
    public class GrabCasterEvent : IEventType
    {
        //<PROPERTIES>
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        /// Gets or sets the set event action event.
        /// </summary>
        public ActionEvent ActionEvent { get; set; }


        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        [EventPropertyContract("DataContext", "Main data context")]
        public byte[] DataContext { get; set; }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="actionEvent">
        /// The set event action event.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        [EventActionContract("{*CONTRACTID*}", "Main action", "Main action executed by the event")]
        public byte[] Execute(ActionEvent actionEvent, ActionContext context)
        {
            //<MAINCODE>

            actionEvent(this, context);
            return null;
        }
        //<FUNCTIONS>
    }
}