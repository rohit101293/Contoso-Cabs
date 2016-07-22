using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Service;
using ContosoCabs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCabs.UWP.Auth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OAuthWebViewPage : Page
    {
        
        public OAuthWebViewPage()
        {
            this.InitializeComponent();
            WebViewControl.Navigate(new Uri("https://contosocabs.azurewebsites.net/uber/login"));
        }
        async void WebViewControl_ScriptNotify(object sender, NotifyEventArgs e)
        {
            // Be sure to verify the source of the message when performing actions with the data.
            // As webview can be navigated, you need to check that the message is coming from a page/code
            // that you trust.

            string code = e.Value;
            string token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Token"].ToString();
            CabsAPI api = new CabsAPI();
            SendTokenResponse response = await api.SendToken(token, code);
            if (response.Code == ResponseCode.SUCCESS)
            {
                Frame.BackStack.Clear();
                Frame.Navigate(typeof(Navigation.NavigationPage), "OPEN_PROFILE");
            }
            else
            {
                await new MessageDialog("Cannot Authenticate your account now. Try again later").ShowAsync();
                Frame.BackStack.Clear();
                Frame.Navigate(typeof(Navigation.NavigationPage), "OPEN_PROFILE");
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.Frame.GoBack();
            }
        }
        private void MoreLess_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            var hyperlink = (Hyperlink)sender;
            if (MoreInformationText.Visibility == Visibility.Visible)
            {
                MoreInformationText.Visibility = Visibility.Collapsed;
                MoreLessText.Text = "Show more information";
            }
            else
            {
                MoreInformationText.Visibility = Visibility.Visible;
                MoreLessText.Text = "Show less information";
            }           
        }
    }
}
