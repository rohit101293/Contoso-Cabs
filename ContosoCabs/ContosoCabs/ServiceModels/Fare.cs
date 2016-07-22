using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.Models
{
    public class Fare
    {
        [JsonProperty("baseFare")]
        public string BaseFare { get; set; }
        [JsonProperty("costPerKilometer")]
        public string CostPerKilometer { get; set; }
        [JsonProperty("costPerMinute")]
        public string CostPerMinute { get; set; }
        [JsonProperty("surcharge")]
        public string Surge { get; set; }

    }
}
