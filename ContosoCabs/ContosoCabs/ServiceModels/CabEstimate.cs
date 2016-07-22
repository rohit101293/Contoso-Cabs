using ContosoCabs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.ServiceModels
{
    public class CabEstimate
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
        [JsonProperty("estimate")]
        public PriceEstimate CurrentEstimate { get; set; }
    }
}
