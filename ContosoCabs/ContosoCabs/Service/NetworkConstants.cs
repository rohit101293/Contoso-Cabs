using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.Service
{
    public class NetworkConstants
    {
        public const string BASE_URL = "http://contosocabs.azurewebsites.net/";
        public const string SIGNIN_PATH_NAME = "api/v1/signin";
        public const string SIGNUP_PATH_NAME = "api/v1/signup";
        public const string NEARBY_PATH_NAME = "api/v1/private/nearby";
        public const string SEARCH_PATH_NAME = "api/v1/private/search";
        public const string FORGOT_PATH_NAME = "api/v1/forgot";
        public const string TERMS_PATH_NAME = "tc";
        public const string GEOCODE_PATH_NAME = "api/v1/private/geocode";
        public const string ACCOUNT_PATH_NAME = "api/v1/private/profile";
        public const string ESTIMATE_PATH_NAME = "api/v1/private/estimate";
        public const string OATUH_PATH_NAME = "api/v1/private/getoauth";
        public const string PLACES_PATH_NAME = "api/v1/private/places";
        public const string REGEOCODEPLACE_PATH_NAME = "api/v1/private/reverseGeocode";
        public const string REGEOCODELATLNG_PATH_NAME = "api/v1/private/reverseGeocodeLatlng";
        public const string BOOKINGDETAILS_PATH_NAME = "api/v1/private/book";
    }
}
