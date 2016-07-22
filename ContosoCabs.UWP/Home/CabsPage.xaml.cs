using ContosoCabs.Models;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Service;
using ContosoCabs.Utils;
using ContosoCabs.UWP.Speech;
using ContosoCabs.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace ContosoCabs.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CabsPage : Page
    {
        private string mSource, mDestination;
        private SpeechRecognitionResult speechRecognition;
        public ObservableCollection<string> DestinationSuggestions { get; private set; }
        public ObservableCollection<string> SourceSuggestions { get; private set; }
        string slat, slng, dlat, dlng,token,latlng,CurrentSource;
        public string Token;
        int flag;
        private Geoposition position;
        private CabsListViewModel _cabsView;
        private string _source, _destination;
        private bool _isFromInsight = false;
        private MapIcon _sourceIcon;
        public CabsPage()
        {
            this.InitializeComponent();
            checkConn();
            DestinationSuggestions = new ObservableCollection<string>();
            SourceSuggestions = new ObservableCollection<string>();
            var storage = Windows.Storage.ApplicationData.Current.LocalSettings;
            Token = storage.Values["Token"].ToString();
        }

        private async void checkConn()
        {
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.Frame.GoBack();
            }
        }
        private void AddMapIcon(double latitude, double longitude)
        {
            if (_sourceIcon != null)
            {
                MyMap.MapElements.Remove(_sourceIcon);
            } 
            _sourceIcon = new MapIcon();
            _sourceIcon.Location = new Geopoint(new BasicGeoposition()
            {
                Latitude = latitude,
                Longitude = longitude
            });
            _sourceIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            _sourceIcon.Title = "Your Location";
            MyMap.MapElements.Add(_sourceIcon);
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
                if (speechRecognition != null)
                {
                    mSource = this.SemanticInterpretation("source", speechRecognition);
                    mDestination = this.SemanticInterpretation("destination", speechRecognition);
                    if (mSource != null)
                        SearchSourceBox.Text = mSource;
                    if (mDestination != null)
                        SearchDestinationBox.Text = mDestination;
                }
                try
                {
                    var parameter = e.Parameter as WwwFormUrlDecoder;
                    _destination = parameter.GetFirstValueByName("nmlocation");
                    if (_destination == null || _destination.Equals(""))
                    {
                        _destination = "Gachibowli";
                    }
                    System.Diagnostics.Debug.WriteLine(_destination);
                    _isFromInsight = true;
                }
                catch (Exception)
                {

                }
                finally
                {
                    MyMap.MapServiceToken = "YP9WTSzOHUo0dbahsH8J~J-K6u01dT98SF4uCCKpiwA~AnGaYM6yJxoLF1tGHEIXHFskGfwSRJTr1S5aO1dB-TCXjQ1ZX0xEWgeYslbC3Fov";
                    Geolocator locator = new Geolocator();
                    locator.DesiredAccuracyInMeters = 50;
                    position = await locator.GetGeopositionAsync();
                    await MyMap.TrySetViewAsync(position.Coordinate.Point, 17D);
                    progress.IsActive = false;
                    mySlider.Value = MyMap.ZoomLevel;
                    AddMapIcon(position.Coordinate.Point.Position.Latitude, position.Coordinate.Point.Position.Longitude);
                    CabsAPI api = new CabsAPI();
                    latlng = position.Coordinate.Point.Position.Latitude.ToString() + "," + position.Coordinate.Point.Position.Longitude.ToString();
                    ReverseGeoResposne res = await api.GetReverseCodingResultlatlng(Token, latlng);
                    if(res.Code == ResponseCode.SUCCESS)
                    {
                        CurrentSource = res.FormattedAddress;
                        SearchSourceBox.Text = CurrentSource;
                        _source = CurrentSource;
                        string lat = position.Coordinate.Point.Position.Latitude.ToString();
                        string lng = position.Coordinate.Point.Position.Longitude.ToString();
                        _cabsView = new CabsListViewModel(lng, lat, Token);
                        latlng = position.Coordinate.Point.Position.Latitude.ToString() + "," + position.Coordinate.Point.Position.Longitude.ToString();
                        if (_isFromInsight)
                        {
                            SearchDestinationBox.Text = _destination;
                            GeoResponse location;
                            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                            token = localSettings.Values["Token"].ToString();
                            location = await api.GeoCodingResult(token, _destination);
                            if(location.Code == ResponseCode.SUCCESS)
                            {
                                dlat = location.Position.Latitude;
                                dlng = location.Position.Longitude;
                                AddMapIcon(double.Parse(dlat), double.Parse(dlng));
                                _cabsView.SetLatLng(dlat, dlng);
                                ShowLoader(true);
                                await _cabsView.RefreshView(CabsListViewModel.REFRESH_ESTIMATE);
                                CabsListView.ItemsSource = _cabsView.Cabs;
                                ShowLoader(false);
                            }
                            else
                            {
                                await new MessageDialog("Error fetching coordinates").ShowAsync();
                            }
                        }
                        else
                        {
                            ShowLoader(true);
                            await _cabsView.RefreshView(CabsListViewModel.REFRESH_SURGE);
                            CabsListView.ItemsSource = _cabsView.Cabs;
                            ShowLoader(false);
                        }
                    }
                    else
                    {
                        await new MessageDialog("No such location exists").ShowAsync();
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
        private async void SearchSourceBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                _source = args.SelectedItem.ToString();
                flag = 1;
                progress.IsActive = true;
                //string dlat, dlng, token;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                token = localSettings.Values["Token"].ToString();
                GeoResponse location;
                CabsAPI api = new CabsAPI();
                location = await api.GeoCodingResult(token, args.SelectedItem.ToString());
                if(location.Code == ResponseCode.SUCCESS)
                {
                    slat = location.Position.Latitude;
                    slng = location.Position.Longitude;
                    BasicGeoposition curPos = new BasicGeoposition();
                    curPos.Latitude = double.Parse(slat);
                    curPos.Longitude = double.Parse(slng);
                    await MyMap.TrySetViewAsync(new Geopoint(curPos), 17D);
                    AddMapIcon(double.Parse(slat), double.Parse(slng));
                    CabsResponse response = await api.GetNearbyCabs(slat, slng, Token);
                    if (response.Code == ResponseCode.SUCCESS)
                    {
                        progress.IsActive = false;
                        CabsListView.ItemsSource = response.Cabs;
                    }
                    else
                    {
                        await new MessageDialog("Error fetching nearby cabs").ShowAsync();
                        return;
                    }
                }
                else
                {
                    await new MessageDialog("Error retrieving Geoposition coordinates").ShowAsync();
                    return;
                }
            }
        }
        private void SearchSourceBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
        }
        private async void SearchSourceBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    SourceSuggestions.Clear();
                    CabsAPI api = new CabsAPI();
                    SearchResponse searchResponse = await api.GetSuggestions(sender.Text, Token);
                    if (searchResponse.Code == ResponseCode.SUCCESS)
                    {
                        List<String> suggestions = searchResponse.Suggestions.Select(su => su.Text).ToList();
                        if (suggestions.Count == 0)
                        {
                            SearchSourceBox.ItemsSource = new string[] { "No Results" };
                        }
                        else
                        {
                            SourceSuggestions = new ObservableCollection<string>(suggestions);
                            SearchSourceBox.ItemsSource = suggestions;
                        }
                    }
                    else
                    {
                        SearchSourceBox.ItemsSource = new string[] { "Auto complete not available" };
                    }
                }
            }
        }
        private void SearchDestinationBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
        }
        private async void CabsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Cab cabDetails = e.ClickedItem as Cab;
            if (_source == null || _source == "")
            {
                await new MessageDialog("Please Enter Source").ShowAsync();
                return;
            }
            if (_destination == null || _destination == "")
            {
                await new MessageDialog("Please Enter Destination").ShowAsync();
                return;

            }
            if (_destination != null && _source != null)
            {
                Dictionary<string, object> Parameters = new Dictionary<string, object>();
                Parameters.Add("eta", cabDetails.Eta);
                Parameters.Add("price", cabDetails.FareData.Surge);
                Parameters.Add("distance", cabDetails.Distance);
                Parameters.Add("time", cabDetails.Time);
                Parameters.Add("source", _source);
                Parameters.Add("destination", _destination);
                Arguments data = new Arguments(null);
                data.AddType(VoiceCommandType.NO_VOICE, true);
                data.Values = Parameters;
                Frame.Navigate(typeof(BookingPage), data);
            }
        }
        private async void SearchDestinationBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                _destination = args.SelectedItem.ToString();
                CabsAPI api = new CabsAPI();
                GeoResponse location;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                token = localSettings.Values["Token"].ToString();
                location = await api.GeoCodingResult(token, args.SelectedItem.ToString());
                if(location.Code == ResponseCode.SUCCESS)
                {
                    dlat = location.Position.Latitude;
                    dlng = location.Position.Longitude;
                    AddMapIcon(double.Parse(dlat), double.Parse(dlng));
                    _cabsView.SetLatLng(dlat, dlng);
                    ShowLoader(true);
                    await _cabsView.RefreshView(CabsListViewModel.REFRESH_ESTIMATE);
                    CabsListView.ItemsSource = _cabsView.Cabs;
                    ShowLoader(false);
                }
                else
                {
                    await new MessageDialog("Error retrieving Geoposition").ShowAsync();
                    return;
                }
            }
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
        private async void SearchDestinationBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    DestinationSuggestions.Clear();
                    CabsAPI api = new CabsAPI();
                    SearchResponse searchResponse = await api.GetSuggestions(sender.Text, Token);
                    if (searchResponse.Code == ResponseCode.SUCCESS)
                    {
                        List<String> suggestions = searchResponse.Suggestions.Select(su => su.Text).ToList();
                        if (suggestions.Count == 0)
                        {
                            SearchDestinationBox.ItemsSource = new string[] { "No Results" };
                        }
                        else
                        {
                            DestinationSuggestions = new ObservableCollection<string>(suggestions);
                            SearchDestinationBox.ItemsSource = suggestions;
                        }

                    }
                    else
                    {
                        SearchDestinationBox.ItemsSource = new string[] { "Auto complete not available" };
                    }
                }
            }
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
