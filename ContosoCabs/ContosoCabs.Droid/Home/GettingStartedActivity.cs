using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using ContosoCabs.Droid.NavigationFragments;
using Android.Speech;
using Android.Util;
using ContosoCabs.Droid.Confirm_Booking;
using ContosoCabs.Droid.Dialogs;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Graphics.Drawables;
using Android.Locations;
using System.Threading.Tasks;
using Java.Util;
using Android.Speech.Tts;
using ContosoCabs.Droid.Speech;
using Android.Net;
using ContosoCabs.Service;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Utils;

namespace ContosoCabs.Droid.Home
{
    [Activity(Label = "GettingStartedActivity", Theme = "@style/AppTheme.NoActionBar")]
    [IntentFilter (new [] { Intent.ActionView},Categories =new[] { Intent.CategoryBrowsable,Intent.CategoryDefault},DataScheme = "cocabs")]
    public class GettingStartedActivity : AppCompatActivity, View.IOnClickListener, ILocationListener , TextToSpeech.IOnInitListener, IRecognitionListener
    {
        private Button mGetStarted,mUri;
        private ImageView mSpeak;
        private TextView mTextView;
        private LoadingDialog mLoadingDialog;
        private Typeface typeface;
        private LocationManager mLocationManager;
        private Location mLocation;
        private string mSourceAddress;
        private string mLocationProvider;
        private string mDestinationAddress;
        private string token;
        private TextToSpeech mTts;
        private ISharedPreferences mSharedPreference;
        private string error;
        private ISharedPreferencesEditor mEditor;
        private Intent mRecognizerIntent;
        private SpeechRecognizer speech = null;
        private ErrorDialog mErrorDialog;

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btnGetStarted:
                    Intent mIntent = new Intent(this, typeof(NavigationActivity));
                    mIntent.PutExtra("source", mSourceAddress);
                    mEditor.PutString("source", mSourceAddress).Apply();
                    StartActivity(mIntent);
                    break;
                case Resource.Id.imagespeech:
                    processSpeechInput();
                    break;
                case Resource.Id.btnuri:
                    Intent mInt = new Intent(Intent.ActionView);
                    mInt.SetData(Android.Net.Uri.Parse("contosocabs://?mtgtitle=rohit"));
                    StartActivity(mInt);
                    break;
            }
        }

        private void processSpeechInput()
        {
            mRecognizerIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            mRecognizerIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            mRecognizerIntent.PutExtra(RecognizerIntent.ExtraLanguage, Locale.Default);
            mRecognizerIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Give commands compatible to Contoso Cabs");
            mRecognizerIntent.PutExtra("android.speech.extra.DICTATION_MODE", true);
            mRecognizerIntent.PutExtra(RecognizerIntent.ExtraPartialResults, true);
            mRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 3000);
            mRecognizerIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference,
                              "en");
            speech.StartListening(mRecognizerIntent);
            StartActivityForResult(mRecognizerIntent, 100);
        }

        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            switch (requestCode)
            {
                case 100:

                    if (resultCode == Result.Ok && data != null)
                    {

                        Intent mIntent = new Intent(this, typeof(ConfirmedCookingActivityViaSpeech));
                        IList<string> mResult = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                        System.Diagnostics.Debug.WriteLine(mResult[0]);
                        string command = mResult[0];
                        Classifier mClassifier = new Classifier(command);
                        ClassifiedResult mResp = mClassifier.Classify();
                        switch (mResp.Type)
                        {

                            case VoiceCommandType.BOOK_ME_CHEAPEST_CAB_FROM_X_TO_Y:
                                string recs = mResp.GetData("source");
                                if (!recs.Equals(""))
                                {
                                    mSourceAddress = recs;
                                }
                                mDestinationAddress = mResp.GetData("destination");
                                error = "Booking Cheapest cab from " + mSourceAddress + "to" + mDestinationAddress;
                                speakUp();
                                mIntent.PutExtra("mSource", mSourceAddress);
                                mIntent.PutExtra("mDestination", mDestinationAddress);
                                StartActivity(mIntent);
                                break;

                            case VoiceCommandType.BOOK_TO_CUSTOM_LOCATION:
                                mDestinationAddress = mResp.GetData("location");
                                mIntent.PutExtra("mSource", mSourceAddress);
                                mIntent.PutExtra("mDestinationAddress", mDestinationAddress);
                                StartActivity(mIntent);
                                break;
                            
                                
                            case VoiceCommandType.BOOK_ME_A_CAB_FROM_X_TO_Y:
                                {
                                    CabsAPI api = new CabsAPI();
                                    string source = mResp.GetData("source");
                                    string destination = mResp.GetData("destination");
                                    mLoadingDialog.Show();
                                    PlacesResponse mResposneSource = await api.GetPlaceLocation(source, token);
                                    PlacesResponse mResponseDest = await api.GetPlaceLocation(destination, token);
                                    if (mResposneSource.Code == ResponseCode.SUCCESS && mResponseDest.Code == ResponseCode.SUCCESS)
                                    {
                                        string msource = mResposneSource.Location.Latitude + "," + mResposneSource.Location.Longitude;
                                        string mDest = mResponseDest.Location.Latitude + "," + mResponseDest.Location.Longitude;
                                        ReverseGeoResposne mResSrc = await api.GetReverseCodingResultlatlng(token, msource);
                                        ReverseGeoResposne mResDest = await api.GetReverseCodingResultlatlng(token, mDest);
                                        if (mResSrc.Code == ResponseCode.SUCCESS && mResDest.Code == ResponseCode.SUCCESS)
                                        {
                                            error = "Booking cab from " + source + "to" + destination;
                                            speakUp();
                                            mLoadingDialog.Dismiss();
                                            mSourceAddress = mResSrc.FormattedAddress;
                                            mDestinationAddress = mResDest.FormattedAddress;
                                            mIntent.PutExtra("mSource", mSourceAddress);
                                            mIntent.PutExtra("mDestination", mDestinationAddress);
                                            StartActivity(mIntent);
                                        }
                                        else
                                        {
                                            error = "Google maps error";
                                            speakUp();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        error = "These places are not there in your notebook";
                                        speakUp();
                                        return;
                                    }
                                    break;
                                }
                            case VoiceCommandType.INVALID_COMMAND:
                                error = "Invalid Commmand Please try again";
                                speakUp();
                                break;


                        }
                    }

                    break;
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if(!isOnline())
            {
                mErrorDialog.Show();
                return;
            }
            SetContentView(Resource.Layout.activity_get_started);
            mLocationManager = GetSystemService(LocationService) as LocationManager;
            mTts = new TextToSpeech(this, this, "com.google.android.tts");
            speech = SpeechRecognizer.CreateSpeechRecognizer(this);
            speech.SetRecognitionListener(this);
            Criteria mLocationServiceCriteria = new Criteria
            {
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };
            IList<string> acceptableLocationProviders = mLocationManager.GetProviders(mLocationServiceCriteria, true);
            if (acceptableLocationProviders.Any())
            {
                mLocationProvider = acceptableLocationProviders.First();
            }
            else
            {
                mLocationProvider = string.Empty;
            }
            mSharedPreference = GetSharedPreferences(Constants.MY_PREF, 0);
            token = mSharedPreference.GetString("token", " ");
            mEditor = mSharedPreference.Edit();
            mLoadingDialog = new LoadingDialog(this, Resource.Drawable.main);
            mLoadingDialog.SetCancelable(false);
            Window window = mLoadingDialog.Window;
            window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
            window.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.trans)));
            SpannableString s = new SpannableString("Contoso Cabs");
            typeface = Typeface.CreateFromAsset(this.Assets, "JosefinSans-SemiBold.ttf");
            s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
            s.SetSpan(new ForegroundColorSpan(this.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
            this.TitleFormatted = s;
            mGetStarted = FindViewById<Button>(Resource.Id.btnGetStarted);
            mUri = FindViewById<Button>(Resource.Id.btnuri);
            mSpeak = FindViewById<ImageView>(Resource.Id.imagespeech);
            mTextView = FindViewById<TextView>(Resource.Id.bookText);
            mTextView.SetTypeface(typeface, TypefaceStyle.Normal);
            mGetStarted.SetTypeface(typeface, TypefaceStyle.Normal);
            mGetStarted.SetOnClickListener(this);
            mUri.SetOnClickListener(this);
            mSpeak.SetOnClickListener(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            mLocationManager.RequestLocationUpdates(mLocationProvider, 0, 0, this);
        }
        protected override void OnPause()
        {
            base.OnPause();
            mLocationManager.RemoveUpdates(this);
        }

        public void OnLocationChanged(Location location)
        {
            try
            {
                mLocation = location;
                if (mLocation == null)
                {
                    mSourceAddress = "Location Not Found";
                    System.Diagnostics.Debug.WriteLine("failure nl");
                }
                else
                {
                    mSourceAddress = string.Format("{0},{1}", mLocation.Latitude, mLocation.Longitude);
                    Geocoder mGeoCoder = new Geocoder(this);
                    IList<Address> addressList = mGeoCoder.GetFromLocation(mLocation.Latitude, mLocation.Longitude, 5);
                    Address mAddress = addressList.FirstOrDefault();
                    if (mAddress != null)
                    {
                        StringBuilder deviceAddress = new StringBuilder();
                        int max = mAddress.MaxAddressLineIndex;
                        if (max > 2)
                        {
                            max = 3;
                        }
                        for (int i = 0; i < max; i++)
                        {
                            deviceAddress.Append(mAddress.GetAddressLine(i)).Append(",");
                        }
                        mSourceAddress = deviceAddress.ToString().Trim().Substring(0, deviceAddress.Length - 1);
                        System.Diagnostics.Debug.WriteLine("success" + mSourceAddress);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("failure nf");
                        mSourceAddress = "Not Found";
                    }
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("failure anf");
                mSourceAddress = "Address not found";
            }

        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }
        public void speakUp()
        {
            if(mTts == null)
            {
                mTts = new TextToSpeech(this, this, "com.google.android.tts");
            }
            else
            {
                mTts.Speak(error, QueueMode.Flush, null,null);
            }
        }
        public void OnInit([GeneratedEnum] OperationResult status)
        {
            if(status!=OperationResult.Error)
            {
                mTts.SetLanguage(Locale.Us);
                speakUp();
            }
        }

        public void OnBeginningOfSpeech()
        {
            
        }

        public void OnBufferReceived(byte[] buffer)
        {
            
        }

        public void OnEndOfSpeech()
        {
            
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            
        }

        public void OnEvent(int eventType, Bundle @params)
        {
            
        }

        public void OnPartialResults(Bundle partialResults)
        {
            IList<string> matches = partialResults.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            string text = "";
            foreach(string result in matches)
            {
                text = text + result;
            }
            mTextView.Text = text;
        }

        public void OnReadyForSpeech(Bundle @params)
        {
            
        }

        public void OnResults(Bundle results)
        {
            
        }

        public void OnRmsChanged(float rmsdB)
        {
           
        }
        public bool isOnline()
        {
            ConnectivityManager mConnectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            NetworkInfo mNetInfo = mConnectivityManager.ActiveNetworkInfo;
            if (mNetInfo != null && mNetInfo.IsConnectedOrConnecting)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}