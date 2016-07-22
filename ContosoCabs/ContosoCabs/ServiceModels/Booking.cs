using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.ServiceModels
{
    public class Booking
    {
        [JsonProperty("driver")]
        public DriverDetails DriverDetails;
        [JsonProperty("vehicle")]
        public VehicleDetails VehicleDetails;
    }
}
