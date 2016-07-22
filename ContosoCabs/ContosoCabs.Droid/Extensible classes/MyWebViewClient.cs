using Android.Net;
using Android.Webkit;
using ContosoCabs.Droid.NavigationFragments;

namespace ContosoCabs.Droid.Extensible_classes
{
    public class MyWebViewClient : WebViewClient
    {
        private WebViewActivity webViewActivity;

        public MyWebViewClient(WebViewActivity webViewActivity)
        {
            this.webViewActivity = webViewActivity;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            view.LoadUrl(url);
            return true;
        }
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            if(url.Contains("last_html"))
            {
                webViewActivity.Finish();
            }
        }
    }
}