using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace ContosoCabs.Essentials
{
    public class RouteClient
    {
        //private string mSource, mDestination;
        public RouteClient()
        {
        }
        public async Task<List<BasicGeoposition>> route(string mSource, string mDestination)
        {
            var coordinates = new List<BasicGeoposition>();
            var client = new HttpClient();
            string url = "https://maps.googleapis.com/maps/api/directions/json?origin=" + mSource + "&destination=" + mDestination + "&key=AIzaSyBhlUdNlPVoQ926MmBsAIIPI0v96ENgW_Q";
            HttpResponseMessage res;
            res = await client.GetAsync(url);
            var result = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<RootObject>(result);
            foreach (var route in data.routes)
            {
                if (route != null)
                {
                    foreach (var leg in route.legs)
                    {
                        if (leg != null)
                        {
                            foreach (var step in leg.steps)
                            {
                                if (step != null)
                                {
                                    var slat = step.start_location.lat;
                                    var slng = step.start_location.lng;
                                    coordinates.Add(new BasicGeoposition() { Latitude = slat, Longitude = slng });
                                }
                            }
                        }
                    }
                }
            }
            return coordinates;
        }
    }
}
