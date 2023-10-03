using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleAppStoreServerLibrary.Model
{
    public enum Status
    {
        ACTIVE,
        EXPIRED,
        BILLING_RETRY,
        BILLING_GRACE_PERIOD,
        REVOKED
    }
}
