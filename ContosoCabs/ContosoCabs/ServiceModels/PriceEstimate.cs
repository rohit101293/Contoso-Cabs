using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ContosoCabs.Models;

namespace ContosoCabs.ServiceModels
{
    public class PriceEstimate
    {
        [JsonProperty("lowRange")]
        public string LowRange { get; set; }
        [JsonProperty("highRange")]
        public string HighRange { get; set; }
        [JsonProperty("distance")]
        public string Distance { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("fare")]
        public Fare FareData { get; set; }
    }
}
