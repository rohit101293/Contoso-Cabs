using System;
using Windows.UI.Xaml;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ContosoCabs.Service;
using ContosoCabs.Utils;
using ContosoCabs.ResponseModels.Auth;
using Windows.Media.SpeechRecognition;
using Windows.Networking.Connectivity;
using System.Text.RegularExpressions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCabs.UWP.Auth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignUpPage : Page
    {
        private SpeechRecognitionResult speechRecognition;
        public SignUpPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.Frame.GoBack();
            }
        }
        private async void Signup_Click(object sender, RoutedEventArgs e)
        {

            //Validation of entries in the fields
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                string nametext, mobiletext, emailtext, passtext, cnfpasstext;
                nametext = UsernameBox.Text;
                mobiletext = ContactNumberBox.Text;
                emailtext = EmailidBox.Text;
                passtext = PasswordBox.Password;
                cnfpasstext = ConfirmPasswordBox.Password;
                if (!checkEmpty(nametext, "Name"))
                {
                    await new MessageDialog("Name field cannot be empty").ShowAsync();
                    return;
                }
                else if (!checkEmpty(mobiletext, "Phone number"))
                {
                    await new MessageDialog("Phone field cannot be empty").ShowAsync();
                    return;
                }
                else if (!checkEmpty(emailtext, "Email"))
                {
                    await new MessageDialog("Email field cannot be empty").ShowAsync();
                    return;
                }
                else if (!checkEmpty(passtext, "Password"))
                {
                    await new MessageDialog("Password field cannot be empty").ShowAsync();
                    return;
                }
                else if (!checkEmpty(cnfpasstext, "Confirm Password"))
                {
                    await new MessageDialog("Confirm Password field cannot be empty").ShowAsync();
                    return;
                }
                else
                {
                    if (!isNameVaid(nametext))
                    {
                        await new MessageDialog("Please enter a valid Name").ShowAsync();
                        return;
                    }
                    else if(!isPassValid(passtext))
                    {
                        await new MessageDialog("Password must contain at least 6 characters").ShowAsync();
                        return;
                    }
                    else if(!isEmailValid(emailtext))
                    {
                        await new MessageDialog("Please enter a valid email").ShowAsync();
                        return;
                    }
                    else if(!isMobileValid(mobiletext))
                    {
                        await new MessageDialog("Please enter a valid 10 digit mobile number").ShowAsync();
                        return;
                    }
                    else if(!checkPassValidity(passtext,cnfpasstext))
                    {
                        await new MessageDialog("Passwords do not match").ShowAsync();
                        return;
                    }
                    else if(!areTermsAccepted())
                    {
                        await new MessageDialog("You have to accept our T&C").ShowAsync();
                        return;
                    }
                    else
                    {
                        progress.IsActive = true;
                        CabsAPI api = new CabsAPI();
                        SignupResponse response = await api.RegisterUser(UsernameBox.Text, EmailidBox.Text, ContactNumberBox.Text, PasswordBox.Password);
                        if (response.Code == ResponseCode.SUCCESS)
                        {
                            progress.IsActive = false;
                            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                            localSettings.Values["LoggedIn"] = true;
                            localSettings.Values["Token"] = response.Token;
                            localSettings.Values["Email"] = EmailidBox.Text;
                            localSettings.Values["Mobile"] = ContactNumberBox.Text;
                            localSettings.Values["Name"] = UsernameBox.Text;
                            Frame.Navigate(typeof(Navigation.NavigationPage), speechRecognition);
                        }
                        else if (response.Code == ResponseCode.MYSQL_DUPLICATES)
                        {
                            progress.IsActive = false;
                            await new MessageDialog("Email or Contact Number already exists. Please try again").ShowAsync();
                        }
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

        private bool areTermsAccepted()
        {
            if (!(condition.IsChecked == true))
            {
                return false;
            }
            return true;
        }

        private bool isEmailValid(string emailtext)
        {
            string pattern = "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";
            Match match = Regex.Match(emailtext, pattern);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isMobileValid(string mobiletext)
        {
            string pattern = "^[0-9]{10}$";
            Match match = Regex.Match(mobiletext, pattern);
            if (match.Success && mobiletext.Length==10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isNameVaid(string nametext)
        {
            return true;
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
