using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.Service
{
    public class CabsClient
    {
        public Dictionary<string, string> Parameters { get; set; }
        public string EndPoint { get; set; }

        public CabsClient(string endPoint)
        {
            this.EndPoint = endPoint;
            this.Parameters = new Dictionary<string, string>();
        }
        public void AddParameter(string key, string value)
        {
            this.Parameters.Add(key, value);
        }
        public CabsClient(string endPoint, Dictionary<string, string> parameters)
        {
            this.EndPoint = endPoint;
            this.Parameters = parameters;
        }
        private Uri GetEndpointUri()
        {
            UriBuilder builder = new UriBuilder(NetworkConstants.BASE_URL);
            switch (this.EndPoint)
            {
                case NetworkConstants.SIGNIN_PATH_NAME:
                    builder.Path = NetworkConstants.SIGNIN_PATH_NAME;
                    break;
                case NetworkConstants.SIGNUP_PATH_NAME:
                    builder.Path = NetworkConstants.SIGNUP_PATH_NAME;
                    break;
                case NetworkConstants.NEARBY_PATH_NAME:
                    builder.Path = NetworkConstants.NEARBY_PATH_NAME;
                    break;
                case NetworkConstants.FORGOT_PATH_NAME:
                    builder.Path = NetworkConstants.FORGOT_PATH_NAME;
                    break;
                case NetworkConstants.SEARCH_PATH_NAME:
                    builder.Path = NetworkConstants.SEARCH_PATH_NAME;
                    break;
                case NetworkConstants.ACCOUNT_PATH_NAME:
                    builder.Path = NetworkConstants.ACCOUNT_PATH_NAME;
                    break;
                case NetworkConstants.GEOCODE_PATH_NAME:
                    builder.Path = NetworkConstants.GEOCODE_PATH_NAME;
                    break;
                case NetworkConstants.ESTIMATE_PATH_NAME:
                    builder.Path = NetworkConstants.ESTIMATE_PATH_NAME;
                    break;
                case NetworkConstants.PLACES_PATH_NAME:
                    builder.Path = NetworkConstants.PLACES_PATH_NAME;
                    break;
                case NetworkConstants.REGEOCODEPLACE_PATH_NAME:
                    builder.Path = NetworkConstants.REGEOCODEPLACE_PATH_NAME;
                    break;
                case NetworkConstants.REGEOCODELATLNG_PATH_NAME:
                    builder.Path = NetworkConstants.REGEOCODELATLNG_PATH_NAME;
                    break;
                case NetworkConstants.BOOKINGDETAILS_PATH_NAME:
                    builder.Path = NetworkConstants.BOOKINGDETAILS_PATH_NAME;
                    break;
                case NetworkConstants.OATUH_PATH_NAME:
                    builder.Path = NetworkConstants.OATUH_PATH_NAME;
                    break;
            }
            return builder.Uri;
        }
        public async Task<string> GetResponse()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, GetEndpointUri().ToString());
            System.Diagnostics.Debug.WriteLine(GetEndpointUri().ToString());
            string a = await (new FormUrlEncodedContent(this.Parameters).ReadAsStringAsync());
            System.Diagnostics.Debug.WriteLine(a);
            client.Timeout = TimeSpan.FromMilliseconds(15000);
            request.Content = new FormUrlEncodedContent(this.Parameters);
            var response = await client.SendAsync(request);
            string res = await response.Content.ReadAsStringAsync();
            return res;
        }
    }
}
