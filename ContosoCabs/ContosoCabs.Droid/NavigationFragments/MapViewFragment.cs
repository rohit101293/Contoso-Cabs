using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Support.V7.Widget;
using ContosoCabs.Droid.Adapters;
using ContosoCabs.Service;
using ContosoCabs.Models;
using ContosoCabs.Utils;
using Android.Gms.Maps;
using ContosoCabs.Droid.Extensible_classes;
using Android.Gms.Maps.Model;
using ContosoCabs.Droid.Dialogs;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using Android.Graphics.Drawables;
using System;
using ContosoCabs.ResponseModels.Private;
using Android.Locations;
using Android.Widget;
using ContosoCabs.Droid.Confirm_Booking;
using ContosoCabs.ServiceModels;
using Java.Lang;
using ContosoCabs.Droid.Search;
using Android.Net;

namespace ContosoCabs.Droid.NavigationFragments
{
    public class MapViewFragment : Fragment, IOnMapReadyCallback, View.IOnClickListener

    {
      
        public RecyclerView mRecyclerView;
        public CabRecyclerAdapter mAdapter;
        public GoogleMap gMap;
        public NavigationActivity mActivity;
        private SearchSuggestionsAdapter sourceSuggestionsAdapter;
        private SearchSuggestionsAdapter destinationSuggestionsAdapter;
        private LoadingDialog mLoadingDialog;
        private CardView mCardView;
        private Typeface typeface;
        private Fragment kpMapFragment;
        private Android.Support.V7.Widget.SearchView mSearchSource;
        private Android.Support.V7.Widget.SearchView mSearchDestination;
        private List<CabEstimate> mCabsEst;
        private List<Cab> mCabs;
        private AppCompatAutoCompleteTextView autoSourceBox;
        private AppCompatAutoCompleteTextView autoDestinationBox;
        private ISharedPreferences mSharedPreference;
        private string token,msourceRecieved;
        public int VERTICAL_ITEM_SPACE = 30;
        private string slat, slng, dlat, dlng,dest;
        private CabsAPI api;
        private bool hasDestinationset = false;
        private bool hasSourceSet = false;
        static int count = 0;
        private MarkerOptions[] mOptions = new MarkerOptions[2];
        private LatLng mLatLngSource, mLatLngDest;
        private bool isMapAvailable = false;
        private ErrorDialog mErrorDialog;
        public MapViewFragment(NavigationActivity activity)
        {
            this.mActivity = activity;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_home_layout, container, false);
            init(view);
            return view;
        }
        public async void init(View rootView)
        {
            if(!isOnline())
            {
                mErrorDialog.Show();
                return;
            }
            api = new CabsAPI();
            mLoadingDialog = new LoadingDialog(mActivity, Resource.Drawable.main);
            mLoadingDialog.SetCancelable(false);
            Window window = mLoadingDialog.Window;
            window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
            window.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.trans)));
            SpannableString s = new SpannableString("Home");
            typeface = Typeface.CreateFromAsset(mActivity.Assets, "JosefinSans-SemiBold.ttf");
            s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
            s.SetSpan(new ForegroundColorSpan(this.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
            mActivity.TitleFormatted = s;
            mSharedPreference = mActivity.GetSharedPreferences(Constants.MY_PREF,0);
            token = mSharedPreference.GetString("token", " ");
            if (mActivity.Intent.GetStringExtra("source") != null)
                msourceRecieved = mActivity.Intent.GetStringExtra("source");
            else
                msourceRecieved = mSharedPreference.GetString("source", "1,ISB Rd,Gachibowli");
            autoSourceBox = rootView.FindViewById<AppCompatAutoCompleteTextView>(Resource.Id.sourceauto);
            autoSourceBox.Text = msourceRecieved;
            sourceSuggestionsAdapter = new SearchSuggestionsAdapter(mActivity);
            autoSourceBox.Adapter = sourceSuggestionsAdapter;
            destinationSuggestionsAdapter = new SearchSuggestionsAdapter(mActivity);
            autoSourceBox.ItemClick += mSourceTextBoxClicked;
            autoDestinationBox = rootView.FindViewById<AppCompatAutoCompleteTextView>(Resource.Id.destinationauto);
            autoDestinationBox.Adapter = destinationSuggestionsAdapter;
            autoDestinationBox.ItemClick += mDestinationTextBoxClicked;
            mLoadingDialog.Show();
            mRecyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.cabrecycler);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Application.Context);
            mRecyclerView.SetLayoutManager(linearLayoutManager);
            mRecyclerView.AddItemDecoration(new VerticalSpaceItemDecoration(VERTICAL_ITEM_SPACE));
            LayoutInflater inflater = (LayoutInflater)Activity.GetSystemService(Context.LayoutInflaterService);
            View Childlayout = inflater.Inflate(Resource.Layout.fragment_google_maps, null);
            if (msourceRecieved!=null)
            {
                GeoResponse mResponseGetSourcelatlng = await api.GeoCodingResult(token, msourceRecieved);
                slat = mResponseGetSourcelatlng.Position.Latitude;
                slng = mResponseGetSourcelatlng.Position.Longitude;
                hasSourceSet = false;
                setSourceMarker(msourceRecieved);
            }       
            CabsResponse response = await api.GetNearbyCabs(slat, slng, Constants.AUTH_TOKEN);
            if(response.Code == ResponseCode.SUCCESS)
            {
                mCabs = response.Cabs;
                mAdapter = new CabRecyclerAdapter(Application.Context, mCabs, mActivity);
                mAdapter.ItemClick += OnItemClick;
                mRecyclerView.SetAdapter(mAdapter);
                mLoadingDialog.Dismiss();
            }
        }

        private async void mDestinationTextBoxClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            dest = destinationSuggestionsAdapter.GetObjectAt(e.Position);
            System.Diagnostics.Debug.Write(dest);
            autoDestinationBox.Text = destinationSuggestionsAdapter.GetObjectAt(e.Position);
            if (msourceRecieved!=null&&dest!=null)
            {
                GeoResponse mResponseGetDestlatlng = await api.GeoCodingResult(token, dest);
                dlat = mResponseGetDestlatlng.Position.Latitude;
                dlng = mResponseGetDestlatlng.Position.Longitude;
                setDestinationMarker(dest);
                mLoadingDialog.Show();
                PriceEstimateResponse mRes = await api.GetEstimate(token, slat, slng, dlat, dlng);
                mCabsEst = mRes.Estimates;
                CabRecyclerAdapterWithDestination mAdapter = new CabRecyclerAdapterWithDestination(Application.Context, mCabsEst, mActivity);
                mAdapter.ItemClick1 += Item_Click_New;
                mRecyclerView.SetAdapter(mAdapter);
                mLoadingDialog.Dismiss();
            }
        }

        private void mSourceTextBoxClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            string itemClicked = sourceSuggestionsAdapter.GetObjectAt(e.Position);
            msourceRecieved = itemClicked;
            autoSourceBox.Text = itemClicked;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            kpMapFragment = FragmentManager.FindFragmentById(Resource.Id.mapscard);
            if (kpMapFragment == null)
            {
                kpMapFragment = MapFragment.NewInstance();
                ChildFragmentManager.BeginTransaction().Replace(Resource.Id.mapscard, kpMapFragment).Commit();
            }
            setUpGoogleMap();
        }

        private void setUpGoogleMap()
        {
            ((MapFragment)kpMapFragment).GetMapAsync(this);
        }

        public override void OnResume()
        {
            base.OnResume();
            kpMapFragment = null;
            kpMapFragment = FragmentManager.FindFragmentById(Resource.Id.mapscard);
            if (kpMapFragment == null)
            {
                kpMapFragment = MapFragment.NewInstance();
                ChildFragmentManager.BeginTransaction().Replace(Resource.Id.mapscard, kpMapFragment).Commit();
            }
            setUpGoogleMap();

        }
        public void OnItemClick (object sender, int position)
        {

            if (autoSourceBox.Text.ToString() == "")
            {
                Toast.MakeText(Application.Context, "Please fill the source box", ToastLength.Short).Show();
                return;
            }
            else if (autoDestinationBox.Text.ToString() == "")
            {
                Toast.MakeText(Application.Context, "Please fill the destination box", ToastLength.Short).Show();
                return;
            }
        }

        public void setSourceMarker(string mSource)
        {
            while (!isMapAvailable) ;
            double slatd, slngd;
            if (slat != "")
            {
                slatd = Convert.ToDouble(slat);
            }
            else
            {
                slatd = 17.4622;
            }
            if (slng != "")
            {
                slngd = Convert.ToDouble(slng);
            }
            else
            {
                slngd = 78.3568;
            }
            if (!hasSourceSet)
            {
                mLatLngSource = new LatLng(slatd, slngd);
                mOptions[1] = new MarkerOptions()
               .SetPosition(mLatLngSource)
               .SetTitle(msourceRecieved)
               .SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow))
               .Draggable(true);
                gMap.AddMarker(mOptions[1]);
                hasSourceSet = true;
                CameraUpdate mCameraUpdate1 = CameraUpdateFactory.NewLatLngZoom(mLatLngSource, 12);
                gMap.MoveCamera(mCameraUpdate1);
            }
        }

        public void setDestinationMarker(string mDestination)
        {
            while (!isMapAvailable) ;
            double dlatd, dlngd;

            if (dlat != "")
            {
                dlatd = Convert.ToDouble(dlat);
            }
            else
            {
                dlatd = 17.3888;
            }
            if (dlng != "")
            {
                dlngd = Convert.ToDouble(dlng);
            }
            else
            {
                dlngd = 78.4356;
            }

            if (!hasDestinationset)
            {
                mLatLngDest = new LatLng(dlatd, dlngd);
                mOptions[0] = new MarkerOptions()
               .SetPosition(mLatLngDest)
               .SetTitle(dest)
               .Draggable(true);
                gMap.AddMarker(mOptions[0]);
                hasDestinationset = true;
                
            }
        }
        
        public void OnMapReady(GoogleMap googleMap)
        {
            gMap = googleMap;
            isMapAvailable = true;
        }

        public void OnClick(View v)
        {
           
        }

        private void Item_Click_New(object sender, int position)
        {
            if(autoSourceBox.Text.ToString() == ""|| autoDestinationBox.Text.ToString() == "")
            {
                Toast.MakeText(Application.Context, "Please fill source and destination", ToastLength.Short).Show();
            }
            else
            {
                CabEstimate mCab = mCabsEst[position];
                Intent mIntent = new Intent(mActivity, typeof(ConfirmOrderActivity));
                mIntent.PutExtra("source", msourceRecieved);
                mIntent.PutExtra("destination", dest);
                mIntent.PutExtra("eta", mCab.Eta);
                mIntent.PutExtra("provider", mCab.Provider);
                mIntent.PutExtra("type", mCab.Type);
                mIntent.PutExtra("distance", mCab.CurrentEstimate.Distance);
                mIntent.PutExtra("fare", mCab.CurrentEstimate.FareData.BaseFare);
                mIntent.PutExtra("high", mCab.CurrentEstimate.HighRange);
                mIntent.PutExtra("low", mCab.CurrentEstimate.LowRange);
                mIntent.PutExtra("time", mCab.CurrentEstimate.Time);
                mActivity.StartActivity(mIntent);
            }
           
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            

        }
        public bool isOnline()
        {
            ConnectivityManager mConnectivityManager = (ConnectivityManager)mActivity.GetSystemService(Context.ConnectivityService);
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