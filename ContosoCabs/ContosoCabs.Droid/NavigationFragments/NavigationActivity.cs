using Android.Views;
using Android.OS;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.App;
using Android.Widget;
using Android.Graphics;
using Android.Content;
using ContosoCabs.Utils;
using UniversalImageLoader.Core;
using Android.Gms.Maps;
using System;
using Android.Gms.Maps.Model;
using Android.Speech;
using Java.Util;
using Android.Runtime;
using System.Collections.Generic;
using ContosoCabs.Droid.Confirm_Booking;
using Android.Locations;
using System.Threading.Tasks;
using System.Linq;
using Java.Lang;
using ContosoCabs.Droid.Dialogs;
using Android.Graphics.Drawables;
using Android.Speech.Tts;
using ContosoCabs.Droid.Speech;
using ContosoCabs.Service;
using ContosoCabs.ResponseModels.Private;
using Android.Support.V4.View;
using Android.Net;

namespace ContosoCabs.Droid.NavigationFragments
{
    [Activity(Label = "NavigationActivity", Theme = "@style/AppTheme.NoActionBar", WindowSoftInputMode =SoftInput.AdjustPan)]
    [IntentFilter (new [] { Android.Content.Intent.ActionView },
        DataScheme = "cocabs" ,
        DataHost = "",
        Categories = new[] { Android.Content.Intent.CategoryDefault })]
    public class NavigationActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, IOnMapReadyCallback, TextToSpeech.IOnInitListener
    {
        private SupportToolbar mToolbar;
        private DrawerLayout mDrawerLayout;
        private Fragment mFragment;
        private NavigationView mNavigationView;
        private ActionBarDrawerToggle mActionBarDrawerToggle;
        private TextView mTextView;
        private Typeface typeface;
        private ISharedPreferences mSharedPreference;
        private GoogleMap mGoogleMap;
        private string token;
        private TextToSpeech mTts;
        private string error;
        private string mDestinationAddress;
        private static string mSourceAddress;
        private LoadingDialog mLoadingDialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            DisplayImageOptions displayImageOptions = new DisplayImageOptions.Builder()
               .CacheInMemory(false)
               .ShowImageOnLoading(Resource.Drawable.main)
               .ResetViewBeforeLoading(false)
               .CacheOnDisk(false).Build();
            ImageLoaderConfiguration imagaeLoaderConfigurtaion = new ImageLoaderConfiguration.Builder(ApplicationContext)
                .DefaultDisplayImageOptions(displayImageOptions)
                .Build();
            ImageLoader.Instance.Init(imagaeLoaderConfigurtaion);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_navigation);
            mTts = new TextToSpeech(this, this, "com.google.android.tts");
            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            if(Intent.GetStringExtra("source")!=null)
            mSourceAddress = Intent.GetStringExtra("source");
            typeface = Typeface.CreateFromAsset(this.Assets, "JosefinSans-SemiBold.ttf");
            mLoadingDialog = new LoadingDialog(this, Resource.Drawable.main);
            mLoadingDialog.SetCancelable(false);
            Window window = mLoadingDialog.Window;
            window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
            window.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.trans)));
            mSharedPreference = GetSharedPreferences(Constants.MY_PREF, 0);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mActionBarDrawerToggle = new ActionBarDrawerToggle(this, mDrawerLayout, mToolbar, Resource.String.openD, Resource.String.closedD);
            mDrawerLayout.AddDrawerListener(mActionBarDrawerToggle);
            mActionBarDrawerToggle.SyncState();
            mNavigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            mNavigationView.ItemIconTintList = null;
            mNavigationView.SetNavigationItemSelectedListener(this);
            View headerLayout = mNavigationView.GetHeaderView(0);
            mTextView = headerLayout.FindViewById<TextView>(Resource.Id.nametodisplay);
            mTextView.SetTypeface(typeface, TypefaceStyle.Normal);
            mTextView.Text = mSharedPreference.GetString("name", " ");
            if (bundle == null)
            {
                mFragment = new MapViewFragment(this);
                FragmentManager.BeginTransaction().Replace(Resource.Id.nav_frame, mFragment).Commit();
            }
            // handle deep link here
            var x = Intent;
            System.Diagnostics.Debug.WriteLine(x.Action);
            var nmLocaiton = "NO MEETING DATA FOUND";
            if (Intent.ActionView.Equals(x.Action))
            {
                var uri = x.Data;
                nmLocaiton = uri.GetQueryParameter("nmlocation");
            }
            System.Diagnostics.Debug.Write("Deep Link: " + nmLocaiton);
        }

        public void setupGoogleMap(View childlayout)
        {
            if (mGoogleMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapview);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_speak:
                    promptSpeechAction();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void promptSpeechAction()
        {
            Intent mIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            mIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            mIntent.PutExtra(RecognizerIntent.ExtraLanguage, Locale.Default);
            mIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Give commands compatible to Contoso Cabs");
            StartActivityForResult(mIntent, 100);
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.nav_items, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            if(mDrawerLayout.IsDrawerOpen((GravityCompat.Start)))
            {
                mDrawerLayout.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                if(mFragment.Class.Equals(typeof(MapViewFragment)))
                {
                    Intent intent = new Intent(Intent.ActionMain);
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetFlags(ActivityFlags.NewTask);
                    StartActivity(intent);
                }
                else
                {
                    mFragment = new MapViewFragment(this);
                    FragmentManager.BeginTransaction().Replace(Resource.Id.nav_frame, mFragment).Commit();
                }
            }
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            int id = menuItem.ItemId;
            switch (id)
            {
                case Resource.Id.homefrag:
                    mFragment = new MapViewFragment(this);
                    FragmentManager.BeginTransaction().Replace(Resource.Id.nav_frame, mFragment).Commit();
                    break;
                case Resource.Id.profilefrag:
                    mFragment = new ProfileFragment(this);
                    FragmentManager.BeginTransaction().Replace(Resource.Id.nav_frame, mFragment).Commit();
                    break;
                case Resource.Id.paymentfrag:
                    mFragment = new PaymentFragment(this);
                    FragmentManager.BeginTransaction().Replace(Resource.Id.nav_frame, mFragment).Commit();
                    break;
                case Resource.Id.invitefrag:
                    mFragment = new InviteFragment(this);
                    FragmentManager.BeginTransaction().Replace(Resource.Id.nav_frame, mFragment).Commit();
                    break;
                case Resource.Id.feedbackfrag:
                    mFragment = new FeedbackFragment(this);
                    FragmentManager.BeginTransaction().Replace(Resource.Id.nav_frame, mFragment).Commit();
                    break;
            }
            mDrawerLayout.CloseDrawer(GravityCompat.Start);
            return true;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mGoogleMap = googleMap;
            //gMap = googleMap;
            mGoogleMap.AddMarker(new MarkerOptions()
        .SetPosition(new LatLng(10, 10))
        .SetTitle("Hello world"));
        }

        public void speakUp()
        {
            if (mTts == null)
            {
                mTts = new TextToSpeech(this, this, "com.google.android.tts");
            }
            else
            {
                mTts.Speak(error, QueueMode.Flush, null, null);
            }
        }

        public void OnInit([GeneratedEnum] OperationResult status)
        {
            if (status != OperationResult.Error)
            {
                mTts.SetLanguage(Locale.Us);
                speakUp();
            }
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