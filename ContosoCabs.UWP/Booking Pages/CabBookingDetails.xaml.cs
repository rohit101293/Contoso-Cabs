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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCabs.UWP.Home
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CabBookingDetails : Page
    {
        public CabBookingDetails()
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            Dictionary<string, object> Parameters = e.Parameter as Dictionary<string, object>;
            PriceData.Text = Parameters["price"].ToString();
            DistanceData.Text = Parameters["distance"].ToString();
            SrcBox.Text = Parameters["source"].ToString();
            DestBox.Text = Parameters["destination"].ToString();
            TimeData.Text = Parameters["time"].ToString();
            DriverData.Text = Parameters["driverName"].ToString();
            DriverContact.Text = Parameters["driverContact"].ToString();
            var source = Parameters["driverPic"].ToString();
            carmake.Text = Parameters["carMake"].ToString();
            carmodel.Text = Parameters["carModel"].ToString();
            carnumber.Text = Parameters["carNumber"].ToString();
            Uri uri = new Uri(source, UriKind.Absolute);
            ImageSource imgSource = new BitmapImage(uri);
            provider.Source = imgSource;
            //carplate.Text = Parameters["carNumber"].ToString();
        }

        private async void Call_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await new MessageDialog("Make a call to " + carnumber.Text).ShowAsync();

        }
    }
    
}
