
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Net;
using ContosoCabs.Droid.Dialogs;

namespace ContosoCabs.Droid.NavigationFragments
{
    public class FeedbackFragment : Fragment
    {
        private NavigationActivity navigationActivity;
        private Typeface typeface;
        private ErrorDialog mErrorDialog;

        public FeedbackFragment(NavigationActivity navigationActivity)
        {
            this.navigationActivity = navigationActivity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_feedback_layout, container, false);
            init(view);
            return view;
        }

        private void init(View view)
        {
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                SpannableString s = new SpannableString("Feedback");
                typeface = Typeface.CreateFromAsset(navigationActivity.Assets, "JosefinSans-SemiBold.ttf");
                s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                s.SetSpan(new ForegroundColorSpan(navigationActivity.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                navigationActivity.TitleFormatted = s;
                Intent emailIntent = new Intent(Intent.ActionSendto, Android.Net.Uri.FromParts("mailto", "support@contosocabs.com", null));
                emailIntent.PutExtra(Intent.ExtraSubject, "Feedback from User");
                StartActivity(Intent.CreateChooser(emailIntent, "Send Feedback..."));
            }
           
        }
        public bool isOnline()
        {
            ConnectivityManager mConnectivityManager = (ConnectivityManager)navigationActivity.GetSystemService(Context.ConnectivityService);
            NetworkInfo mNetInfo = mConnectivityManager.ActiveNetworkInfo;
            if (mNetInfo != null && mNetInfo.IsConnectedOrConnecting)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}