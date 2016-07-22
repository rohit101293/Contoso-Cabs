
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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ContosoCabs.Service;
using ContosoCabs.ServiceModels;
using ContosoCabs.Utils;
using ContosoCabs.ResponseModels.Auth;
using Windows.Media.SpeechRecognition;
using Windows.Networking.Connectivity;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCabs.UWP.Auth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private SpeechRecognitionResult speechRecognition;
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void Forgot_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameBx.Text.Equals(""))
            {
                await new MessageDialog("Please fill mobile number").ShowAsync();
            }
            else
            {
                var mobile = UsernameBx.Text;
                Frame.Navigate(typeof(Auth.PasswordForgot), mobile);
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
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                if (UsernameBx.Text.Equals("") || PasswordBx.Password.Equals(""))
                {
                    await new MessageDialog("Please fill all the fields").ShowAsync();
                }
                else
                {
                    progress.IsActive = true;
                    CabsAPI api = new CabsAPI();
                    SignInResponse mSignInResponse = await api.LoginUser(UsernameBx.Text, PasswordBx.Password);
                    if (mSignInResponse.Code == ResponseCode.SUCCESS)
                    {
                        // save the session data
                        progress.IsActive = false;
                        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                        localSettings.Values["LoggedIn"] = true;
                        localSettings.Values["Token"] = mSignInResponse.Token;
                        localSettings.Values["Email"] = mSignInResponse.Data.Email;
                        localSettings.Values["Mobile"] = mSignInResponse.Data.Mobile;
                        localSettings.Values["Name"] = mSignInResponse.Data.Name;
                        Frame.Navigate(typeof(Navigation.NavigationPage), speechRecognition);
                    }
                    else if (mSignInResponse.Code == ResponseCode.MYSQL_FIELDS_MISMATCH)
                    {
                        progress.IsActive = false;
                        await new MessageDialog("Password is incorrect. Try Again").ShowAsync();
                    }
                    else if (mSignInResponse.Code == ResponseCode.MYSQL_NO_SUCH_VALUE)
                    {
                        progress.IsActive = false;
                        await new MessageDialog("Account not found. Would you like to register?").ShowAsync();
                        Frame.Navigate(typeof(SignUpPage));
                    }
                    else
                    {
                        progress.IsActive = false;
                        await new MessageDialog("Unfortunately, we can not sign you in at this time").ShowAsync();
                    }
                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            speechRecognition = e.Parameter as SpeechRecognitionResult;
        }

        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
}
