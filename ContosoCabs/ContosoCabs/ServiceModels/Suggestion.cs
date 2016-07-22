using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.ServiceModels
{
    public class Suggestion
    {
        [JsonProperty("description")]
        public string Text { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
