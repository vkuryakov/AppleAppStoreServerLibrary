using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleAppStoreServerLibrary.Model
{
    public class ErrorPayload
    {
        public long ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
