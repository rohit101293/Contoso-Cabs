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
using Android.Webkit;
using ContosoCabs.Droid.Extensible_classes;
using Android.Net;

namespace ContosoCabs.Droid.NavigationFragments
{
    [Activity(Label = "WebViewActivity",ParentActivity = typeof(NavigationActivity))]
    public class WebViewActivity : AppCompatActivity
    {
        private WebView mWebView;
        private WebSettings mWebSettings;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_webview);
            mWebView = FindViewById<WebView>(Resource.Id.mWebView);
            MyWebViewClient mWebClient = new MyWebViewClient(this);
            mWebSettings = mWebView.Settings;
            mWebSettings.JavaScriptEnabled = true;
            mWebView.SetWebViewClient(mWebClient);
            mWebView.LoadUrl("http://rohit101293.comxa.com/");
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
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