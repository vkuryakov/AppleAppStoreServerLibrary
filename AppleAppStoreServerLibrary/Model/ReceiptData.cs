using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleAppStoreServerLibrary.Model
{
    public class ReceiptData
    {
        // ASN.1 Field Type 1703 (Transaction ID)
        // ASN.1 Field Type 1705 (Origignal Transaction ID)
        public string TransactionID { get; set; }
        //ASN.1 Field Type 2
        public string BundleId { get; set; }
        //ASN.1 Field Type 3
        public string VersionId{ get; set; }
        //ASN.1 Field Type 5
        public byte[] SHA1Hash { get; set; }
        //ASN.1 Field Type 12
        public DateTime ReceiptCreationDate { get; set; }
    }
}
