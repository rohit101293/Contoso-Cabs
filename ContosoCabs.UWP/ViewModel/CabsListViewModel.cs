using ContosoCabs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoCabs.Service;
using ContosoCabs.ServiceModels;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Utils;
using Windows.UI.Popups;
using Windows.Networking.Connectivity;

namespace ContosoCabs.UWP.ViewModel
{
    public class CabsListViewModel
    {
        public List<Cab> Cabs;
        private string _startLat;
        private string _startLng;
        private string _endLat;
        private string _endLng;
        private string _token;
        private CabsAPI _cabsApi;
        public const int REFRESH_SURGE = 10;
        public const int REFRESH_ESTIMATE = 15;
        public CabsListViewModel(string startLng, string startLat,string token)
        {
            _startLng = startLng;
            _startLat = startLat;
            _token = token;
            _cabsApi = new CabsAPI();
        }
        public void SetLatLng(string endLat, string endLng)
        {
            _endLat = endLat;
            _endLng = endLng;
        }
        public async Task RefreshView(int refreshType)
        {
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                List<Cab> refreshed = new List<Cab>();
                if (refreshType == REFRESH_SURGE)
                {
                    CabsResponse res = await _cabsApi.GetNearbyCabs(_startLat, _startLng, _token);
                    if (res.Code == ResponseCode.SUCCESS)
                    {
                        refreshed = res.Cabs;
                    }
                }
                else if (refreshType == REFRESH_ESTIMATE)
                {
                    PriceEstimateResponse res = await _cabsApi.GetEstimate(_token, _startLat, _startLng,
                        _endLat, _endLng);
                    if (res.Code == ResponseCode.SUCCESS)
                    {
                        foreach (var estimate in res.Estimates)
                        {
                            Cab cab = new Cab();
                            cab.Provider = estimate.Provider;
                            cab.Eta = estimate.Eta;
                            cab.ImageURL = estimate.ImageURL;
                            cab.Type = estimate.Type;
                            cab.Time = estimate.CurrentEstimate.Time;
                            cab.Distance = estimate.CurrentEstimate.Distance;
                            cab.FareData = estimate.CurrentEstimate.FareData;
                            cab.FareData.Surge = estimate.CurrentEstimate.LowRange + "-" +
                                estimate.CurrentEstimate.HighRange;
                            refreshed.Add(cab);
                        }
                    }
                }
                Cabs = refreshed;
            }
               
        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
}
