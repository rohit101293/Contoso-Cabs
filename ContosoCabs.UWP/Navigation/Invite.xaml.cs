using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
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

namespace ContosoCabs.UWP.Navigation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Invite : Page
    {
        private DataTransferManager dataTransferManager;
        public Invite()
        {
            this.InitializeComponent();
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
                requestData.Properties.Title = "Try Contoso Cabs! The best way to find cabs around you!";
                requestData.SetWebLink(dataPackageUri);
                requestData.Properties.Description = "Contoso cabs...the easiest way to find cabs!";
            }
        }
        private void jbskj(object sender, RoutedEventArgs e)
        {
           DataTransferManager.ShowShareUI();
        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.Frame.GoBack();
            }
        }
    }
}
