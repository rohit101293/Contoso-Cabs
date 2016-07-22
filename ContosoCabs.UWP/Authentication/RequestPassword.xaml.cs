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
    
    public sealed partial class NewPassword : Page
    {
        private string mobilenew;
        public NewPassword()
        {
            this.InitializeComponent();

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            mobilenew = e.Parameter as string;

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
                string passtext, cnfpasstext;
                passtext = New.Password;
                cnfpasstext = ConfirmNew.Password;
                if (!checkEmpty(passtext, "New Password"))
                {
                    await new MessageDialog("New password field cannot be empty").ShowAsync();
                    return;
                }
                else if (!checkEmpty(cnfpasstext, "Phone number"))
                {
                    await new MessageDialog("Confirm password field cannot be empty").ShowAsync();
                    return;
                }
                else if (!checkPassValidity(passtext, cnfpasstext))
                {
                    await new MessageDialog("Passwords do not match").ShowAsync();
                    return;
                }
                else if (!isPassValid(passtext))
                {
                    await new MessageDialog("Password must be of atleast 6 digits").ShowAsync();
                    return;
                }
                else
                {
                    progress.IsActive = true;
                    CabsAPI api = new CabsAPI();
                    OtpResponse response = await api.ResetPassword(mobilenew, ConfirmNew.Password);
                    if (response.Code == ResponseCode.SUCCESS)
                    {
                        progress.IsActive = false;
                        await new MessageDialog("Password changed successfully").ShowAsync();
                        Frame.Navigate(typeof(CabsPage));
                    }
                    else
                    {
                        progress.IsActive = false;
                        await new MessageDialog("Unable to update. Try again later").ShowAsync();
                    }
                }
            }
        }

        private bool checkPassValidity(string passtext, string cnfpasstext)
        {
            if (!passtext.Equals(cnfpasstext))
            {
                return false;
            }
            return true;
        }

        private bool isPassValid(string passtext)
        {
            if (passtext.Length > 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool checkEmpty(string nametext, string v)
        {
            if (nametext.Equals(""))
            {
                return false;
            }
            else
            {
                return true;
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
