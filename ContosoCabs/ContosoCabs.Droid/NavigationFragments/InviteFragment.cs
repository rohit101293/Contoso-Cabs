using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Net;
using ContosoCabs.Droid.Dialogs;

namespace ContosoCabs.Droid.NavigationFragments
{
    public class InviteFragment : Fragment
    {

        private Button invite;
        private NavigationActivity navigationActivity;
        private Typeface typeface;
        private TextView invitetext;
        private ErrorDialog mErrorDialog;

        public InviteFragment(NavigationActivity navigationActivity)
        {
            this.navigationActivity = navigationActivity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_invite_layout, container, false);
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
                SpannableString s = new SpannableString("Invite and Share");
                typeface = Typeface.CreateFromAsset(navigationActivity.Assets, "JosefinSans-SemiBold.ttf");
                s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                s.SetSpan(new ForegroundColorSpan(navigationActivity.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                navigationActivity.TitleFormatted = s;
                invitetext = view.FindViewById<TextView>(Resource.Id.invitetext);
                invitetext.SetTypeface(typeface, TypefaceStyle.Normal);
                invite = view.FindViewById<Button>(Resource.Id.invite);
                invite.SetTypeface(typeface, TypefaceStyle.Normal);
                invite.Click += Invite_Click;
            }
           
        }

        private void Invite_Click(object sender, EventArgs e)
        {
            
            Intent intent = new Intent();
            intent.SetAction(Intent.ActionSend);
            intent.PutExtra(Intent.ExtraText, "http://contosocabs.com");
            intent.SetType("text/plain");
            navigationActivity.StartActivity(intent);
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