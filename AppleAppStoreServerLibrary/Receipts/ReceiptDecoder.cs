using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using System.Numerics;
using AppleAppStoreServerLibrary.Model;
using System.Security.Cryptography.X509Certificates;

namespace AppleAppStoreServerLibrary.Receipts
{
    public class ReceiptDecoder
    {
        private static readonly int BUNDLE_ID_TYPE_ID = 2;
        private static readonly int VERSION_ID_TYPE_ID = 3;
        private static readonly int SHA1_HASH_TYPE_ID = 5;
        private static readonly int RECEIPT_CREATION_DATE_TYPE_ID = 12;
        private static readonly int IN_APP_TYPE_ID = 17;
        private static readonly int TRANSACTION_IDENTIFIER_TYPE_ID = 1703;
        private static readonly int ORIGINAL_TRANSACTION_IDENTIFIER_TYPE_ID = 1705;
        private static readonly string PKCS7_VALUE = "1.2.840.113549.1.7.2";
        public readonly ReceiptData receiptData = new ReceiptData();
        // Uncomment Sections to obtain all data from the receipt (for debug purposes)
        //public Dictionary<BigInteger, string> Sections = new Dictionary<BigInteger, string>();

        public ReceiptDecoder(string appReceipt, X509Certificate2Collection x509Certificates)
        {
            byte[] data = Convert.FromBase64String(appReceipt);
            SignedCms signedCms = new SignedCms();
            
            signedCms.Decode(data);
            /** TODO: use receipt_creation_date field as described here:
             *  https://developer.apple.com/documentation/appstorereceipts/validating_receipts_on_the_device#4180978
             */
            signedCms.CheckSignature(x509Certificates, false);
            AsnReader reader = new AsnReader(signedCms.ContentInfo.Content, AsnEncodingRules.DER);
            FillReceiptData(reader);
        }

        private void FillReceiptData(AsnReader asnReader)
        {
            AsnReader helper;
            while (asnReader.HasData)
            {
                var tag = asnReader.PeekTag();
                switch ((UniversalTagNumber)tag.TagValue)
                {
                    case UniversalTagNumber.Sequence:
                        {
                            helper = asnReader.ReadSequence();
                            var type = helper.ReadInteger();
                            var version = helper.ReadInteger();
                            var octetString = helper.ReadOctetString();
                            //Sections.Add(type, Encoding.UTF8.GetString(octetString));
                            if (type == IN_APP_TYPE_ID)
                            {
                                AsnReader inappSet = new AsnReader(octetString, AsnEncodingRules.DER);
                                FillReceiptData(inappSet);
                            }
                            else if (type == TRANSACTION_IDENTIFIER_TYPE_ID || type == ORIGINAL_TRANSACTION_IDENTIFIER_TYPE_ID)
                            {
                                receiptData.TransactionID = OctetToString(octetString);
                            }
                            else if (type == BUNDLE_ID_TYPE_ID)
                            {
                                receiptData.BundleId = OctetToString(octetString);
                            }
                            else if (type == VERSION_ID_TYPE_ID)
                            {
                                receiptData.VersionId = OctetToString(octetString);
                            }
                            else if (type == SHA1_HASH_TYPE_ID)
                            {
                                receiptData.SHA1Hash = octetString;
                            }
                            else if (type == RECEIPT_CREATION_DATE_TYPE_ID)
                            {
                                DateTime dt;
                                if (DateTime.TryParse(OctetToString(octetString), out dt)) {
                                    receiptData.ReceiptCreationDate = dt;
                                }
                            }
                            break;
                        }
                    case UniversalTagNumber.Set:
                        {
                            FillReceiptData(asnReader.ReadSetOf());
                            break;
                        }
                    default:
                        {
                            asnReader.ReadEncodedValue();
                            break;
                        }
                }
            }
        }

        private string OctetToString(byte[] octetString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in octetString)
            {
                if (!char.IsControl(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
