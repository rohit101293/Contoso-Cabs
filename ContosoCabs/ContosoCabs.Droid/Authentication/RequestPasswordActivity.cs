
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Graphics;
using Android.Text.Style;
using Android.Views.InputMethods;
using ContosoCabs.Service;
using ContosoCabs.ServiceModels;
using ContosoCabs.Utils;
using ContosoCabs.ResponseModels.Auth;
using Android.Net;
using ContosoCabs.Droid.Dialogs;

namespace ContosoCabs.Droid.Authentication
{
    [Activity(Label = "RequestPassword",ParentActivity =typeof(SignInActivity))]
    public class RequestPasswordActivity : AppCompatActivity, View.IOnClickListener
    {

        private TextView textView;
        private EditText newPassword, confirmNewPassword;
        private ProgressBar mProgressbar;
        private Button mResetButtton;
        private Typeface typeface;
        private ErrorDialog mErrorDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_reset_password);
            init();
        }

        private void init()
        {
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                SpannableString s = new SpannableString("Reset Password");
                typeface = Typeface.CreateFromAsset(this.Assets, "JosefinSans-SemiBold.ttf");
                s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                s.SetSpan(new ForegroundColorSpan(this.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                this.TitleFormatted = s;
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                textView = FindViewById<TextView>(Resource.Id.enterreset);
                newPassword = FindViewById<EditText>(Resource.Id.newPasswordEntered);
                mProgressbar = FindViewById<ProgressBar>(Resource.Id.progress);
                confirmNewPassword = FindViewById<EditText>(Resource.Id.confirmNewPasswordEntered);
                mResetButtton = FindViewById<Button>(Resource.Id.resetPasswordButton);
                mResetButtton.SetTypeface(typeface, TypefaceStyle.Normal);
                textView.SetTypeface(typeface, TypefaceStyle.Normal);
                mResetButtton.SetOnClickListener(this);
            }
           
        }

        public async void OnClick(View v)
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
            if (newPassword.Text.Length < 6 || confirmNewPassword.Text.Length < 6)
            {
                Toast.MakeText(ApplicationContext, "The password should be atleast 6 digits in length", ToastLength.Short).Show();
                return;
            }
            else
            {
                if (valid())
                {
                    mProgressbar.Visibility = ViewStates.Visible;
                    string mobile = Intent.GetStringExtra("mobile");
                    string password = newPassword.Text;
                    CabsAPI api = new CabsAPI();
                    OtpResponse response = await api.ResetPassword(mobile, password);
                    if(response.Code == ResponseCode.SUCCESS)
                    {
                        mProgressbar.Visibility = ViewStates.Gone;
                        Toast.MakeText(ApplicationContext, "Password Reset Successful", ToastLength.Short).Show();
                        StartActivity(new Intent(this, typeof(SignInActivity)));
                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(ApplicationContext, "Server Error!", ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }

        private bool valid()
        {
            return newPassword.Text.Equals(confirmNewPassword.Text);
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