using ContosoCabs.ResponseModels.Auth;
using ContosoCabs.Service;
using ContosoCabs.ServiceModels;
using ContosoCabs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCabs.UWP.Auth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PasswordForgot : Page
    {

        public string otpEntered;
        public string otpGot;
        private string _mobile;
        public PasswordForgot()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _mobile = e.Parameter as string;

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.Frame.GoBack();
            }
        }
        public async void OTP_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                progress.IsActive = true;
                CabsAPI api = new CabsAPI();
                OtpResponse otpResponse = await api.GetOTP(_mobile);
                if(otpResponse.Code == ResponseCode.SUCCESS)
                {
                    progress.IsActive = false;
                    otpGot = otpResponse.Otp;
                }
                else
                {
                    await new MessageDialog("Server error!").ShowAsync();
                    return;
                }
            }
        }

        private async void CheckOTP(object sender, RoutedEventArgs e)
        {
            progress.IsActive = true;
            otpEntered = OtpBx.Text;
            if (otpEntered.Equals(otpGot))
            {
                progress.IsActive = false;
                Frame.Navigate(typeof(NewPassword),_mobile);
            }
            else
            {
                progress.IsActive = false;
                await new MessageDialog("OTP entered is incorrect.Try again").ShowAsync();
                return;
            }
        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
  }
