using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ContosoCabs.ServiceModels
{
    public class Position
    {
        [JsonProperty("lat")]
        public string Latitude { get; set; }
        [JsonProperty("lng")]
        public string Longitude { get; set; }
    }
}
