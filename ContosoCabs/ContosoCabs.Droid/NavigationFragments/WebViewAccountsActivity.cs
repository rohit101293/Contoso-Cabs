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
using Android.Webkit;
using ContosoCabs.Droid.Extensible_classes;
using ContosoCabs.Utils;
using Android.Net;

namespace ContosoCabs.Droid.NavigationFragments
{
    [Activity(Label = "WebViewAccountsActivity")]
    public class WebViewAccountsActivity : Activity
    {
        private WebView mWebView;
        private WebSettings mWebSettings;
        private string provider,token;
        private ISharedPreferences mSharedPreference;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_webview);
            mWebView = FindViewById<WebView>(Resource.Id.mWebView);
            provider = Intent.GetStringExtra("provider");
            mSharedPreference = GetSharedPreferences(Constants.MY_PREF, 0);
            token = mSharedPreference.GetString("token", "");
            ProviderWebViewClient mWebClient = new ProviderWebViewClient(this,token);
            mWebSettings = mWebView.Settings;
            mWebSettings.JavaScriptEnabled = true;
            mWebView.SetWebViewClient(mWebClient);
            if (provider.Equals("Uber",StringComparison.InvariantCultureIgnoreCase))
            {
                mWebView.LoadUrl("https://contosocabs.azurewebsites.net/uber/login");
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Right Now we dont support Ola", ToastLength.Short).Show();
                return;
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