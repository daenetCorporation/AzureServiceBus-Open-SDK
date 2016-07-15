using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IotHub.OpenSdk.Core
{
    public enum MessageState
    {
        // Summary:
        //     Specifies an active message state.
        Active = 0,
        //
        // Summary:
        //     Specifies a deferred message state.
        Deferred = 1,
        //
        // Summary:
        //     Specifies the scheduled message state.
        Scheduled = 2,
    }
}
