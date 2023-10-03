using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppleAppStoreServerLibrary.Client
{
    public class APIException : Exception
    {

        public HttpStatusCode HttpStatusCode { get; private set; }
        public APIError? APIError { get; private set; }

        public APIException(HttpStatusCode httpStatusCode, APIError? apiError = null, Exception inner = null)
            : base(string.Empty, inner)
        {
            HttpStatusCode = httpStatusCode;
            APIError = apiError;
        }

        public override string ToString()
        {
            return string.Format("APIException(httpStatusCode={0}, apiError={1}) {2}", HttpStatusCode, APIError, base.ToString());
        }

        
    }
}
