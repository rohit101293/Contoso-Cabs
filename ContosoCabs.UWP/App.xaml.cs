using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Media.SpeechRecognition;
using ContosoCabs.UWP.Home;
using ContosoCabs.UWP.Speech;
using ContosoCabs.Service;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.ServiceModels;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using ContosoCabs.Utils;

namespace ContosoCabs.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        SpeechSynthesizer speech;
        Type navigationTo;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            speech = new SpeechSynthesizer();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                // Register a handler for BackRequested events and set the back button's visibility

                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    rootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;
            }

            if (rootFrame.Content == null)
            {

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values["LoggedIn"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("In main page bro");
                    rootFrame.Navigate(typeof(MainPage));
                }
                else if ((bool)localSettings.Values["LoggedIn"])

                {
                    System.Diagnostics.Debug.WriteLine("In nav page bro");
                    rootFrame.Navigate(typeof(Navigation.NavigationPage));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("In main page bro");
                    rootFrame.Navigate(typeof(MainPage));
                }

            }
            // Ensure the current window is active
            Window.Current.Activate();
            try
            {
                StorageFile vcdStorageFile = await Package.Current.InstalledLocation.GetFileAsync(@"VoiceCommands.xml");
                await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
            }

        }
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // for each occurence of navigation event, back button should be made visible
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;

        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                if (args.Kind == ActivationKind.VoiceCommand)
                {
                    VoiceCommandActivatedEventArgs voiceCommandArgs = args as VoiceCommandActivatedEventArgs;
                    Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = voiceCommandArgs.Result;
                    String text = speechRecognitionResult.Text;
                    Frame rootFrame = Window.Current.Content as Frame;
                    string voiceCommandName = speechRecognitionResult.RulePath[0];
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    if (rootFrame == null)
                    {
                        rootFrame = new Frame();
                    }
                    rootFrame.NavigationFailed += OnNavigationFailed;
                    if (localSettings.Values["LoggedIn"] == null)
                    {
                        string readtext = "You need to login first";
                        ReadSpeech(new MediaElement(), readtext);
                        rootFrame.Navigate(typeof(MainPage), speechRecognitionResult);
                    }
                    else
                    {
                        switch (voiceCommandName)
                        {
                            case "simpleOpeningApp":
                                rootFrame.Navigate(typeof(Navigation.NavigationPage), speechRecognitionResult);
                                break;
                            case "voicecommand2":
                                rootFrame.Navigate(typeof(Navigation.NavigationPage));
                                break;
                            case "toCustomLocationApp":
                                Arguments parameters = await ToCustomLocation(voiceCommandArgs);
                                rootFrame.Navigate(typeof(BookingPage), parameters);
                                break;
                            case "bookcheapestApp":
                                Arguments data = await ProcessBookCheapest(voiceCommandArgs);
                                rootFrame.Navigate(typeof(BookingPage), data);
                                break;
                            case "costEstimateApp":
                                rootFrame.Navigate(typeof(EstimationList), speechRecognitionResult);
                                break;
                            case "bookFromXToYApp":
                                rootFrame.Navigate(typeof(BookingPage), ProcessBookFromXToY(speechRecognitionResult));
                                break;
                        }
                    }
                    Window.Current.Content = rootFrame;
                    Window.Current.Activate();
                }
                else if (args.Kind == ActivationKind.Protocol)
                {
                    var protocolArgs = args as ProtocolActivatedEventArgs;
                    var queryArgs = new WwwFormUrlDecoder(protocolArgs.Uri.Query);
                    Frame rootFrame = Window.Current.Content as Frame;

                    // Do not repeat app initialization when the Window already has content,
                    // just ensure that the window is active
                    if (rootFrame == null)
                    {
                        // Create a Frame to act as the navigation context and navigate to the first page
                        rootFrame = new Frame();

                        rootFrame.NavigationFailed += OnNavigationFailed;

                        if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                        {
                            //TODO: Load state from previously suspended application
                        }

                        // Place the frame in the current Window
                        Window.Current.Content = rootFrame;
                        rootFrame.Navigate(typeof(CabsPage), queryArgs);
                        Window.Current.Activate();
                    }
                }
            }
        }


        private async Task<Arguments> ProcessBookCheapest(VoiceCommandActivatedEventArgs voiceCommandArgs)
        {
            string slat, dlat, slng, dlng, mSource, mDestination, token;
            GeoResponse mResponse;
            CabsAPI api = new CabsAPI();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            token = localSettings.Values["Token"].ToString();
            IReadOnlyList<string> voiceCommandPhrases;
            if (voiceCommandArgs.Result.SemanticInterpretation.Properties.TryGetValue("source", out voiceCommandPhrases))
            {
                mSource = voiceCommandPhrases.First();
            }
            else
            {
                mSource = "";
            }
            if (voiceCommandArgs.Result.SemanticInterpretation.Properties.TryGetValue("destination", out voiceCommandPhrases))
            {
                mDestination = voiceCommandPhrases.First();
            }
            else
            {
                mDestination = "";
            }
            if (mSource == "")
            {
                Geolocator locator = new Geolocator();
                locator.DesiredAccuracyInMeters = 50;
                var position = await locator.GetGeopositionAsync();
                slat = position.Coordinate.Point.Position.Latitude.ToString();
                slng = position.Coordinate.Point.Position.Longitude.ToString();
                ReverseGeoResposne mRevResponse = await api.GetReverseCodingResultlatlng(token, position.Coordinate.Point.Position.Latitude.ToString()+","+position.Coordinate.Point.Position.Longitude.ToString());
                //mSource = mRevResponse.FormattedAddress;
                try
                {
                    mSource = mRevResponse.FormattedAddress.Substring(0, mRevResponse.FormattedAddress.IndexOf(", Hyderabad"));
                }
                catch
                {
                    mSource = mRevResponse.FormattedAddress;
                }
                System.Diagnostics.Debug.WriteLine("source is" + mSource);
            }
            else
            {
                mResponse = await api.GeoCodingResult(token, mSource);
                if(mResponse.Code == ResponseCode.SUCCESS)
                {
                    slat = mResponse.Position.Latitude;
                    slng = mResponse.Position.Longitude;
                }
                else
                {
                    await new MessageDialog("Errors fetching source cordinates").ShowAsync();
                    return null;
                }
            }
            if(mDestination == "")
            {
                await new MessageDialog("No Destination provided").ShowAsync();
                return null;
            }
            else
            {
                mResponse = await api.GeoCodingResult(token, mDestination);
                if(mResponse.Code == ResponseCode.SUCCESS)
                {
                    dlat = mResponse.Position.Latitude;
                    dlng = mResponse.Position.Longitude;
                    PriceEstimateResponse mResponseprice = await api.GetEstimate(token, slat, slng, dlat, dlng);
                    if(mResponseprice.Code == ResponseCode.SUCCESS)
                    {
                        List<CabEstimate> mList = mResponseprice.Estimates;
                        //var sortedCabs = mList.OrderBy(o => o.AverageEstimate);
                        Dictionary<string, object> Parameters = new Dictionary<string, object>();
                        Parameters.Add("source", mSource);
                        Parameters.Add("destination", mDestination);
                        Parameters.Add("list", mList);
                        Arguments data = new Arguments(null);
                        data.AddType(VoiceCommandType.IF_FROM_APP, true);
                        data.AddType(VoiceCommandType.BOOK_CHEAPEST_TO_DEST, true);
                        data.AddType(VoiceCommandType.BOOK_ME_CHEAPEST_CAB_FROM_X_TO_Y, true);
                        data.Values = Parameters;
                        return data;
                    }
                    else
                    {
                        await new MessageDialog("Error fetching estimaates").ShowAsync();
                        return null;
                    }
                }
                else
                {
                    await new MessageDialog("Error retrieving Destination cordinates").ShowAsync();
                    return null;
                }
            }
        }
        private async Task<Arguments> ToCustomLocation(VoiceCommandActivatedEventArgs voiceCommandArgs)
        {
            string slat, dlat, slng, dlng, desiredLocation, token;
            GeoResponse locationResponse;
            CabsAPI api = new CabsAPI();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            token = localSettings.Values["Token"].ToString();
            IReadOnlyList<string> voiceCommandPhrases;
            if (voiceCommandArgs.Result.SemanticInterpretation.Properties.TryGetValue("location", out voiceCommandPhrases))
            {
                desiredLocation = voiceCommandPhrases.First();
            }
            else
            {
                desiredLocation= "";
            }
            Geolocator locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;
            var position = await locator.GetGeopositionAsync();
            slat = position.Coordinate.Point.Position.Latitude.ToString();
            slng = position.Coordinate.Point.Position.Longitude.ToString();
            string source;
            ReverseGeoResposne current = await api.GetReverseCodingResultlatlng(token, position.Coordinate.Point.Position.Latitude.ToString()+","+ position.Coordinate.Point.Position.Longitude.ToString());
            try
            {
                source = current.FormattedAddress.Substring(0, current.FormattedAddress.IndexOf(", Hyderabad"));
            }
            catch
            {
                source = current.FormattedAddress;
            }
            ReadSpeech(new MediaElement(),source);
            ReadSpeech(new MediaElement(), desiredLocation);
            locationResponse = await api.GeoCodingResult(token, desiredLocation);
            if(locationResponse.Code == ResponseCode.SUCCESS)
            {
                dlat = locationResponse.Position.Latitude;
                dlng = locationResponse.Position.Longitude;
                PriceEstimateResponse mResponseprice = await api.GetEstimate(token, slat, slng, dlat, dlng);
                if(mResponseprice.Code == ResponseCode.SUCCESS)
                {
                    List<CabEstimate> desiredList = mResponseprice.Estimates;
                    var sortedCabs = desiredList.OrderBy(o => o.CurrentEstimate.HighRange);
                    Dictionary<string, object> Parameters = new Dictionary<string, object>();
                    Parameters.Add("location", desiredLocation);
                    Parameters.Add("source", source);
                    Parameters.Add("list", desiredList);
                    Arguments data = new Arguments(null);
                    data.AddType(VoiceCommandType.IF_FROM_APP, true);
                    data.AddType(VoiceCommandType.BOOK_TO_CUSTOM_LOCATION, true);
                    data.Values = Parameters;
                    return data;
                }
                else
                {
                    await new MessageDialog("Errors fetching estimates").ShowAsync();
                    return null;
                }
            }
            else
            {
                await new MessageDialog("Errors fetching cordinates").ShowAsync();
                return null;
            }
        }
        private Arguments ProcessBookFromXToY(SpeechRecognitionResult res)
        {
            Arguments args = new Arguments(null);
            args.AddType(VoiceCommandType.BOOK_ME_A_CAB_FROM_X_TO_Y, true);
            try
            {
                var source = res.SemanticInterpretation.Properties["bookFromX"];
                var destination = res.SemanticInterpretation.Properties["bookFromY"];
                args.AddArgument("source", source.FirstOrDefault());
                args.AddArgument("destination", destination.FirstOrDefault());
                ReadSpeech(new MediaElement(), "All Success ");
            }
            catch (Exception ex)
            {
                args.AddArgument("source", "Auntys Place");
                args.AddArgument("destination", "Home");
                //ReadSpeech(new MediaElement(), ex.ToString());
            }
            return args;
        }
        private async void ReadSpeech(MediaElement mediaElement, string readtext)
        {
            SpeechSynthesisStream stream = await speech.SynthesizeTextToStreamAsync(readtext);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        private string SemanticInterpretation(string v, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[v].FirstOrDefault();
        }


        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

    }
}
