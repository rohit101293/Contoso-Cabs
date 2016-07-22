using ContosoCabs.ServiceModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.ResponseModels.Private
{
    public class GeoResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("position")]
        public Position Position { get; set; }
    }
}
