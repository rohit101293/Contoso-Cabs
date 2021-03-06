﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ContosoCabs.ServiceModels;

namespace ContosoCabs.ResponseModels.Private
{
     public class BookingDetailsResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("data")]
        public Booking BookingData { get; set; }
    }
}
