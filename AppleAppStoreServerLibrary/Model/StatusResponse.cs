using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleAppStoreServerLibrary.Model
{
    public class StatusResponse
    {
        public string environment { get; set; }
        public string bundleId { get; set; }
        public long appAppleId { get; set; }
        public SubscriptionGroupIdentifierItem[] data { get; set; }
    }
}
