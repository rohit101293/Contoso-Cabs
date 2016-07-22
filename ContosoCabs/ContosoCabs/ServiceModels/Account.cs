using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.ServiceModels
{
    public class Account
    {
        [JsonProperty("account")]
        public string Provider { get; set; }
        [JsonProperty("oauth_token")]
        public string Token { get; set; }
    }
}
