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
using Android.Graphics;
using ContosoCabs.Droid.Dialogs;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using ContosoCabs.Utils;
using Android.Text;
using Android.Text.Style;
using Android.Graphics.Drawables;
using ContosoCabs.Service;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Droid.Home;
using ContosoCabs.ServiceModels;
using Android.Speech.Tts;
using Java.Util;
using UniversalImageLoader.Core;
using ContosoCabs.Droid.NavigationFragments;
using Android.Net;

namespace ContosoCabs.Droid.Confirm_Booking
{
    [Activity(Label = "ConfirmedCookingActivityViaSpeech",ParentActivity =typeof(NavigationActivity))]
    public class ConfirmedCookingActivityViaSpeech : AppCompatActivity,IOnMapReadyCallback,TextToSpeech.IOnInitListener,View.IOnClickListener
    {
        private Typeface typeface;
        private LoadingDialog mLoadingDialog;
        private GoogleMap gMap;
        private TextView mSource, mDest, eta, dist, price, etatext, disttext, pricetext, drivertext, cartype, carnumber;
        private string etaval, provider, capacity, distance, fare, sourcestring, deststring, high, low, time, type;
        private string slat, slng, dlat, dlng;
        private ImageView mProvider, mDriver, mPhone;
        private string token;
        private LatLng sourcepos, destpos;
        private MarkerOptions[] mOptions = new MarkerOptions[2];
        private ISharedPreferences mSharedPreferences;
        private List<CabEstimate> mCabs;
        private CabEstimate cab;
        private TextToSpeech mTts;
        private string error,phonenumber;
        private ErrorDialog mErrorDialog;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                CabsAPI api = new CabsAPI();
                SetContentView(Resource.Layout.activity_confirmed_booking);
                Drawable upArrow = Resources.GetDrawable(Resource.Drawable.abc_ic_ab_back_mtrl_am_alpha);
                upArrow.SetColorFilter(Resources.GetColor(Resource.Color.white), PorterDuff.Mode.SrcAtop);
                sourcestring = Intent.GetStringExtra("mSource");
                deststring = Intent.GetStringExtra("mDestination");
                mSharedPreferences = GetSharedPreferences(Constants.MY_PREF, 0);
                SupportActionBar.SetHomeAsUpIndicator(upArrow);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                token = mSharedPreferences.GetString("token", " ");
                mLoadingDialog = new LoadingDialog(this, Resource.Drawable.main);
                mLoadingDialog.SetCancelable(false);
                Window window = mLoadingDialog.Window;
                window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
                window.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.trans)));
                SpannableString s = new SpannableString("Successfully Booked");
                typeface = Typeface.CreateFromAsset(this.Assets, "JosefinSans-SemiBold.ttf");
                s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                s.SetSpan(new ForegroundColorSpan(this.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                this.TitleFormatted = s;
                mSource = FindViewById<TextView>(Resource.Id.sourcetext);
                mDest = FindViewById<TextView>(Resource.Id.destinationtext);
                eta = FindViewById<TextView>(Resource.Id.eta);
                dist = FindViewById<TextView>(Resource.Id.dist);
                price = FindViewById<TextView>(Resource.Id.price);
                etatext = FindViewById<TextView>(Resource.Id.etavalue);
                disttext = FindViewById<TextView>(Resource.Id.distvalue);
                pricetext = FindViewById<TextView>(Resource.Id.pricevalue);
                drivertext = FindViewById<TextView>(Resource.Id.drivername);
                cartype = FindViewById<TextView>(Resource.Id.cartype);
                carnumber = FindViewById<TextView>(Resource.Id.carnumber);
                mProvider = FindViewById<ImageView>(Resource.Id.cabprovider);
                mDriver = FindViewById<ImageView>(Resource.Id.driverimage);
                mPhone = FindViewById<ImageView>(Resource.Id.phoneimage);
                mPhone.SetOnClickListener(this);
                mSource.SetTypeface(typeface, TypefaceStyle.Normal);
                mDest.SetTypeface(typeface, TypefaceStyle.Normal);
                eta.SetTypeface(typeface, TypefaceStyle.Normal);
                etatext.SetTypeface(typeface, TypefaceStyle.Normal);
                price.SetTypeface(typeface, TypefaceStyle.Normal);
                pricetext.SetTypeface(typeface, TypefaceStyle.Normal);
                dist.SetTypeface(typeface, TypefaceStyle.Normal);
                disttext.SetTypeface(typeface, TypefaceStyle.Normal);
                drivertext.SetTypeface(typeface, TypefaceStyle.Normal);
                cartype.SetTypeface(typeface, TypefaceStyle.Normal);
                carnumber.SetTypeface(typeface, TypefaceStyle.Normal);
                mSource.Text = sourcestring.ToUpperInvariant();
                mDest.Text = deststring.ToUpperInvariant();
                mLoadingDialog.Show();
                GeoResponse mResponse1 = await api.GeoCodingResult(token, sourcestring);
                GeoResponse mResponse2 = await api.GeoCodingResult(token, deststring);
                if (mResponse1.Code == ResponseCode.SUCCESS && mResponse2.Code == ResponseCode.SUCCESS)
                {
                    slat = mResponse1.Position.Latitude.ToString();
                    slng = mResponse1.Position.Longitude.ToString();
                    dlat = mResponse2.Position.Latitude.ToString();
                    dlng = mResponse2.Position.Longitude.ToString();
                    PriceEstimateResponse mCabsResponse = await api.GetEstimate(token, slat, slng, dlat, dlng);
                    if (mCabsResponse.Code == ResponseCode.SUCCESS)
                    {
                        BookingDetailsResponse mResBooking = await api.BookCab(token, slat, slng);
                        if (mResBooking.Code == ResponseCode.SUCCESS)
                        {
                            mLoadingDialog.Dismiss();
                            mCabs = mCabsResponse.Estimates;
                            etatext.Text = mCabs[0].Eta;
                            disttext.Text = mCabs[0].CurrentEstimate.Distance;
                            pricetext.Text = mCabs[0].CurrentEstimate.LowRange + "-" + mCabs[0].CurrentEstimate.HighRange;
                            error = "Successfully booked " + mCabs[0].Type;
                            speakUp();
                            if (mCabs[0].Provider.Equals("uber", StringComparison.InvariantCultureIgnoreCase))
                            {
                                mProvider.SetImageResource(Resource.Drawable.uber);
                            }
                            else
                            {
                                mProvider.SetImageResource(Resource.Drawable.ola);
                            }
                            if (mCabs[0].Provider.Equals("Uber", StringComparison.InvariantCultureIgnoreCase))
                            {
                                drivertext.Text = mResBooking.BookingData.DriverDetails.Name;
                                cartype.Text = mResBooking.BookingData.VehicleDetails.Make + " " + mResBooking.BookingData.VehicleDetails.Model;
                                carnumber.Text = mResBooking.BookingData.VehicleDetails.License_Plate;
                                ImageLoader.Instance.DisplayImage(mResBooking.BookingData.DriverDetails.Picture_Url, mDriver);
                                phonenumber = mResBooking.BookingData.DriverDetails.Phone_Number;
                            }
                            else
                            {
                                phonenumber = "8110020055";
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        mLoadingDialog.Dismiss();
                        Toast.MakeText(Application.Context, "Error", ToastLength.Short).Show();
                        return;
                    }
                    sourcepos = new LatLng(Convert.ToDouble(slat), Convert.ToDouble(slng));
                    destpos = new LatLng(Convert.ToDouble(dlat), Convert.ToDouble(dlng));
                    setupGoogleMap();
                }
                else
                {
                    mLoadingDialog.Dismiss();
                    Toast.MakeText(Application.Context, "Wrong Address", ToastLength.Short).Show();
                    return;
                }
            }
            
        }

        private void setupGoogleMap()
        {
            if (gMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapview2).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            gMap = googleMap;
            CameraUpdate mCameraUpdate1 = CameraUpdateFactory.NewLatLngZoom(sourcepos, 12);
            gMap.MoveCamera(mCameraUpdate1);

            mOptions[0] = new MarkerOptions()
                .SetPosition(sourcepos)
                .SetTitle(sourcestring)
                .SetSnippet("This is source")
                .Draggable(true)
                .SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow));

            mOptions[1] = new MarkerOptions()
                .SetPosition(destpos)
                .SetTitle(deststring)
                .SetSnippet("This is destination")
                .Draggable(true);

            gMap.AddMarker(mOptions[0]);
            gMap.AddMarker(mOptions[1]);
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

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.phoneimage:
                    Intent mIntent = new Intent(Intent.ActionDial, Android.Net.Uri.Parse("tel:" + phonenumber));
                    StartActivity(mIntent);
                    break;
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