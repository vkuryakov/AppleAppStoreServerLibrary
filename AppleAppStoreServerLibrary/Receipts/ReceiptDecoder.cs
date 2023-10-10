using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.Pkcs;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using System.Numerics;
using AppleAppStoreServerLibrary.Model;
using System.Security.Cryptography.X509Certificates;

namespace AppleAppStoreServerLibrary.Receipts
{
    public class ReceiptDecoder
    {
        private const string PRODUCTION_ENVIRONMENT_VALUE = "Production";
        private const string SANDBOX_ENVIRONMENT_VALUE = "ProductionSandbox";
        private static readonly int ENVIRONMENT_TYPE_ID = 0;
        private static readonly int BUNDLE_ID_TYPE_ID = 2;
        private static readonly int VERSION_ID_TYPE_ID = 3;
        private static readonly int SHA1_HASH_TYPE_ID = 5;
        private static readonly int RECEIPT_CREATION_DATE_TYPE_ID = 12;
        private static readonly int IN_APP_TYPE_ID = 17;
        private static readonly int SUBSCRIPTION_IDENTIFIER_TYPE_ID = 1702;
        private static readonly int ORIGINAL_TRANSACTION_IDENTIFIER_TYPE_ID = 1705;
        private static readonly string PKCS7_VALUE = "1.2.840.113549.1.7.2";
        public readonly ReceiptData receiptData = new ReceiptData();
        public readonly Dictionary<int, List<string>> Errors = new Dictionary<int, List<string>>();
        // Uncomment Sections to obtain all data from the receipt (for debug purposes)
        public Dictionary<BigInteger, string> Sections = new Dictionary<BigInteger, string>();

        public ReceiptDecoder(string appReceipt, X509Certificate2Collection x509Certificates)
        {
            byte[] data = Convert.FromBase64String(appReceipt);
            SignedCms signedCms = new SignedCms();
            
            signedCms.Decode(data);
            /** TODO: use receipt_creation_date field as described here:
             *  https://developer.apple.com/documentation/appstorereceipts/validating_receipts_on_the_device#4180978
             */
            try
            {
                signedCms.CheckSignature(x509Certificates, false);
                receiptData.IsSignatureValid = true;
            }
            catch
            {

            }
            AsnReader reader = new AsnReader(signedCms.ContentInfo.Content, AsnEncodingRules.DER);
            FillReceiptData(reader);
        }

        public bool Validate(string bundleId, string versionId)
        {
            return string.Equals(receiptData.BundleId, bundleId) && string.Equals(receiptData.VersionId, versionId);
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
                            if (!Sections.ContainsKey(type))
                            {
                                Sections.Add(type, Encoding.UTF8.GetString(octetString));
                            }
                            
                            if (type == ENVIRONMENT_TYPE_ID)
                            {
                                ValidateBytes(ENVIRONMENT_TYPE_ID, octetString);
                                string envStr = OctetToString(octetString, 1);
                                switch(envStr)
                                {
                                    case PRODUCTION_ENVIRONMENT_VALUE:
                                        receiptData.Environment = AppleEnvironment.PRODUCTION;
                                        break;
                                    case SANDBOX_ENVIRONMENT_VALUE:
                                        receiptData.Environment = AppleEnvironment.SANDBOX; 
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else if (type == IN_APP_TYPE_ID)
                            {
                                AsnReader inappSet = new AsnReader(octetString, AsnEncodingRules.DER);
                                FillReceiptData(inappSet);
                            }
                            else if (type == ORIGINAL_TRANSACTION_IDENTIFIER_TYPE_ID)
                            {
                                ValidateBytes(ORIGINAL_TRANSACTION_IDENTIFIER_TYPE_ID, octetString);
                                receiptData.OriginalTransactionIDs.Add(OctetToString(octetString, 2));
                            }
                            else if (type == BUNDLE_ID_TYPE_ID)
                            {
                                ValidateBytes(BUNDLE_ID_TYPE_ID, octetString);
                                receiptData.BundleId = OctetToString(octetString, 2);
                            }
                            else if (type == VERSION_ID_TYPE_ID)
                            {
                                ValidateBytes(VERSION_ID_TYPE_ID, octetString);
                                receiptData.VersionId = OctetToString(octetString, 2);
                            }
                            else if (type == SHA1_HASH_TYPE_ID)
                            {
                                receiptData.SHA1Hash = octetString;
                            }
                            else if (type == RECEIPT_CREATION_DATE_TYPE_ID)
                            {
                                ValidateBytes(RECEIPT_CREATION_DATE_TYPE_ID, octetString);
                                DateTime dt;
                                if (DateTime.TryParse(OctetToString(octetString, 2), out dt)) {
                                    receiptData.ReceiptCreationDate = dt;
                                }
                            }
                            else if (type == SUBSCRIPTION_IDENTIFIER_TYPE_ID)
                            {
                                ValidateBytes(SUBSCRIPTION_IDENTIFIER_TYPE_ID, octetString);
                                receiptData.ProductId = OctetToString(octetString, 2);
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

        private string OctetToString(byte[] octetString, int skip)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = skip; i < octetString.Length; i++)
            {
                sb.Append((char)octetString[i]);
            }
            return sb.ToString();
        }

        private void ValidateBytes(int type, byte[] bytes)
        {

            if (!char.IsControl((char)bytes[0]))
            {
                AddError(type, string.Format("The first byte is not expected: {0}", (char)bytes[0]));
            }

            // ENVIRONMENT_TYPE_ID has only 1 control byte
            if (type == ENVIRONMENT_TYPE_ID) return;

            if (type == SUBSCRIPTION_IDENTIFIER_TYPE_ID)
            {
                if (char.IsLetterOrDigit((char)bytes[1]) || (char)bytes[1] == '_' || (char)bytes[1] == '.')
                {
                    AddError(type, string.Format("The second byte is not expected: {0}", (char)bytes[1]));
                }
            }
            else if (type == BUNDLE_ID_TYPE_ID)
            {
                if (char.IsLetterOrDigit((char)bytes[1]) || (char)bytes[1] == '-' || (char)bytes[1] == '.')
                {
                    AddError(type, string.Format("The second byte is not expected: {0}", (char)bytes[1]));
                }
            }
            else if (type == VERSION_ID_TYPE_ID || type == RECEIPT_CREATION_DATE_TYPE_ID || type == ORIGINAL_TRANSACTION_IDENTIFIER_TYPE_ID)
            {
                if (char.IsDigit((char)bytes[1]))
                {
                    AddError(type, string.Format("The second byte is not expected: {0}", (char)bytes[1]));
                }
            }
            else if(!char.IsControl((char)bytes[1]))
            {
                AddError(type, string.Format("The second byte is not expected: {0}", (char)bytes[1]));
            }
        }

        private void AddError(int type,  string message)
        {
            if (!Errors.ContainsKey(type))
            {
                Errors.Add(type, new List<string>());
            }
            Errors[type].Add(message);
        }
    }
}
