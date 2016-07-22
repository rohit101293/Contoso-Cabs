using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Service;
using ContosoCabs.ServiceModels;
using ContosoCabs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Geolocation;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace ContosoCabs.VoiceCommandService
{
    public sealed class ContosoCabsVoiceCommandService : IBackgroundTask
    {
        VoiceCommandServiceConnection voiceServiceConnection;
        BackgroundTaskDeferral serviceDeferral;
        ResourceMap cortanaResourceMap;
        ResourceContext cortanaContext;
        private int flag;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            serviceDeferral = taskInstance.GetDeferral();
            var userMessage1 = new VoiceCommandUserMessage();
            string book;
            taskInstance.Canceled += OnTaskCanceled;
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (triggerDetails != null)
            {
                try
                {
                    voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                    voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;
                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();
                    switch (voiceCommand.CommandName)
                    {
                        case "bookFromXToY":
                            try
                            {
                                string source = voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties["bookFromX"].FirstOrDefault();
                                string destination = voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties["bookFromY"].FirstOrDefault();
                                await SendCompletionMessageForBookFromXtoY(source, destination);
                            }
                            catch
                            {
                                userMessage1 = new VoiceCommandUserMessage();
                                book = "Please give proper saved places";
                                userMessage1.DisplayMessage = userMessage1.SpokenMessage = book;
                                var response = VoiceCommandResponse.CreateResponse(userMessage1);
                                await voiceServiceConnection.ReportSuccessAsync(response);
                            }
                            break;
                        case "costEstimate":
                            try
                            {
                                string sourceCost = voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties["source"].First();
                                string destinationCost = voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties["destination"].First();
                                await SendCompletionMessageForCostEstimate(sourceCost, destinationCost);

                            }
                            catch
                            {
                                userMessage1 = new VoiceCommandUserMessage();
                                book = "Please give proper source and destination";
                                userMessage1.DisplayMessage = userMessage1.SpokenMessage = book;
                                var response = VoiceCommandResponse.CreateResponse(userMessage1);
                                await voiceServiceConnection.ReportSuccessAsync(response);
                            }
                            break;
                        case "costEstimateCustom":
                            //try
                            //{
                            string destinationCost1 = voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties["destination"].First();
                            string slat, slng, token;
                            Geolocator locator = new Geolocator();
                            locator.DesiredAccuracyInMeters = 50;
                            var mPosition = await locator.GetGeopositionAsync();
                            slat = mPosition.Coordinate.Point.Position.Latitude.ToString();
                            slng = mPosition.Coordinate.Point.Position.Longitude.ToString();
                            token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
                            CabsAPI api = new CabsAPI();
                            ReverseGeoResposne current = await api.GetReverseCodingResultlatlng(token, slat + "," + slng);
                            string source1;
                            try
                            {
                                source1 = current.FormattedAddress.Substring(0, current.FormattedAddress.IndexOf(", Hyderabad"));
                            }
                            catch
                            {
                                source1 = current.FormattedAddress;
                            }
                            await SendCompletionMessageForCostEstimate(source1, destinationCost1);


                            //catch (Exception e)
                            //{
                            //    userMessage1 = new VoiceCommandUserMessage();
                            //    book = "please give proper Destination";
                            //    userMessage1.DisplayMessage = userMessage1.SpokenMessage = book;
                            //    var response = VoiceCommandResponse.CreateResponse(userMessage1);
                            //    await voiceServiceConnection.ReportSuccessAsync(response);
                            //}
                            break;
                        case "toCustomLocation":
                            try
                            {
                                string location = voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties["location"].FirstOrDefault();
                                await SendCompletionMessageForBookToCustomLocation(location);
                            }
                            catch (Exception e)
                            {
                                userMessage1 = new VoiceCommandUserMessage();
                                book = "Please give proper Destination";
                                userMessage1.DisplayMessage = userMessage1.SpokenMessage = book;
                                var response = VoiceCommandResponse.CreateResponse(userMessage1);
                                await voiceServiceConnection.ReportSuccessAsync(response);
                            }
                            break;
                        case "bookcheapest":
                            string mSource;
                            IReadOnlyList<string> voicecommandphrase;
                            if (voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties.TryGetValue("source", out voicecommandphrase))
                            {
                                mSource = voicecommandphrase.First();
                            }
                            else
                            {
                                mSource = "";
                            }
                            string mDest = voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties["destination"].FirstOrDefault();
                            await SendCompletionMessageForBookCheapestFromXtoY(mSource, mDest);
                            break;
                        default:
                            LaunchAppInForeground();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.ToString());
                }
            }
        }
        private async Task SendCompletionMessageForBookCheapestFromXtoY(string mSource, string mDest)
        {
            string slat, slng, dlat, dlng, token, mDisplay;
            slat = slng = dlng = dlat = null;
            var mUserMessage = new VoiceCommandUserMessage();
            var mUserPrompt = new VoiceCommandUserMessage();
            VoiceCommandResponse mResponseError, mResponseSuccess, mResponseUserPrompt;
            VoiceCommandContentTile mCabTile = new VoiceCommandContentTile();
            CabsAPI mCabsApi = new CabsAPI();
            GeoResponse mGeoResp;
            Geolocator mLocator;
            ReverseGeoResposne mRevGeoResp;
            PriceEstimateResponse mPriceEstResp;
            BookingDetailsResponse mBookingDetailsResp;
            CabEstimate mCabToDisplay;
            List<CabEstimate> mCabEstimate;
            List<VoiceCommandContentTile> mCabTiles = new List<VoiceCommandContentTile>();
            token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
            if (mDest.Equals(""))
            {
                await ShowProgressScreen("Insufficient Info");
                mDisplay = "Sorry no destination provided Please enter a valid destination";
                mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                mResponseError = VoiceCommandResponse.CreateResponse(mUserMessage);
            }
            else
            {
                mGeoResp = await mCabsApi.GeoCodingResult(token, mDest);
                if (mGeoResp.Code == ResponseCode.SUCCESS)
                {
                    dlat = mGeoResp.Position.Latitude;
                    dlng = mGeoResp.Position.Longitude;
                }
                else
                {
                    mDisplay = "Plese enter proper Destination";
                    mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                    mResponseError = VoiceCommandResponse.CreateResponse(mUserMessage);
                    await voiceServiceConnection.ReportFailureAsync(mResponseError);
                }
            }
            if (mSource.Equals(""))
            {
                mLocator = new Geolocator();
                mLocator.DesiredAccuracyInMeters = 50;
                var mPosition = await mLocator.GetGeopositionAsync();
                slat = mPosition.Coordinate.Point.Position.Latitude.ToString();
                slng = mPosition.Coordinate.Point.Position.Longitude.ToString();
                mRevGeoResp = await mCabsApi.GetReverseCodingResultlatlng(token, slat + "," + slng);
                if (mRevGeoResp.Code == ResponseCode.SUCCESS)
                {
                    try
                    {
                        mSource = mRevGeoResp.FormattedAddress.Substring(0, mRevGeoResp.FormattedAddress.IndexOf(", Hyderabad"));
                    }
                    catch
                    {
                        mSource = mRevGeoResp.FormattedAddress;
                    }
                }
                else
                {
                    mDisplay = "Source not found with current location";
                    mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                    mResponseError = VoiceCommandResponse.CreateResponse(mUserMessage);
                    await voiceServiceConnection.ReportFailureAsync(mResponseError);
                }
            }
            else
            {
                mGeoResp = await mCabsApi.GeoCodingResult(token, mSource);
                if (mGeoResp.Code == ResponseCode.SUCCESS)
                {
                    slat = mGeoResp.Position.Latitude;
                    slng = mGeoResp.Position.Longitude;
                }
                else
                {
                    mDisplay = "Source not found";
                    mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                    mResponseError = VoiceCommandResponse.CreateResponse(mUserMessage);
                    await voiceServiceConnection.ReportFailureAsync(mResponseError);
                }
            }
            if (slat != "" && slng != "" && dlat != "" && dlng != "")
            {
                mDisplay = "Looking for best cab providers from " + mSource.ToUpperInvariant() + " to " + mDest.ToUpperInvariant();
                await ShowProgressScreen(mDisplay);
                mDisplay = "Booking cheapest cab from " + mSource.ToUpperInvariant() + " to " + mDest.ToUpperInvariant();
                await ShowProgressScreen(mDisplay);
                mPriceEstResp = await mCabsApi.GetEstimate(token, slat, slng, dlat, dlng);
                if (mPriceEstResp.Code == ResponseCode.SUCCESS)
                {
                    mCabEstimate = mPriceEstResp.Estimates;
                    if (mCabEstimate.Count != 0)
                    {
                        string mDisplay1 = "Ok I have found the cheapest cab for you. Shall i book it?";
                        mUserPrompt.DisplayMessage = mUserPrompt.SpokenMessage = mDisplay1;
                        mDisplay = "Shall i book it";
                        mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                        mCabTile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;

                        mCabToDisplay = mCabEstimate[0];
                        if (mCabToDisplay.Provider.Equals("UBER"))
                            mCabTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ContosoCabs.VoiceCommandService/img/uber.png"));
                        else
                            mCabTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ContosoCabs.VoiceCommandService/img/ola.png"));

                        mCabTile.Title = mCabToDisplay.Provider;
                        mCabTile.TextLine1 = "Type : " + mCabToDisplay.Type;
                        mCabTile.TextLine2 = "ETA : " + mCabToDisplay.Eta;
                        mCabTile.TextLine3 = "Estimated Fare : " + mCabToDisplay.CurrentEstimate.LowRange + "-" + mCabToDisplay.CurrentEstimate.HighRange;
                        mCabTiles.Add(mCabTile);
                        mResponseUserPrompt = VoiceCommandResponse.CreateResponseForPrompt(mUserPrompt, mUserMessage, mCabTiles);
                        var voiceCommandConfirmation = await voiceServiceConnection.RequestConfirmationAsync(mResponseUserPrompt);
                        if (voiceCommandConfirmation.Confirmed)
                        {
                            mBookingDetailsResp = await mCabsApi.BookCab(token, slat, slng);
                            mDisplay = "Successfully booked." + mCabToDisplay.Type + " Your cab driver " + mBookingDetailsResp.BookingData.DriverDetails.Name + " will be arriving in " + mCabToDisplay.Eta;
                            mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                            mResponseSuccess = VoiceCommandResponse.CreateResponse(mUserMessage, mCabTiles);
                            await voiceServiceConnection.ReportSuccessAsync(mResponseSuccess);
                        }
                        else
                        {
                            var userMessage = new VoiceCommandUserMessage();
                            string BookingCabToDestination = "Cancelling cab request...";
                            await ShowProgressScreen(BookingCabToDestination);
                            string keepingTripToDestination = "Cancelled.";
                            userMessage.DisplayMessage = userMessage.SpokenMessage = keepingTripToDestination;
                            var response1 = VoiceCommandResponse.CreateResponse(userMessage);
                            await voiceServiceConnection.ReportSuccessAsync(response1);
                        }
                    }
                    else
                    {
                        mDisplay = "Sorry there are no cabs available right now";
                        mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                        mResponseError = VoiceCommandResponse.CreateResponse(mUserMessage);
                        await voiceServiceConnection.ReportFailureAsync(mResponseError);
                    }
                }
            }
        }
        private async Task SendCompletionMessageForBookToCustomLocation(string location)
        {
            string slat, dlat, slng, dlng;
            GeoResponse locationResponse;
            string gettingCabsToDesiredLocation = "Getting Details of " + location;
            await ShowProgressScreen(gettingCabsToDesiredLocation);
            CabsAPI api = new CabsAPI();
            string token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
            string loadingCabsToDesiredLocation = "Loading Details of cabs to " + location;
            await ShowProgressScreen(loadingCabsToDesiredLocation);
            Geolocator locator = new Geolocator();
            var userMessage1 = new VoiceCommandUserMessage();
            locator.DesiredAccuracyInMeters = 50;
            var position = await locator.GetGeopositionAsync();
            slat = position.Coordinate.Point.Position.Latitude.ToString();
            slng = position.Coordinate.Point.Position.Longitude.ToString();
            string latlng1 = slat + "," + slng;
            ReverseGeoResposne reverse = await api.GetReverseCodingResultlatlng(token, latlng1);
            locationResponse = await api.GeoCodingResult(token, location);
            dlat = locationResponse.Position.Latitude;
            dlng = locationResponse.Position.Longitude;
            PriceEstimateResponse Responseprice = await api.GetEstimate(token, slat, slng, dlat, dlng);
            List<CabEstimate> desiredList = Responseprice.Estimates;
            var sortedCabs = desiredList.OrderBy(o => o.CurrentEstimate.HighRange);
            var userMessage = new VoiceCommandUserMessage();
            var cabTiles = new List<VoiceCommandContentTile>();
            if (desiredList == null)
            {
                string foundNoCabs = "Sorry No Cabs Found";
                userMessage.DisplayMessage = foundNoCabs;
                userMessage.SpokenMessage = foundNoCabs;
            }
            else
            {
                string message = "Ok! I have found the Best Cab for you";
                userMessage.DisplayMessage = message;
                userMessage.SpokenMessage = message;
                var cabsNeeded = desiredList[0];
                var cabShow = new VoiceCommandContentTile();
                cabShow.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                if (cabsNeeded.Provider.Equals("UBER"))
                    cabShow.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ContosoCabs.VoiceCommandService/img/uber.png"));
                else
                    cabShow.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ContosoCabs.VoiceCommandService/img/ola.png"));

                cabShow.AppContext = cabsNeeded;
                cabShow.Title = cabsNeeded.Provider;
                cabShow.TextLine1 = cabsNeeded.Type;
                cabShow.TextLine2 = "ETA : " + cabsNeeded.Eta;
                cabShow.TextLine3 = "Estimated Fare: " + cabsNeeded.CurrentEstimate.LowRange + "-" + cabsNeeded.CurrentEstimate.HighRange;
                cabTiles.Add(cabShow);
                var userPrompt = new VoiceCommandUserMessage();
                VoiceCommandResponse response1;
                string source;
                try
                {
                    source = reverse.FormattedAddress.Substring(0, reverse.FormattedAddress.IndexOf(", Hyderabad"));
                }
                catch
                {
                    source = reverse.FormattedAddress;
                }
                string BookCabToDestination = "Booking " + cabsNeeded.Provider + " with " + cabsNeeded.Type + " from " + source + " to " + location + " arriving in " + cabsNeeded.Eta + cabsNeeded.CurrentEstimate.LowRange + " to " + cabsNeeded.CurrentEstimate.HighRange + " cost estimation";
                userPrompt.DisplayMessage = userPrompt.SpokenMessage = BookCabToDestination;
                var userReprompt = new VoiceCommandUserMessage();
                string confirmBookCabToDestination = "confirm booking";
                userReprompt.DisplayMessage = userReprompt.SpokenMessage = confirmBookCabToDestination;
                response1 = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userReprompt, cabTiles);
                var voiceCommandConfirmation = await voiceServiceConnection.RequestConfirmationAsync(response1);
                if (voiceCommandConfirmation != null)
                {
                    if (voiceCommandConfirmation.Confirmed == true)
                    {
                        BookingDetailsResponse booking = await api.BookCab(token, slat, slng);
                        string BookCabInformation = "Cab booked. your cab driver " + booking.BookingData.DriverDetails.Name + " will be arriving shortly.";
                        userMessage1 = new VoiceCommandUserMessage();
                        //userPrompt.DisplayMessage = userPrompt.SpokenMessage = BookCabInformation;
                        userMessage.DisplayMessage = userMessage.SpokenMessage = BookCabInformation;
                        userMessage1.DisplayMessage = userMessage1.SpokenMessage = BookCabInformation;
                        var response = VoiceCommandResponse.CreateResponse(userMessage1, cabTiles);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                    }
                    else
                    {
                        userMessage = new VoiceCommandUserMessage();
                        string BookingCabToDestination = "Cancelling cab request...";
                        await ShowProgressScreen(BookingCabToDestination);
                        string keepingTripToDestination = "Cancelled.";
                        userMessage.DisplayMessage = userMessage.SpokenMessage = keepingTripToDestination;
                        response1 = VoiceCommandResponse.CreateResponse(userMessage);
                        await voiceServiceConnection.ReportSuccessAsync(response1);
                    }
                }
            }
        }
        private async Task SendCompletionMessageForCostEstimate(string source, string destination)
        {
            string slat, slng, dlat, dlng, token, mDisplay;
            slat = slng = dlng = dlat = null;
            var mUserMessage = new VoiceCommandUserMessage();
            var mUserPrompt = new VoiceCommandUserMessage();
            VoiceCommandResponse mResponseError;
            VoiceCommandContentTile mCabTile = new VoiceCommandContentTile();
            CabsAPI mCabsApi = new CabsAPI();
            GeoResponse mGeoResp;
            List<VoiceCommandContentTile> mCabTiles = new List<VoiceCommandContentTile>();
            token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
            if (destination.Equals(""))
            {
                await ShowProgressScreen("Insufficient Info");
                mDisplay = "Sorry no destination provided Please enter a valid destination";
                mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                mResponseError = VoiceCommandResponse.CreateResponse(mUserMessage);
            }
            else
            {
                mGeoResp = await mCabsApi.GeoCodingResult(token, destination);
                if (mGeoResp.Code == ResponseCode.SUCCESS)
                {
                    dlat = mGeoResp.Position.Latitude;
                    dlng = mGeoResp.Position.Longitude;
                }
                else
                {
                    mDisplay = "Destination not found";
                    mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                    mResponseError = VoiceCommandResponse.CreateResponse(mUserMessage);
                    await voiceServiceConnection.ReportFailureAsync(mResponseError);
                }
            }
            mGeoResp = await mCabsApi.GeoCodingResult(token, source);
            if (mGeoResp.Code == ResponseCode.SUCCESS)
            {
                slat = mGeoResp.Position.Latitude;
                slng = mGeoResp.Position.Longitude;
            }
            else
            {
                mDisplay = "Source not found";
                mUserMessage.DisplayMessage = mUserMessage.SpokenMessage = mDisplay;
                mResponseError = VoiceCommandResponse.CreateResponse(mUserMessage);
                await voiceServiceConnection.ReportFailureAsync(mResponseError);
            }
            string EstimatingCabsFromXtoY = "Getting cost Details from  " + source + " to " + destination;
            await ShowProgressScreen(EstimatingCabsFromXtoY);
            CabsAPI api = new CabsAPI();
            string loadingCabsFromXtoY = "Loading Details of cabs from " + source + " to " + destination;
            await ShowProgressScreen(loadingCabsFromXtoY);
            GeoResponse sResponse = await api.GeoCodingResult(token, source);
            GeoResponse dResponse = await api.GeoCodingResult(token, destination);
            PriceEstimateResponse pResponse = await api.GetEstimate(token, slat, slng, dlat, dlng);
            var userMessage = new VoiceCommandUserMessage();
            var cabTiles = new List<VoiceCommandContentTile>();
            List<CabEstimate> cabsAvaialble = pResponse.Estimates;
            CabEstimate cabSelected;
            if (cabsAvaialble == null)
            {
                string foundNoCabs = "Sorry No Cabs Found";
                userMessage.DisplayMessage = foundNoCabs;
                userMessage.SpokenMessage = foundNoCabs;
            }
            else
            {
                string message = "Ok! I have found the following Cabs for you";
                userMessage.DisplayMessage = message;
                userMessage.SpokenMessage = message;
                cabSelected = await AvailableList(cabsAvaialble, "Which cab do you want to book?", "Book the selected cab?");
                var userPrompt = new VoiceCommandUserMessage();
                VoiceCommandResponse response;
                string BookCabToDestination = "Booking " + cabSelected.Provider + " with " + cabSelected.Type + " from " + source + " to " + destination + " arriving in " + cabSelected.Eta + " with " + cabSelected.CurrentEstimate.LowRange + " to " + cabSelected.CurrentEstimate.HighRange + " cost estimation";
                userPrompt.DisplayMessage = userPrompt.SpokenMessage = BookCabToDestination;
                var userReprompt = new VoiceCommandUserMessage();
                string confirmBookCabToDestination = "Confirm booking";
                userReprompt.DisplayMessage = userReprompt.SpokenMessage = confirmBookCabToDestination;
                response = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userReprompt, cabTiles);
                var voiceCommandConfirmation = await voiceServiceConnection.RequestConfirmationAsync(response);
                if (voiceCommandConfirmation != null)
                {
                    if (voiceCommandConfirmation.Confirmed == true)
                    {
                        BookingDetailsResponse booking = await api.BookCab(token, sResponse.Position.Latitude, sResponse.Position.Longitude);
                        string BookingCabToDestination = "Booking cab";
                        await ShowProgressScreen(BookingCabToDestination);
                        var cabShow = new VoiceCommandContentTile();
                        cabShow.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                        if (cabSelected.Provider.Equals("UBER"))
                            cabShow.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ContosoCabs.VoiceCommandService/img/uber.png"));
                        else
                            cabShow.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ContosoCabs.VoiceCommandService/img/ola.png"));

                        cabShow.Title = cabSelected.Type;
                        cabShow.TextLine1 = "ETA : " + cabSelected.Eta;
                        cabShow.TextLine2 = "Estimated Fare: " + cabSelected.CurrentEstimate.LowRange + "-" + cabSelected.CurrentEstimate.HighRange;
                        cabShow.TextLine3 = "Call driver at " + booking.BookingData.DriverDetails.Phone_Number;
                        cabTiles.Add(cabShow);
                        var userMessage1 = new VoiceCommandUserMessage();
                        string BookCabInformation = "Cab booked. your cab driver " + booking.BookingData.DriverDetails.Name + "will be arriving shortly."; //Vehicle number :"+ booking.BookingData.VehicleDetails.License_Plate
                        userMessage.DisplayMessage = userMessage.SpokenMessage = BookCabInformation;
                        userMessage1.DisplayMessage = userMessage1.SpokenMessage = BookCabInformation;
                        response = VoiceCommandResponse.CreateResponse(userMessage1, cabTiles);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                    }
                    else
                    {
                        userMessage = new VoiceCommandUserMessage();
                        string BookingCabToDestination = "Cancelling cab request...";
                        await ShowProgressScreen(BookingCabToDestination);
                        string keepingTripToDestination = "Cancelled.";
                        userMessage.DisplayMessage = userMessage.SpokenMessage = keepingTripToDestination;
                        response = VoiceCommandResponse.CreateResponse(userMessage);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                    }
                }
            }
        }
        private async Task SendCompletionMessageForBookFromXtoY(string source, string destination)
        {
            string gettingCabsFromXtoY = "Getting Details of your " + source + " and " + destination;
            await ShowProgressScreen(gettingCabsFromXtoY);
            CabsAPI api = new CabsAPI();
            string token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
            string loadingCabsFromXtoY = "Loading Details of cabs from " + source + " and " + destination;
            await ShowProgressScreen(loadingCabsFromXtoY);
            PlacesResponse sResponse = await api.GetPlaceLocation(source.ToLower(), token);
            PlacesResponse dResponse = await api.GetPlaceLocation(destination.ToLower(), token);
            //ReadSpeech(new MediaElement(), sResponse.Location.Latitude + " ok " + dResponse.Location.Longitude);
            PriceEstimateResponse pRespone = await api.GetEstimate(token, sResponse.Location.Latitude, sResponse.Location.Longitude, dResponse.Location.Latitude, dResponse.Location.Longitude);
            var userMessage = new VoiceCommandUserMessage();
            var cabTiles = new List<VoiceCommandContentTile>();
            List<CabEstimate> cabsAvaialble = pRespone.Estimates;
            CabEstimate cabSelected;
            if (cabsAvaialble == null)
            {
                string foundNoCabs = "Sorry No Cabs Found";
                userMessage.DisplayMessage = foundNoCabs;
                userMessage.SpokenMessage = foundNoCabs;
            }
            else
            {
                string message = "Ok! I have found the following Cabs for you";
                userMessage.DisplayMessage = message;
                userMessage.SpokenMessage = message;
                cabSelected = await AvailableList(cabsAvaialble, "Which cab do you want to book?", "Book the selected cab?");
                var userPrompt = new VoiceCommandUserMessage();
                VoiceCommandResponse response;
                string BookCabToDestination = "Booking " + cabSelected.Provider + " with " + cabSelected.Type + " from " + source + " to " + destination + " arriving in " + cabSelected.Eta + " with " + cabSelected.CurrentEstimate.LowRange + " to " + cabSelected.CurrentEstimate.HighRange + " cost estimation";
                userPrompt.DisplayMessage = userPrompt.SpokenMessage = BookCabToDestination;
                var userReprompt = new VoiceCommandUserMessage();
                string confirmBookCabToDestination = "Confirm booking";
                userReprompt.DisplayMessage = userReprompt.SpokenMessage = confirmBookCabToDestination;
                response = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userReprompt);
                var voiceCommandConfirmation = await voiceServiceConnection.RequestConfirmationAsync(response);
                if (voiceCommandConfirmation != null)
                {
                    if (voiceCommandConfirmation.Confirmed == true)
                    {
                        string BookingCabToDestination = "Booking cab";
                        await ShowProgressScreen(BookingCabToDestination);
                        BookingDetailsResponse booking = await api.BookCab(token, sResponse.Location.Latitude, sResponse.Location.Longitude);
                        var userMessage1 = new VoiceCommandUserMessage();
                        string BookCabInformation = "Cab booked. your cab driver " + booking.BookingData.DriverDetails.Name + "will be arriving shortly."; //Vehicle number :"+ booking.BookingData.VehicleDetails.License_Plate;                       
                        userMessage.DisplayMessage = userMessage.SpokenMessage = BookCabInformation;
                        var cabShow = new VoiceCommandContentTile();
                        cabShow.ContentTileType = VoiceCommandContentTileType.TitleWithText;
                        cabShow.Title = cabSelected.Type;
                        cabShow.TextLine1 = "ETA : " + cabSelected.Eta;
                        cabShow.TextLine2 = "Estimated Fare: " + cabSelected.CurrentEstimate.LowRange + "-" + cabSelected.CurrentEstimate.HighRange;
                        cabShow.TextLine3 = "Call driver at " + booking.BookingData.DriverDetails.Phone_Number;
                        cabTiles.Add(cabShow);
                        userMessage1.DisplayMessage = userMessage1.SpokenMessage = BookCabInformation;
                        response = VoiceCommandResponse.CreateResponse(userMessage1, cabTiles);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                    }
                    else
                    {
                        userMessage = new VoiceCommandUserMessage();
                        string BookingCabToDestination = "Cancelling cab request...";
                        await ShowProgressScreen(BookingCabToDestination);
                        string keepingTripToDestination = "Cancelled.";
                        userMessage.DisplayMessage = userMessage.SpokenMessage = keepingTripToDestination;
                        response = VoiceCommandResponse.CreateResponse(userMessage);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                    }
                }
            }
        }
        private async Task<CabEstimate> AvailableList(IEnumerable<CabEstimate> selected, string selectionMessage, string secondSelectionMessage)
        {
            var userPrompt = new VoiceCommandUserMessage();
            userPrompt.DisplayMessage = userPrompt.SpokenMessage = selectionMessage;
            var userReprompt = new VoiceCommandUserMessage();
            userReprompt.DisplayMessage = userReprompt.SpokenMessage = secondSelectionMessage;
            var ContentTiles = new List<VoiceCommandContentTile>();
            foreach (var cabAvailable in selected)
            {
                var cabTile = new VoiceCommandContentTile();
                // cabTile.ContentTileType = VoiceCommandContentTileType.TitleWithText;
                cabTile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                if (cabAvailable.Provider.Equals("UBER"))
                    cabTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ContosoCabs.VoiceCommandService/img/uber.png"));
                else
                    cabTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ContosoCabs.VoiceCommandService/img/ola.png"));
                cabTile.AppContext = cabAvailable;
                // cabTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri(cabAvailable.ImageURL));
                cabTile.Title = cabAvailable.Provider;
                cabTile.TextLine1 = cabAvailable.Type;
                cabTile.TextLine3 = "Es. Fare: " + cabAvailable.CurrentEstimate.LowRange + "-" + cabAvailable.CurrentEstimate.HighRange;
                cabTile.TextLine2 = "ETA : " + cabAvailable.Eta;
                ContentTiles.Add(cabTile);
            }
            var response = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userReprompt, ContentTiles);
            var voiceCommandDisambiguationResult = await voiceServiceConnection.RequestDisambiguationAsync(response);
            if (voiceCommandDisambiguationResult != null)
            {
                return (CabEstimate)voiceCommandDisambiguationResult.SelectedItem.AppContext;
            }
            return null;
        }
        private async Task ShowProgressScreen(string message)
        {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;
            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceServiceConnection.ReportProgressAsync(response);
        }
        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            string book = "opening contoso cabs";
            userMessage.DisplayMessage = userMessage.SpokenMessage = book;
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            response.AppLaunchArgument = "";
            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }
        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }
        private async void ReadSpeech(MediaElement mediaElement, string readtext)
        {
            SpeechSynthesisStream stream = await new SpeechSynthesizer().SynthesizeTextToStreamAsync(readtext);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }
    }
}
