using AppleAppStoreServerLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace AppleAppStoreServerLibrary.Client
{
    public class AppStoreServerAPIClient
    {
        private static readonly string PRODUCTION_URL = "https://api.storekit.itunes.apple.com";
        private static readonly string SANDBOX_URL = "https://api.storekit-sandbox.itunes.apple.com";
        private static readonly string USER_AGENT = "dot-net-app-store-server-library/0.0.1";
        private static readonly string JSON_MEDIA_TYPE = "application/json; charset=utf-8";

        private readonly HttpClient _httpClient;
        private readonly BearerTokenAuthenticator _bearerTokenAuthenticator;
        private readonly Uri _urlBase;
        //private readonly Gson gson;

        public AppStoreServerAPIClient(string signingKey, string keyId, string issuerId, string bundleId, AppleEnvironment environment)
        {
            _bearerTokenAuthenticator = new BearerTokenAuthenticator(signingKey, keyId, issuerId, bundleId);
            _urlBase = environment == AppleEnvironment.PRODUCTION ? new Uri(PRODUCTION_URL) : new Uri(SANDBOX_URL);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _urlBase;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _bearerTokenAuthenticator.GenerateToken());
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        private HttpRequestMessage BuildRequest(string path, HttpMethod method, Dictionary<string, List<string>> queryParameters, object jsonBody = null)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, path);
            if (queryParameters != null && queryParameters.Count > 0)
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                foreach (var plist in queryParameters)
                {
                    foreach (var p in plist.Value)
                    {
                        query.Add(plist.Key, p);
                    }
                }
                path += "?" + query.ToString();
            }
            if (jsonBody != null && method == HttpMethod.Post)
            {
                string body = JsonSerializer.Serialize(jsonBody);
                httpRequestMessage.Content = new StringContent(body, Encoding.UTF8, JSON_MEDIA_TYPE);
            }
            return httpRequestMessage;
        }

        private async Task<T> MakeHttpCallAsync<T>(string path, HttpMethod method, Dictionary<string, List<string>> queryParameters = null, object body = null)
        {
            var request = BuildRequest(path, method, queryParameters, body);
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(responseContent))
                {
                    throw new Exception("Response code was 2xx but no body returned");
                }
                return JsonSerializer.Deserialize<T>(responseContent);
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        ErrorPayload errorPayload = JsonSerializer.Deserialize<ErrorPayload>(responseContent);
                        if (Enum.IsDefined(typeof(APIError), errorPayload.ErrorCode))
                        {
                            var apiError = (APIError)errorPayload.ErrorCode;
                            throw new APIException(response.StatusCode, apiError);
                        }
                    }
                }
                catch(Exception ex)
                {
                    APIException e = new APIException(response.StatusCode, inner: ex);
                    throw e;
                }
                throw new APIException(response.StatusCode);
            }
        }

        /**
         * Get the statuses for all of a customer’s auto-renewable subscriptions in your app.
         *
         * @param transactionId The identifier of a transaction that belongs to the customer, and which may be an original transaction identifier.
         * @param status An optional filter that indicates the status of subscriptions to include in the response. Your query may specify more than one status query parameter.
         * @return A response that contains status information for all of a customer’s auto-renewable subscriptions in your app.
         * @throws APIException If a response was returned indicating the request could not be processed
         * @throws IOException  If an exception was thrown while making the request
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/get_all_subscription_statuses">Get All Subscription Statuses</a>
         */
        public async Task<StatusResponse> GetAllSubscriptionStatusesAsync(string transactionId, Status[] status = null){
            Dictionary<string, List<string>> queryParameters = new Dictionary<string, List<string>>();
            if (status != null) {
                List<string> statusList = new List<string>(status.Length);
                foreach(Status statusItem in status)
                {
                    statusList.Add(statusItem.ToString());
                }
                queryParameters.Add("status", statusList);
            }
            return await MakeHttpCallAsync<StatusResponse>("/inApps/v1/subscriptions/" + transactionId, HttpMethod.Get, queryParameters);
        }
    }
}
