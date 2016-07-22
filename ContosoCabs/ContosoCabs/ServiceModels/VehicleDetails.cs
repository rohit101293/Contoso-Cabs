using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.ServiceModels
{
    public class VehicleDetails
    {
        [JsonProperty("make")]
        public string Make { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("license_plate")]
        public string License_Plate { get; set; }


    }
}
