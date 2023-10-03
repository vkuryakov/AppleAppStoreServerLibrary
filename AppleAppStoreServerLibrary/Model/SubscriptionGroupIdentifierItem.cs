using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleAppStoreServerLibrary.Model
{
    public class SubscriptionGroupIdentifierItem
    {
        public string subscriptionGroupIdentifier { get; set; }
        public LastTransactionsItem[] lastTransactions { get; set; }
    }
}
