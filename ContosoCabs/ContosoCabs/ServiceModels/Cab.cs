using Java.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.Models
{
    public class Cab
    {
        [JsonProperty("provider")]
        public string Provider { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("eta")]
        public string Eta { get; set; }
        [JsonProperty("capacity")]
        public int Capacity { get; set; }
        [JsonProperty("distance")]
        public string Distance { get; set; }
        [JsonProperty("image")]
        public string ImageURL { get; set; }
        [JsonProperty("fare")]
        public Fare FareData { get; set; }
        [JsonIgnore]
        public string Time { get; set; }
    }
}
