using ContosoCabs.Essentials;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Service;
using ContosoCabs.ServiceModels;
using ContosoCabs.Utils;
using ContosoCabs.UWP.Speech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

    public sealed partial class BookingPage : Page
    {
        public BookingPage()
        {
            this.InitializeComponent();
        }
        private void AddMapIcon(double latitude, double longitude,string title)
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
       private async void Addroute(GeoResponse sResponse,GeoResponse dResponse,string source, string destination)
        {
            AddMapIcon(Convert.ToDouble(sResponse.Position.Latitude), Convert.ToDouble(sResponse.Position.Longitude), source);
            AddMapIcon(Convert.ToDouble(dResponse.Position.Latitude), Convert.ToDouble(dResponse.Position.Longitude), destination);
            MapPolyline polyline = new MapPolyline();
            var coordinates = new List<BasicGeoposition>();
            var routeClient = new RouteClient();
            coordinates = await routeClient.route(source, destination);
            //polyline.StrokeColor = Windows.UI.Color.FromArgb(128, 255, 0, 0);
            //polyline.StrokeThickness = 5;
            //polyline.Path = new Geopath(coordinates);
            //MyMap.MapElements.Add(polyline);
        }
        private async void Addroute(PlacesResponse sResponse, PlacesResponse dResponse, string source, string destination)
        {
            AddMapIcon(Convert.ToDouble(sResponse.Location.Latitude), Convert.ToDouble(sResponse.Location.Longitude), source);
            AddMapIcon(Convert.ToDouble(dResponse.Location.Latitude), Convert.ToDouble(dResponse.Location.Longitude), destination);
            MapPolyline polyline = new MapPolyline();
            var coordinates = new List<BasicGeoposition>();
            var routeClient = new RouteClient();
            string sourceLoc = sResponse.Location.Latitude + "," + sResponse.Location.Longitude;
            string destinationLoc = dResponse.Location.Latitude + "," + dResponse.Location.Longitude;
            coordinates = await routeClient.route(sourceLoc, destinationLoc);
            polyline.StrokeColor = Windows.UI.Color.FromArgb(128, 255, 0, 0);
            polyline.StrokeThickness = 5;
            polyline.Path = new Geopath(coordinates);
            MyMap.MapElements.Add(polyline);
        }
        private void ShowLoader(bool show)
        {
            if (show)
            {
                Loader.Visibility = Visibility.Visible;
                TB_Loader.Visibility = Visibility.Visible;
            }
            else
            {
                Loader.Visibility = Visibility.Collapsed;
                TB_Loader.Visibility = Visibility.Collapsed;
            }

        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ShowLoader(true);
            {
                if (!IsInternet())
                {
                    await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                    return;
                }
                else
                {
                    MyMap.MapServiceToken = "YP9WTSzOHUo0dbahsH8J~J-K6u01dT98SF4uCCKpiwA~AnGaYM6yJxoLF1tGHEIXHFskGfwSRJTr1S5aO1dB-TCXjQ1ZX0xEWgeYslbC3Fov";
                    Geolocator locator = new Geolocator();
                    locator.DesiredAccuracyInMeters = 50;
                    var position = await locator.GetGeopositionAsync();
                    await MyMap.TrySetViewAsync(position.Coordinate.Point, 10D);

                    //mySlider.Value = 13;
                    Arguments args = e.Parameter as Arguments;
                    if (args.GetType(VoiceCommandType.BOOK_CHEAPEST_TO_DEST))
                    {
                        string source = args.Values["source"].ToString();
                        string destination = args.Values["destination"].ToString();
                        CabsAPI api = new CabsAPI();
                        string token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
                        GeoResponse sResponse = await api.GeoCodingResult(token,source.ToLower());

                        GeoResponse dResponse = await api.GeoCodingResult(token,destination.ToLower());
                        if (dResponse.Code == ResponseCode.SUCCESS)
                        {
                            if (sResponse.Code == ResponseCode.SUCCESS)
                            {
                                Addroute(sResponse, dResponse, source, destination);
                                Dictionary<string, object> Parameters = args.Values;
                                List<CabEstimate> mCabs = new List<CabEstimate>();
                                mCabs = Parameters["list"] as List<CabEstimate>;
                                ETAData.Text = mCabs[0].Eta;
                                PriceData.Text = mCabs[0].CurrentEstimate.LowRange + "-" + mCabs[0].CurrentEstimate.HighRange;
                                DistanceData.Text = mCabs[0].CurrentEstimate.Distance;
                                TimeData.Text = mCabs[0].CurrentEstimate.Time;
                                if (Parameters["source"] != null)
                                    if (Parameters["source"].ToString() == null)
                                    {
                                        SrcBox.Text = "Microsoft Building 1 ".ToUpperInvariant();
                                    }
                                    else
                                    {
                                        SrcBox.Text = Parameters["source"].ToString().ToUpperInvariant();
                                    }
                                if (Parameters["destination"] != null)
                                    DestBox.Text = Parameters["destination"].ToString().ToUpperInvariant();
                            }
                            else
                            {
                                await new MessageDialog("Error retrieving Geoposition").ShowAsync();
                                return;
                            }
                        }
                        else
                        {
                            await new MessageDialog("Destiantion not found.").ShowAsync();
                            return;
                        }
                    }
                    else if (args.GetType(VoiceCommandType.NO_VOICE))
                    {
                        Dictionary<string, object> Parameters = args.Values;
                        string source = Parameters["source"].ToString();
                        string destination = Parameters["destination"].ToString();
                        CabsAPI apiCus = new CabsAPI();
                        string tokenCus = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
                        GeoResponse sResponse = await apiCus.GeoCodingResult(tokenCus, source.ToLower());
                        GeoResponse dResponse = await apiCus.GeoCodingResult(tokenCus, destination.ToLower());
                        Addroute(sResponse, dResponse, source, destination);

                        ETAData.Text = Parameters["eta"].ToString();
                        PriceData.Text = Parameters["price"].ToString();
                        DistanceData.Text = Parameters["distance"].ToString();
                        TimeData.Text = Parameters["time"].ToString();
                        SrcBox.Text = Parameters["source"].ToString().ToUpperInvariant();
                        DestBox.Text = Parameters["destination"].ToString().ToUpperInvariant();
                    }
                    else if (args.GetType(VoiceCommandType.BOOK_TO_CUSTOM_LOCATION))
                    {
                        string source = args.Values["source"].ToString();
                        string destination = args.Values["location"].ToString() ;
                        CabsAPI apiCus = new CabsAPI();
                        string tokenCus = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
                        GeoResponse sResponse = await apiCus.GeoCodingResult(tokenCus,source.ToLower());
                        GeoResponse dResponse = await apiCus.GeoCodingResult(tokenCus,destination.ToLower());
                        if (sResponse.Code == ResponseCode.SUCCESS)
                        {
                            if (dResponse.Code == ResponseCode.SUCCESS)
                            {
                                Addroute(sResponse, dResponse, source, destination);
                                Dictionary<string, object> Parameters = args.Values;
                                List<CabEstimate> DesiredCabs = new List<CabEstimate>();
                                DesiredCabs = Parameters["list"] as List<CabEstimate>;
                                ETAData.Text = DesiredCabs[0].Eta;
                                PriceData.Text = DesiredCabs[0].CurrentEstimate.LowRange + "-" + DesiredCabs[0].CurrentEstimate.HighRange;
                                DistanceData.Text = DesiredCabs[0].CurrentEstimate.Distance;
                                TimeData.Text = DesiredCabs[0].CurrentEstimate.Time;
                                SrcBox.Text = "Microsoft Building 1 Gachibowli Hyderabad".ToUpperInvariant();
                                if (Parameters["location"] != null)
                                    DestBox.Text = Parameters["location"].ToString().ToUpperInvariant();
                            }
                            else
                            {
                                await new MessageDialog("Destination not found.").ShowAsync();
                                return;
                            }
                        }
                        else
                        {
                            await new MessageDialog("Error retrieving Geoposition").ShowAsync();
                            return;
                        }
                    }
                    else if (args.GetType(VoiceCommandType.BOOK_ME_A_CAB_FROM_X_TO_Y))
                    {
                        string source = args.Values["source"].ToString();
                        string destination = args.Values["destination"].ToString();
                        CabsAPI api = new CabsAPI();
                        string token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
                        PlacesResponse sResponse = await api.GetPlaceLocation(source.ToLower(), token);
                        PlacesResponse dResponse = await api.GetPlaceLocation(destination.ToLower(), token);
                        if (sResponse.Code == ResponseCode.SUCCESS)
                        {
                            if (dResponse.Code == ResponseCode.SUCCESS)
                            {
                                Addroute(sResponse, dResponse, source, destination);
                                //AddMapIcon(Convert.ToDouble(sResponse.Location.Latitude), Convert.ToDouble(sResponse.Location.Longitude), source);
                                //AddMapIcon(Convert.ToDouble(dResponse.Location.Latitude), Convert.ToDouble(dResponse.Location.Longitude), destination);
                                //MapPolyline polyline = new MapPolyline();
                                //var coordinates = new List<BasicGeoposition>();
                                //var routeClient = new RouteClient();
                                //string sourceLoc = sResponse.Location.Latitude + "," + sResponse.Location.Longitude;
                                //string destinationLoc = dResponse.Location.Latitude + "," + dResponse.Location.Longitude;
                                //coordinates = await routeClient.route(sourceLoc, destinationLoc);
                                //polyline.StrokeColor = Windows.UI.Color.FromArgb(128, 255, 0, 0);
                                //polyline.StrokeThickness = 5;
                                //polyline.Path = new Geopath(coordinates);
                                //MyMap.MapElements.Add(polyline);
                                //ReadSpeech(new MediaElement(), sResponse.Location.Latitude + " ok " + dResponse.Location.Longitude);
                                PriceEstimateResponse pRespone = await api.GetEstimate(token, sResponse.Location.Latitude, sResponse.Location.Longitude, dResponse.Location.Latitude, dResponse.Location.Longitude);
                                //ReadSpeech(new MediaElement(), pRespone.Estimates.First().Provider);
                                SrcBox.Text = source;
                                DestBox.Text = destination;
                                //AddMapIcon(sResponse.Location.Latitude,)
                                if (pRespone.Code == ResponseCode.SUCCESS)
                                {
                                    ETAData.Text = pRespone.Estimates[1].Eta;
                                    PriceData.Text = pRespone.Estimates[1].CurrentEstimate.LowRange + "-" + pRespone.Estimates[1].CurrentEstimate.HighRange;
                                    DistanceData.Text = pRespone.Estimates[1].CurrentEstimate.Distance;
                                    TimeData.Text = pRespone.Estimates[1].CurrentEstimate.Time;
                                }
                                else
                                {
                                    await new MessageDialog("Error in estimating prices").ShowAsync();
                                    return;
                                }
                            }else
                            {
                                await new MessageDialog("Destination not found").ShowAsync();
                                return;
                            }
                        }
                        else
                        {
                            await new MessageDialog("Error retrieving Geoposition").ShowAsync();
                            return;
                        }
                    }
                    else if (args.GetType(VoiceCommandType.ESTIMATE_FROM))
                    {
                        
                        string source = args.Values["source"].ToString();
                        string destination = args.Values["destination"].ToString();
                        CabsAPI apiCus = new CabsAPI();
                        string token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
                        GeoResponse sResponse = await apiCus.GeoCodingResult(token,source.ToLower());
                        GeoResponse dResponse = await apiCus.GeoCodingResult(token,destination.ToLower());
                        Addroute(sResponse,dResponse,source,destination);
                        Arguments eArgs = args as Arguments;
                        SrcBox.Text = source;
                        DestBox.Text = destination;
                        ETAData.Text = eArgs.Values["eta"].ToString();
                        PriceData.Text = eArgs.Values["low"].ToString() + "-" + eArgs.Values["high"].ToString();
                        DistanceData.Text = eArgs.Values["distance"].ToString();
                        TimeData.Text = eArgs.Values["time"].ToString();
                    }
                    
                }
            }
            ShowLoader(false);
        }
        
        private async void ReadSpeech(MediaElement mediaElement, string readtext)
        {
            SpeechSynthesisStream stream = await new SpeechSynthesizer().SynthesizeTextToStreamAsync(readtext);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.Frame.GoBack();
            }
        }
        private async void ConfirmBooking(object sender, RoutedEventArgs e)
        {
            Geolocator locator = new Geolocator();
            var position = await locator.GetGeopositionAsync();
            CabsAPI api = new CabsAPI();
            string token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
            BookingDetailsResponse booking = await api.BookCab(token, position.Coordinate.Point.Position.Latitude.ToString(), position.Coordinate.Point.Position.Longitude.ToString());
            if (booking.Code == ResponseCode.SUCCESS)
            {
                var drivername = booking.BookingData.DriverDetails.Name;
                var drivercontact = booking.BookingData.DriverDetails.Phone_Number;
                var driverpic = booking.BookingData.DriverDetails.Picture_Url;
                var carmake = booking.BookingData.VehicleDetails.Make;
                var carmodel = booking.BookingData.VehicleDetails.Model;
                var carnumber = booking.BookingData.VehicleDetails.License_Plate;
                Dictionary<string, object> Parameters = new Dictionary<string, object>();
                Parameters.Add("time", TimeData.Text);
                Parameters.Add("price", PriceData.Text);
                Parameters.Add("distance", DistanceData.Text);
                Parameters.Add("driverName", drivername);
                Parameters.Add("driverContact", drivercontact);
                Parameters.Add("driverPic", driverpic);
                Parameters.Add("carMake", carmake);
                Parameters.Add("carModel", carmodel);
                Parameters.Add("carNumber", carnumber);
                Parameters.Add("source", SrcBox.Text);
                Parameters.Add("destination", DestBox.Text);
                Frame.Navigate(typeof(Home.CabBookingDetails), Parameters);
            }
            else
            {
                await new MessageDialog("Error retrieving Driver Info").ShowAsync();
                return;
            }
        }
    }
}
