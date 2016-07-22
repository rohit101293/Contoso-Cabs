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
using Android.Gms.Maps;
using Android.Support.V7.Widget;
using Android.Graphics;
using ContosoCabs.Droid.Dialogs;
using Android.Text;
using Android.Text.Style;
using Android.Graphics.Drawables;
using Android.Gms.Maps.Model;
using ContosoCabs.Utils;
using ContosoCabs.Service;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Droid.Home;
using ContosoCabs.Droid.NavigationFragments;
using Android.Net;

namespace ContosoCabs.Droid.Confirm_Booking
{
    [Activity(Label = "ConfirmOrderActivity",ParentActivity =typeof(NavigationActivity))]
    public class ConfirmOrderActivity : AppCompatActivity, IOnMapReadyCallback,View.IOnClickListener
    {
        private GoogleMap gMap;
        private CardView mCardView;
        private Button mConfirmButton;
        private Typeface typeface;
        private LoadingDialog mLoadingDialog;
        private ISharedPreferences mSharedPreferences;
        private TextView mSource, mDest, etaText, etaValue, distText, distValue, priceText, priceVlaue, timeText, timeValue;
        private string eta, provider, capacity, distance, fare, sourcestring,deststring,high,low,time,type;
        private string slat, slng, dlat, dlng;
        private string token;
        private LatLng sourcepos, destpos;
        private MarkerOptions [] mOptions = new MarkerOptions[2];
        private ErrorDialog mErrorDialog;

        public void OnClick(View v)
        {
            switch(v.Id)
            {
                case Resource.Id.btn_request:
                    Intent mIntent = new Intent(this, typeof(ConfirmedOrderActivity));
                    mIntent.PutExtra("source", sourcestring);
                    mIntent.PutExtra("destination", deststring);
                    mIntent.PutExtra("eta", eta);
                    mIntent.PutExtra("provider", provider);
                    mIntent.PutExtra("type", type);
                    mIntent.PutExtra("distance", distance);
                    mIntent.PutExtra("fare",fare);
                    mIntent.PutExtra("high", high);
                    mIntent.PutExtra("low", low);
                    mIntent.PutExtra("time", time);
                    StartActivity(mIntent);
                    break;
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            gMap = googleMap;
            CameraUpdate mCameraUpdate1 = CameraUpdateFactory.NewLatLngZoom(sourcepos, 12);
            gMap.MoveCamera(mCameraUpdate1);

            //CameraUpdate mCameraUpdate2 = CameraUpdateFactory.NewLatLng(destpos);
            //gMap.MoveCamera(mCameraUpdate2);

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


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                SetContentView(Resource.Layout.activity_confirm_details);
                Drawable upArrow = Resources.GetDrawable(Resource.Drawable.abc_ic_ab_back_mtrl_am_alpha);
                upArrow.SetColorFilter(Resources.GetColor(Resource.Color.white), PorterDuff.Mode.SrcAtop);
                eta = Intent.GetStringExtra("eta");
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
                SpannableString s = new SpannableString("Confirm Booking");
                typeface = Typeface.CreateFromAsset(this.Assets, "JosefinSans-SemiBold.ttf");
                s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                s.SetSpan(new ForegroundColorSpan(this.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                this.TitleFormatted = s;
                mCardView = FindViewById<CardView>(Resource.Id.mapsviewcard);
                mConfirmButton = FindViewById<Button>(Resource.Id.btn_request);
                if (provider.Equals("ola", StringComparison.InvariantCultureIgnoreCase))
                {
                    mConfirmButton.SetBackgroundColor(Resources.GetColor(Resource.Color.greenola));
                    mConfirmButton.SetTextColor(Resources.GetColor(Resource.Color.black));
                }
                else
                {
                    mConfirmButton.SetBackgroundColor(Resources.GetColor(Resource.Color.black));
                }
                mSource = FindViewById<TextView>(Resource.Id.sourcetext);
                mDest = FindViewById<TextView>(Resource.Id.destinationtext);
                etaText = FindViewById<TextView>(Resource.Id.etatext);
                etaValue = FindViewById<TextView>(Resource.Id.etavalue);
                distText = FindViewById<TextView>(Resource.Id.disttext);
                distValue = FindViewById<TextView>(Resource.Id.distvalue);
                priceText = FindViewById<TextView>(Resource.Id.pricetext);
                priceVlaue = FindViewById<TextView>(Resource.Id.pricevalue);
                timeText = FindViewById<TextView>(Resource.Id.timetext);
                timeValue = FindViewById<TextView>(Resource.Id.timevalue);
                mConfirmButton.SetTypeface(typeface, TypefaceStyle.Normal);
                mSource.SetTypeface(typeface, TypefaceStyle.Normal);
                mDest.SetTypeface(typeface, TypefaceStyle.Normal);
                etaText.SetTypeface(typeface, TypefaceStyle.Normal);
                etaValue.SetTypeface(typeface, TypefaceStyle.Normal);
                distText.SetTypeface(typeface, TypefaceStyle.Normal);
                distValue.SetTypeface(typeface, TypefaceStyle.Normal);
                priceText.SetTypeface(typeface, TypefaceStyle.Normal);
                priceVlaue.SetTypeface(typeface, TypefaceStyle.Normal);
                timeText.SetTypeface(typeface, TypefaceStyle.Normal);
                timeValue.SetTypeface(typeface, TypefaceStyle.Normal);
                mSource.Text = sourcestring;
                mDest.Text = deststring;
                etaValue.Text = eta;
                distValue.Text = distance;
                priceVlaue.Text = low + "-" + high;
                timeValue.Text = time;
                mConfirmButton.Text = "REQUEST " + type;
                sourcestring = mSource.Text;
                deststring = mDest.Text;
                CabsAPI api = new CabsAPI();
                mLoadingDialog.Show();
                GeoResponse mResponse1 = await api.GeoCodingResult(token, sourcestring);
                GeoResponse mResponse2 = await api.GeoCodingResult(token, deststring);
                if (mResponse1.Code == ResponseCode.SUCCESS && mResponse2.Code == ResponseCode.SUCCESS)
                {
                    mLoadingDialog.Dismiss();
                    slat = mResponse1.Position.Latitude.ToString();
                    slng = mResponse1.Position.Longitude.ToString();
                    dlat = mResponse2.Position.Latitude.ToString();
                    dlng = mResponse2.Position.Longitude.ToString();
                }
                sourcepos = new LatLng(Convert.ToDouble(slat), Convert.ToDouble(slng));
                destpos = new LatLng(Convert.ToDouble(dlat), Convert.ToDouble(dlng));
                mConfirmButton.SetOnClickListener(this);
                setupGoogleMap();
            }
          
        }

        private void setupGoogleMap()
        {
            if(gMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapview1).GetMapAsync(this);
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