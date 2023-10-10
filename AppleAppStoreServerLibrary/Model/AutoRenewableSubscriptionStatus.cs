using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleAppStoreServerLibrary.Model
{
    public enum AutoRenewableSubscriptionStatus
    {
        ACTIVE,
        EXPIRED,
        BILLING_RETRY_PERIOD,
        BILLING_GRACE_PERIOD,
        REVOKED
    }
}
