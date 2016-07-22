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
using ContosoCabs.Service;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Droid.NavigationFragments;
using ContosoCabs.Utils;
using Android.Net;
using ContosoCabs.Droid.Home;

namespace ContosoCabs.Droid.Extensible_classes
{
    public class ProviderWebViewClient : WebViewClient
    {
        private string code;
        private CabsAPI mCabsApi;
        private string token;
        private WebViewAccountsActivity webViewAccountsActivity;
        private ISharedPreferences mSharePreference;
        private ISharedPreferencesEditor mEditor;
        private bool isDone = false;
        public ProviderWebViewClient(WebViewAccountsActivity webViewAccountsActivity, string token)
        {
            this.webViewAccountsActivity = webViewAccountsActivity;
            this.token = token;
            mSharePreference = webViewAccountsActivity.GetSharedPreferences(Constants.MY_PREF, 0);
            mEditor = mSharePreference.Edit();
            mCabsApi = new CabsAPI();
        }


        public override async void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);


            if (url.Contains("https://contosocabs.azurewebsites.net/oauth/uber?code="))
            {
                int index = url.IndexOf("=");
                code = url.Substring(index + 1);
                code = code.Trim();
                System.Diagnostics.Debug.WriteLine("ac code :" + code);

                SendTokenResponse mResp = await mCabsApi.SendToken(token, code);
                System.Diagnostics.Debug.WriteLine("In webviewclient resp code " + mResp.Code);
                if (mResp.Code == ResponseCode.SUCCESS)
                {
                    mEditor.PutBoolean("hasAccounts", true).Apply();
                    isDone = true;
                    webViewAccountsActivity.StartActivity(new Intent(webViewAccountsActivity, typeof(GettingStartedActivity)));
                    webViewAccountsActivity.Finish();
                }
                else
                {
                    //webViewAccountsActivity.StartActivity(new Intent(webViewAccountsActivity, typeof(NavigationActivity)));
                    //webViewAccountsActivity.Finish();
                }
            }
            else
            {

            }
        }

    }
    //public override bool ShouldOverrideUrlLoading(WebView view, string url)
    //{
    //    view.LoadUrl(url);
    //    return true;
    //}
}
