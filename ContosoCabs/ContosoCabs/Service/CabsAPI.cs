using ContosoCabs.Models;
using ContosoCabs.ResponseModels.Auth;
using ContosoCabs.ResponseModels.Private;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.Service
{
    public class CabsAPI
    {
        public async Task<SignupResponse> RegisterUser(string name, string email, string mobile, string password)
        {
            CabsClient client = new CabsClient(NetworkConstants.SIGNUP_PATH_NAME);
            client.AddParameter("name", name);
            client.AddParameter("email", email);
            client.AddParameter("mobile", mobile);
            client.AddParameter("password", password);
            string response = await client.GetResponse();
            System.Diagnostics.Debug.WriteLine(response);
            return JsonConvert.DeserializeObject<SignupResponse>(response);
        }
        public async Task<CabsResponse> GetNearbyCabs(string latitude, string longitude, string token)
        {
            CabsClient client = new CabsClient(NetworkConstants.NEARBY_PATH_NAME);
            client.AddParameter("pickUpLat", latitude);
            client.AddParameter("pickUpLong", longitude);
            client.AddParameter("token", token);
            string response = await client.GetResponse();
            System.Diagnostics.Debug.WriteLine(response);
            return JsonConvert.DeserializeObject<CabsResponse>(response);
        }
        public async Task<BookingDetailsResponse> BookCab(string token,string start_latitude, string start_longitude)
        {
            CabsClient client = new CabsClient(NetworkConstants.BOOKINGDETAILS_PATH_NAME);
            client.AddParameter("token", token);
            client.AddParameter("start_latitude", start_latitude);
            client.AddParameter("start_longitude", start_longitude);
            //client.AddParameter("is_real", "true");
            string response = await client.GetResponse();
            System.Diagnostics.Debug.WriteLine(response);
            System.Diagnostics.Debug.WriteLine(start_latitude);
            System.Diagnostics.Debug.WriteLine(start_longitude);
            return JsonConvert.DeserializeObject<BookingDetailsResponse>(response);
        }
        public async Task<PriceEstimateResponse> GetEstimate(string token, string slatitude,string slongitude, string dlatitude, string dlongitude)
        {
            CabsClient client = new CabsClient(NetworkConstants.ESTIMATE_PATH_NAME);
            client.AddParameter("pickUpLat", slatitude);
            client.AddParameter("pickUpLong", slongitude);
            client.AddParameter("dropLat", dlatitude);
            client.AddParameter("dropLong", dlongitude);
            client.AddParameter("token", token);
            string response = await client.GetResponse();
            System.Diagnostics.Debug.WriteLine("Get Estimate :" + response);
            return JsonConvert.DeserializeObject<PriceEstimateResponse>(response);

        }
        public async Task<SignInResponse> LoginUser(string mobile, string password)
        {
            CabsClient client = new CabsClient(NetworkConstants.SIGNIN_PATH_NAME);
            client.AddParameter("mobile", mobile);
            client.AddParameter("password", password);
            string response = await client.GetResponse();
            return JsonConvert.DeserializeObject<SignInResponse>(response);
        }
        public async Task<OtpResponse> GetOTP(string mobile)
        {
            CabsClient client = new CabsClient(NetworkConstants.FORGOT_PATH_NAME);
            client.AddParameter("mobile", mobile);
            string response = await client.GetResponse();
            return JsonConvert.DeserializeObject<OtpResponse>(response);
        }
        public async Task<OtpResponse> ResetPassword(string mobile, string newPassword)
        {
            CabsClient client = new CabsClient(NetworkConstants.FORGOT_PATH_NAME);
            client.AddParameter("mobile", mobile);
            client.AddParameter("newPassword", newPassword);
            string response = await client.GetResponse();
            return JsonConvert.DeserializeObject<OtpResponse>(response);
        }
        public async Task<SearchResponse> GetSuggestions(string input, string token, string latLong = "")
        {
            CabsClient client = new CabsClient(NetworkConstants.SEARCH_PATH_NAME);
            client.AddParameter("input", input);
            client.AddParameter("token", token);
            if (!latLong.Equals(""))
            {
                client.AddParameter("area", latLong);
            }
            string response = await client.GetResponse();
            System.Diagnostics.Debug.WriteLine("The server response is " + response);
            return JsonConvert.DeserializeObject<SearchResponse>(response);
        }
        public async Task<GeoResponse> GeoCodingResult(string token, string location)
        {
            CabsClient client = new CabsClient(NetworkConstants.GEOCODE_PATH_NAME);
            client.AddParameter("address", location);
            client.AddParameter("token", token);
            string response = await client.GetResponse();
            System.Diagnostics.Debug.WriteLine("The server response : " + response);
            return JsonConvert.DeserializeObject<GeoResponse>(response);
        }
        public async Task<ReverseGeoResposne> GetReverseCodingResult(string token, string place_id)
        {
            CabsClient client = new CabsClient(NetworkConstants.REGEOCODEPLACE_PATH_NAME);
            client.AddParameter("token", token);
            client.AddParameter("place_id", place_id);
            string response = await client.GetResponse();
            return JsonConvert.DeserializeObject<ReverseGeoResposne>(response);
        }
        public async Task<ReverseGeoResposne> GetReverseCodingResultlatlng(string token, string latlng)
        {
            CabsClient client = new CabsClient(NetworkConstants.REGEOCODELATLNG_PATH_NAME);
            client.AddParameter("token", token);
            client.AddParameter("latlng", latlng);
            string response = await client.GetResponse();
            System.Diagnostics.Debug.WriteLine(response);
            return JsonConvert.DeserializeObject<ReverseGeoResposne>(response);
        }
        public async Task<UserResponse> GetProfile(string token)
        {
            CabsClient client = new CabsClient(NetworkConstants.ACCOUNT_PATH_NAME);
            client.AddParameter("token", token);
            string response = await client.GetResponse();
            return JsonConvert.DeserializeObject<UserResponse>(response);
        }
        public async Task<PlacesResponse> GetPlaceLocation(string name, string token)
        {
            CabsClient client = new CabsClient(NetworkConstants.PLACES_PATH_NAME);
            client.AddParameter("token", token);
            client.AddParameter("name", name);
            string response = await client.GetResponse();
            System.Diagnostics.Debug.WriteLine("PLace Responese "  + response);
            return JsonConvert.DeserializeObject<PlacesResponse>(response);
        }
        public async Task<SendTokenResponse> SendToken(string token, string code)
        {
            CabsClient client = new CabsClient(NetworkConstants.OATUH_PATH_NAME);
            client.AddParameter("code", code);
            client.AddParameter("token", token);
            string response = await client.GetResponse();
            System.Diagnostics.Debug.WriteLine(response);
            return JsonConvert.DeserializeObject<SendTokenResponse>(response);
        }

    }
}
