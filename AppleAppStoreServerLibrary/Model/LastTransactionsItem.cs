using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleAppStoreServerLibrary.Model
{
    public class LastTransactionsItem
    {
        public string originalTransactionId { get; set; }
        public int status { get; set; }
        public string signedRenewalInfo { get; set; }
        public string signedTransactionInfo { get; set; }
    }
}
