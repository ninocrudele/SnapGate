namespace GrabCaster.Framework.BTSTransformComponent
{
    using GrabCaster.Framework.Contracts.Components;
    using GrabCaster.Framework.Contracts.Attributes;
    using GrabCaster.Framework.Base;
    //<USING>

    [ComponentContract("{*ID*}", "*NAME*", "*DESCRIPTION*")]
    public class GrabCasterComponent : IChainComponentType
    {
        //<PROPERTIES>

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        [ComponentPropertyContract("DataContext", "Main data context")]
        public byte[] DataContext { get; set; }
        [ComponentActionContract("{*CONTRACTID*}", "Main action", "Main action executed by the component")]
        public byte[] Execute()
        {
            //<MAINCODE>
            return EncodingDecoding.EncodingString2Bytes("result");

        }
        //<FUNCTIONS>
    }
}
