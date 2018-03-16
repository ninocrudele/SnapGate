//------------------------------------------------------------------------------
// Author: Daniel Vasquez-Lopez 2009
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Samples.Eventing
{
    [Serializable]
    public sealed class PropertyBag : SortedList<string, object>
    {
        internal PropertyBag()
            : base(StringComparer.Ordinal)
        {
        }

        internal PropertyBag(int capacity)
            : base(capacity, StringComparer.Ordinal)
        {
        }
    }
}
