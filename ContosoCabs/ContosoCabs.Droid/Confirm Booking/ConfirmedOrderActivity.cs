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
using Android.Text;
using Android.Text.Style;
using Android.Graphics.Drawables;
using Android.Gms.Maps;
using ContosoCabs.ResponseModels.Private;
using Android.Gms.Maps.Model;
using ContosoCabs.Service;
using ContosoCabs.Utils;
using ContosoCabs.Droid.Home;
using UniversalImageLoader.Core;
using Android.Net;

namespace ContosoCabs.Droid.Confirm_Booking
{
    [Activity(Label = "ConfiremedOrderActivity", ParentActivity =typeof(GettingStartedActivity))]
    public class ConfirmedOrderActivity : AppCompatActivity,IOnMapReadyCallback,View.IOnClickListener
    {
        private Typeface typeface;
        private LoadingDialog mLoadingDialog;
        private GoogleMap gMap;
        private TextView mSource, mDest, eta, dist, price, etatext, disttext, pricetext, drivertext,cartype,carnumber;
        private string etaval, provider, capacity, distance, fare, sourcestring, deststring, high, low, time, type;
        private string slat, slng, dlat, dlng;
        private ImageView mProvider,mDriver,mPhone;
        private string token;
        private LatLng sourcepos, destpos;
        private MarkerOptions[] mOptions = new MarkerOptions[2];
        private ISharedPreferences mSharedPreferences;
        private CabsAPI mcab;
        private string phonenumner;
        private ErrorDialog mErrorDialog;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    mOptions[i] = new MarkerOptions();
                }
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.activity_confirmed_booking);
                Drawable upArrow = Resources.GetDrawable(Resource.Drawable.abc_ic_ab_back_mtrl_am_alpha);
                upArrow.SetColorFilter(Resources.GetColor(Resource.Color.white), PorterDuff.Mode.SrcAtop);
                etaval = Intent.GetStringExtra("eta");
                high = Intent.GetStringExtra("high");
                low = Intent.GetStringExtra("low");
                time = Intent.GetStringExtra("time");
                sourcestring = Intent.GetStringExtra("source");
                type = Intent.GetStringExtra("type");
                deststring = Intent.GetStringExtra("destination");
                provider = Intent.GetStringExtra("provider");
                capacity = Intent.GetStringExtra("capacity");
                distance = Intent.GetStringExtra("distance");
                fare = Intent.GetStringExtra("fare");
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
                etatext.Text = etaval;
                disttext.Text = distance;
                pricetext.Text = low + "-" + high;
                if (provider.Equals("Uber", StringComparison.InvariantCultureIgnoreCase))
                {
                    mProvider.SetImageResource(Resource.Drawable.uber);
                }
                else
                {
                    mProvider.SetImageResource(Resource.Drawable.ola);
                }
                mcab = new CabsAPI();
                mLoadingDialog.Show();
                GeoResponse mResponse1 = await mcab.GeoCodingResult(token, sourcestring);
                GeoResponse mResponse2 = await mcab.GeoCodingResult(token, deststring);
                if (mResponse1.Code == ResponseCode.SUCCESS && mResponse2.Code == ResponseCode.SUCCESS)
                {
                    slat = mResponse1.Position.Latitude.ToString();
                    slng = mResponse1.Position.Longitude.ToString();
                    dlat = mResponse2.Position.Latitude.ToString();
                    dlng = mResponse2.Position.Longitude.ToString();
                    BookingDetailsResponse mResBooking = await mcab.BookCab(token, slat, slng);
                    if (mResBooking.Code == ResponseCode.SUCCESS)
                    {
                        mLoadingDialog.Dismiss();
                        if (provider.Equals("Uber", StringComparison.InvariantCultureIgnoreCase))
                        {
                            drivertext.Text = mResBooking.BookingData.DriverDetails.Name;
                            cartype.Text = mResBooking.BookingData.VehicleDetails.Make + " " + mResBooking.BookingData.VehicleDetails.Model;
                            carnumber.Text = mResBooking.BookingData.VehicleDetails.License_Plate;
                            ImageLoader.Instance.DisplayImage(mResBooking.BookingData.DriverDetails.Picture_Url, mDriver);
                            phonenumner = mResBooking.BookingData.DriverDetails.Phone_Number;
                        }
                        else
                        {
                            phonenumner = "8110020055";
                        }
                    }
                    else
                    {

                    }

                }
                sourcepos = new LatLng(Convert.ToDouble(slat), Convert.ToDouble(slng));
                destpos = new LatLng(Convert.ToDouble(dlat), Convert.ToDouble(dlng));
                setupGoogleMap();
            }

        }

        private void setupGoogleMap()
        {
            if(gMap==null)
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

        public void OnClick(View v)
        {
            switch(v.Id)
            {
                case Resource.Id.phoneimage:
                    Intent mIntent = new Intent(Intent.ActionDial, Android.Net.Uri.Parse("tel:" + phonenumner));
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