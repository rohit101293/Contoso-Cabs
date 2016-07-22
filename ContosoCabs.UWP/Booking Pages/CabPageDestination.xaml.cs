using ContosoCabs.Essentials;
using ContosoCabs.Models;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Service;
using ContosoCabs.Utils;
using ContosoCabs.UWP.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCabs.UWP.Home
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CabPageDestination : Page
    {
        private string mSource;
        private string mDestination;
        private string Token;
        public CabPageDestination()
        {
            this.InitializeComponent();
            var storage = Windows.Storage.ApplicationData.Current.LocalSettings;
            Token = storage.Values["Token"].ToString();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.Frame.GoBack();
            }
        }
        private void AddMapIcon(double latitude, double longitude, string title)
        {
            MapIcon MapIcon1 = new MapIcon();
            MapIcon1.Location = new Geopoint(new BasicGeoposition()
            {
                Latitude = latitude,
                Longitude = longitude
            });
            MapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            MapIcon1.Title = title;
            MyMap.MapElements.Add(MapIcon1);
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                progress.IsActive = true;
                MyMap.MapServiceToken = "YP9WTSzOHUo0dbahsH8J~J-K6u01dT98SF4uCCKpiwA~AnGaYM6yJxoLF1tGHEIXHFskGfwSRJTr1S5aO1dB-TCXjQ1ZX0xEWgeYslbC3Fov";
                Geolocator locator = new Geolocator();
                locator.DesiredAccuracyInMeters = 50;
                var position = await locator.GetGeopositionAsync();
                await MyMap.TrySetViewAsync(position.Coordinate.Point, 17D);
                progress.IsActive = false;
                mySlider.Value = 13;
                Arguments args = e.Parameter as Arguments;
                Dictionary<string, object> Parameters = args.Values;
                if (args.GetType(VoiceCommandType.NO_VOICE))
                {
                    mSource = Parameters["source"].ToString();
                    mDestination = Parameters["destination"].ToString();
                    CabsAPI api = new CabsAPI();
                    GeoResponse sResponse = await api.GeoCodingResult(Token, mSource);
                    GeoResponse dResponse = await api.GeoCodingResult(Token, mDestination);
                    if (sResponse.Code == ResponseCode.SUCCESS && dResponse.Code == ResponseCode.SUCCESS)
                    {
                        AddMapIcon(Convert.ToDouble(sResponse.Position.Latitude), Convert.ToDouble(sResponse.Position.Longitude), mSource);
                        AddMapIcon(Convert.ToDouble(dResponse.Position.Latitude), Convert.ToDouble(dResponse.Position.Longitude), mDestination);
                    }
                    else
                    {
                        await new MessageDialog("Error retrieving Geopositions!").ShowAsync();
                        return;
                    }
                    MapPolyline polyline = new MapPolyline();
                    var coordinates = new List<BasicGeoposition>();
                    var routeClient = new RouteClient();
                    coordinates = await routeClient.route(mSource, mDestination);
                    polyline.StrokeColor = Windows.UI.Color.FromArgb(128, 255, 0, 0);
                    polyline.StrokeThickness = 5;
                    polyline.Path = new Geopath(coordinates);
                    MyMap.MapElements.Add(polyline);
                    SrcBox.Text = mSource;
                    DestBox.Text = mDestination;
                    PriceEstimateResponse pResponse = await api.GetEstimate(Token, Parameters["slat"].ToString(), Parameters["slng"].ToString(), Parameters["dlat"].ToString(), Parameters["dlng"].ToString());
                    if (pResponse.Code == ResponseCode.SUCCESS)
                    {
                        CabsListView.ItemsSource = pResponse.Estimates;
                        // ReadSpeech(new MediaElement(), "Showing Estimated cab fare from" + mSource + "to" + mDestination);
                    }
                }
            }
        }
        private string SemanticInterpretation(string v, SpeechRecognitionResult speechRecognition)
        {
            return speechRecognition.SemanticInterpretation.Properties[v].FirstOrDefault();
        }
        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

            if (MyMap != null)
                MyMap.ZoomLevel = e.NewValue;
        }
        private void CabsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Cab cabDetails = e.ClickedItem as Cab;
            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            Parameters.Add("eta", cabDetails.Eta);
            Parameters.Add("price", cabDetails.Provider);
            Parameters.Add("capacity", cabDetails.Capacity);
            Parameters.Add("time", cabDetails.Capacity);
            Parameters.Add("source", SrcBox.Text);
            Parameters.Add("destination", DestBox.Text);
            Arguments data = new Arguments(null);
            data.AddType(VoiceCommandType.NO_VOICE, true);
            data.Values = Parameters;
            Frame.Navigate(typeof(BookingPage), data);
        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
}

