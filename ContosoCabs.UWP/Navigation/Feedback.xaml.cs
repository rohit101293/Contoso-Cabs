using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCabs.UWP.Navigation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Feedback : Page
    {
        
        private DataTransferManager dataTransferManager;
        public Feedback()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if(this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                this.Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }
        private async void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (!IsInternet())
            {
                await new MessageDialog("Seems you are not connected to the Internet").ShowAsync();
                return;
            }
            else
            {
                Uri dataPackageUri = new Uri("http://contosocabs.com");
                DataPackage requestData = e.Request.Data;
                requestData.Properties.Title = "We value your feedback!";
                requestData.SetWebLink(dataPackageUri);
                requestData.Properties.Description = "Email us at support@contosocabs.com!";
                requestData.Properties.EnterpriseId = "support@contosocabs.com";
            }
        }
        private void Share(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
}
