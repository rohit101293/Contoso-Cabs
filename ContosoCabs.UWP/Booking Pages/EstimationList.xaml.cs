using ContosoCabs.Essentials;
using ContosoCabs.Models;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Service;
using ContosoCabs.ServiceModels;
using ContosoCabs.Utils;
using ContosoCabs.UWP.Home;
using ContosoCabs.UWP.Speech;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCabs.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EstimationList : Page
    {
        private string mSource, mDestination;
        private SpeechRecognitionResult speechRecognition;
        public string Token;
        public EstimationList()
        {
            this.InitializeComponent();
            var storage = Windows.Storage.ApplicationData.Current.LocalSettings;
            Token = storage.Values["Token"].ToString();
        }
        private void AddMapIcon(double latitude ,double longitude,string title)
        {
            MapIcon MapIcon1 = new MapIcon();
            MapIcon1.Location = new Geopoint(new BasicGeoposition()
            {
                Latitude =latitude,
                Longitude =longitude
            });
            MapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            MapIcon1.Title = title;
            MyMap.MapElements.Add(MapIcon1);

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.Frame.GoBack();
            }
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
                speechRecognition = e.Parameter as SpeechRecognitionResult;
                mSource = this.SemanticInterpretation("source", speechRecognition);
                mDestination = this.SemanticInterpretation("destination", speechRecognition);
                MyMap.MapServiceToken = "YP9WTSzOHUo0dbahsH8J~J-K6u01dT98SF4uCCKpiwA~AnGaYM6yJxoLF1tGHEIXHFskGfwSRJTr1S5aO1dB-TCXjQ1ZX0xEWgeYslbC3Fov";
                Geolocator locator = new Geolocator();
                locator.DesiredAccuracyInMeters = 50;
                var position = await locator.GetGeopositionAsync();
                await MyMap.TrySetViewAsync(position.Coordinate.Point, 10D);
                progress.IsActive = false;
              //  mySlider.Value = 13;
                CabsAPI api = new CabsAPI();
                GeoResponse sResponse = await api.GeoCodingResult(Token, mSource);
                GeoResponse dResponse = await api.GeoCodingResult(Token, mDestination);
                if (sResponse.Code == ResponseCode.SUCCESS)
                {
                    if (dResponse.Code == ResponseCode.SUCCESS)
                    {
                        AddMapIcon(Convert.ToDouble(sResponse.Position.Latitude), Convert.ToDouble(sResponse.Position.Longitude), mSource);
                        AddMapIcon(Convert.ToDouble(dResponse.Position.Latitude), Convert.ToDouble(dResponse.Position.Longitude), mDestination);
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
                        PriceEstimateResponse pResponse = await api.GetEstimate(Token, sResponse.Position.Latitude, sResponse.Position.Longitude, dResponse.Position.Latitude, dResponse.Position.Longitude);
                        if (pResponse.Code == ResponseCode.SUCCESS)
                        {
                            CabsListView.ItemsSource = pResponse.Estimates;
                            ReadSpeech(new MediaElement(), "Showing Estimated cab fare from" + mSource + "to" + mDestination);
                        }
                        else
                        {
                            await new MessageDialog("Error in estimating cabs").ShowAsync();
                            return;
                        }
                    }
                    else
                    {
                        await new MessageDialog("Destination not found").ShowAsync();
                        return;
                    }
                }
                else
                {
                    await new MessageDialog("Source not found").ShowAsync();
                    return;
                }
            }
        }
        private string SemanticInterpretation(string v, SpeechRecognitionResult speechRecognition)
        {
            return speechRecognition.SemanticInterpretation.Properties[v].FirstOrDefault();
        }
        private void CabsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            CabEstimate cabDetails = e.ClickedItem as CabEstimate;
            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            Parameters.Add("eta", cabDetails.Eta);
            Parameters.Add("low", cabDetails.CurrentEstimate.LowRange);
            Parameters.Add("high", cabDetails.CurrentEstimate.HighRange);
            Parameters.Add("distance", cabDetails.CurrentEstimate.Distance);
            Parameters.Add("time", cabDetails.CurrentEstimate.Time);
            Parameters.Add("source", mSource);
            Parameters.Add("destination", mDestination);
            Arguments data = new Arguments(null);
            data.AddType(VoiceCommandType.ESTIMATE_FROM, true);
            data.Values = Parameters;
            Frame.Navigate(typeof(BookingPage), data);
        }
        private async void ReadSpeech(MediaElement mediaElement, string readtext)
        {
            SpeechSynthesisStream stream = await new SpeechSynthesizer().SynthesizeTextToStreamAsync(readtext);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
}