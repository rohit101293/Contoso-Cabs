using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.ServiceModels
{
    public class User
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("mobile")]
        public string Mobile { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }
    }
}
