using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ContosoCabs.ServiceModels
{
    public class DriverDetails
    {
        [JsonProperty("phone_number")]
        public string Phone_Number { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("picture_url")]
        public string Picture_Url { get; set; }
        
    }
}
