using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Graphics;
using Android.Text.Style;
using ContosoCabs.Droid.Dialogs;
using Android.Net;

namespace ContosoCabs.Droid.Authentication
{
    [Activity(Label = "registerhome")]
    public class SignInorRegisterActivity : AppCompatActivity
    {
        private Button btnsignup;
        private Button btnlogin;
        private Typeface typeface;
        private TextView contosotext, copyright;
        private ErrorDialog mErrorDialog;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            mErrorDialog = new ErrorDialog(this);
            SpannableString s = new SpannableString("SignIn or Register");
            typeface = Typeface.CreateFromAsset(this.Assets, "JosefinSans-SemiBold.ttf");
            s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
            s.SetSpan(new ForegroundColorSpan(this.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
            this.TitleFormatted = s;
            SetContentView(Resource.Layout.activity_signin_or_register);
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                contosotext = FindViewById<TextView>(Resource.Id.contosocabheader);
                copyright = FindViewById<TextView>(Resource.Id.copyright);
                contosotext.SetTypeface(typeface, TypefaceStyle.Bold);
                copyright.SetTypeface(typeface, TypefaceStyle.Normal);
                btnsignup = FindViewById<Button>(Resource.Id.signup);
                btnsignup.SetTypeface(typeface, TypefaceStyle.Normal);
                btnsignup.Click += (object sender, EventArgs args) =>
                {
                    Intent intent = new Intent(this, typeof(SignUpActivity));
                    StartActivity(intent);
                };
                btnlogin = FindViewById<Button>(Resource.Id.login);
                btnlogin.SetTypeface(typeface, TypefaceStyle.Normal);
                btnlogin.Click += (object sender, EventArgs args) =>
                {
                    Intent intent = new Intent(this, typeof(SignInActivity));
                    StartActivity(intent);
                };
            }
        }
        public bool isOnline()
        {
            ConnectivityManager mConnectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
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