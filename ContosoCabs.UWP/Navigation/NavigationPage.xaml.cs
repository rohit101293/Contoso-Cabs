using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
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
    public sealed partial class NavigationPage : Page
    {
        private SpeechRecognitionResult speechRecognition;
        public NavigationPage()
        {
            this.InitializeComponent();
            MyFrame.Navigate(typeof(ContosoCabs.UWP.Navigation.Intro));

        }

        private void SplitViewButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Hey " + MySplitView.IsPaneOpen);
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            System.Diagnostics.Debug.WriteLine("Done " + MySplitView.IsPaneOpen);
        }

        private void IconListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            var item = IconListBox.SelectedItem as ListBoxItem;
            if (item != null)
            {
                switch (item.Name)
                {
                    case "About":
                        Frame.Navigate(typeof(Intro));
                        break;
                    case "Home":
                        Frame.Navigate(typeof(CabsPage));
                        break;
                    case "Profile":
                        Frame.Navigate(typeof(UserProfile));
                        break;
                    case "Feedback":
                        Frame.Navigate(typeof(Feedback));
                        break;
                    case "Invite":
                        Frame.Navigate(typeof(Invite));
                        break;

                }
            }
        }


        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                string res = e.Parameter as string;
                if (res.Equals("OPEN_PROFILE"))
                {
                    System.Diagnostics.Debug.WriteLine("yes here");
                    var item = new ListBoxItem();
                    item.Name = "Profile";
                    IconListBox.SelectedItem = item;
                    IconListBox_SelectionChanged(null, null);
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("caught ex");
                speechRecognition = e.Parameter as SpeechRecognitionResult;
            }

        }
        
    }
}
