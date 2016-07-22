using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ContosoCabs.UWP.Auth;
using ContosoCabs.Service;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCabs.UWP.Navigation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserProfile : Page
    {
        private CabsAPI _api;
        private string _token;
        public UserProfile()
        {
            this.InitializeComponent();
            _api = new CabsAPI();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            userContact.Text = localSettings.Values["Mobile"].ToString();
            username.Text = localSettings.Values["Name"].ToString();
            useremail.Text = localSettings.Values["Email"].ToString();
            _token = localSettings.Values["Token"].ToString();
            ShowLoader(true);
            UserResponse res = await _api.GetProfile(_token);
            ShowLoader(false);
            if (res.Code == ResponseCode.SUCCESS)
            {
                if (res.User.Accounts.Count > 0)
                {
                    UberBlock.Text = "Connected";
                }
                else
                {
                    UberBlock.Text = "Not Authenticated";
                }
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

        private void manageAccounts(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ManageAccounts));
        }
        private void logout(object sender, RoutedEventArgs e)
        {
            this.Frame.BackStack.Clear();
            var frame = new Frame();
            frame.Navigate(typeof(MainPage));
            Window.Current.Content = frame;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Mobile"] = null;
            localSettings.Values["Name"] = null;
            localSettings.Values["Email"] = null;
            localSettings.Values["Token"] = null;
            localSettings.Values["LoggedIn"] = null;
            Window.Current.Activate();
        }
        private void ShowLoader(bool show)
        {
            if (show)
            {
                Loader.Visibility = Visibility.Visible;
                TB_Loader.Visibility = Visibility.Visible;
            }
            else
            {
                Loader.Visibility = Visibility.Collapsed;
                TB_Loader.Visibility = Visibility.Collapsed;
            }

        }
    }
}
