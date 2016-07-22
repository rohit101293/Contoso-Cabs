using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using ContosoCabs.Service;
using ContosoCabs.ServiceModels;
using ContosoCabs.Droid.NavigationFragments;
using Android.Support.V7.App;
using ContosoCabs.Droid.Dialogs;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Graphics;
using Android.Text.Style;
using Android.Views.InputMethods;
using ContosoCabs.Utils;
using ContosoCabs.ResponseModels.Auth;
using ContosoCabs.Droid.Home;
using Android.Net;

namespace ContosoCabs.Droid.Authentication
{
    [Activity(Label = "SignIn", ParentActivity = (typeof(SignInorRegisterActivity)))]
    public class SignInActivity : AppCompatActivity, OTPDialogFragment.OtpDialogListener
    {

        private TextView forgot;
        private TextView password;
        private Button signin;
        private EditText mobile;
        private LoadingDialog mLoadingDialog;
        private Typeface typeface;
        private TextView header;
        private string otpRecieved;
        private ISharedPreferences mSharedPreferences;
        private ISharedPreferencesEditor mEditor;
        private ErrorDialog mErrorDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mErrorDialog = new ErrorDialog(this);
            mErrorDialog.SetCancelable(false);
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                mLoadingDialog = new LoadingDialog(this, Resource.Drawable.main);
                mLoadingDialog.SetCancelable(false);
                Window window = mLoadingDialog.Window;
                window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
                window.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.trans)));
                SpannableString s = new SpannableString("SignIn");
                typeface = Typeface.CreateFromAsset(this.Assets, "JosefinSans-SemiBold.ttf");
                s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                s.SetSpan(new ForegroundColorSpan(this.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                this.TitleFormatted = s;
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SetContentView(Resource.Layout.activity_signin);
                mSharedPreferences = GetSharedPreferences(Constants.MY_PREF, 0);
                mEditor = mSharedPreferences.Edit();
                mobile = FindViewById<EditText>(Resource.Id.mobile);
                password = FindViewById<EditText>(Resource.Id.password);
                forgot = FindViewById<TextView>(Resource.Id.forgot);
                header = FindViewById<TextView>(Resource.Id.headersignin);
                header.SetTypeface(typeface, TypefaceStyle.Normal);
                forgot.SetTypeface(typeface, TypefaceStyle.Normal);
                forgot.Click += Forgot_Click;
                signin = FindViewById<Button>(Resource.Id.signin);
                signin.SetTypeface(typeface, TypefaceStyle.Normal);
                signin.Click += Signin_Click;
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
        private async void Signin_Click(object sender, EventArgs e)
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
            string phonenum = mobile.Text;
            string pass = password.Text;
            if (pass.Length > 5)
            {
                if (phonenum.Length == 10)
                {
                    mLoadingDialog.Show();
                    CabsAPI api = new CabsAPI();
                    SignInResponse response = await api.LoginUser(phonenum, pass);
                    if (response.Code == ResponseCode.SUCCESS)
                    {
                        mLoadingDialog.Dismiss();
                        mEditor.PutString("email", response.Data.Email);
                        mEditor.PutString("mobile", response.Data.Mobile);
                        mEditor.PutString("name", response.Data.Name);
                        mEditor.PutString("token", response.Token);
                        mEditor.PutBoolean("isLoggedIn", true);
                        mEditor.Apply();
                        StartActivity(new Intent(this, typeof(GettingStartedActivity)));
                        Finish();
                    }
                    else
                    {
                        mLoadingDialog.Dismiss();
                        Toast.MakeText(this, "Incorrect Credentials", ToastLength.Short).Show();
                        return;
                    }
                }
                else
                {
                    Toast.MakeText(this, "Phone number must be of 10 digits", ToastLength.Short).Show();
                    return;
                }
            }
            else
            {
                Toast.MakeText(this, "Password must have atleast 6 digits", ToastLength.Short).Show();
                return;
            }

        }


        private async void Forgot_Click(object sender, EventArgs e)
        {
            if (mobile.Text.Equals("") || mobile.Text.Length != 10)
            {
                Toast.MakeText(ApplicationContext, "Please enter a 10 digit mobile number", ToastLength.Short).Show();
                return;
            }
            else
            {
                mLoadingDialog.Show();
                CabsAPI api = new CabsAPI();
                OtpResponse response = await api.GetOTP(mobile.Text);
                if (response.Code == ResponseCode.SUCCESS)
                {
                    mLoadingDialog.Dismiss();
                    otpRecieved = response.Otp;
                    showOtpDialog();
                }
                else
                {

                }

            }
            //    FragmentTransaction tra = FragmentManager.BeginTransaction();
            //    ForgotPasswordFragment dia = new ForgotPasswordFragment();
            //    dia.Show(tra, "dialog");
            //    dia.marg += Dia_marg;
        }

        private void showOtpDialog()
        {
            DialogFragment dialogFragment = new OTPDialogFragment();
            dialogFragment.Cancelable = false;
            dialogFragment.Show(FragmentManager, "OTP Verification");
        }

        public void onDialogPositiveClick(DialogFragment dialog, string otpEntered)
        {
            if(otpEntered.Equals(otpRecieved))
            {
                Intent intent = new Intent(this, typeof(RequestPasswordActivity));
                intent.PutExtra("mobile", mobile.Text);
                StartActivity(intent);
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Invalid OTP", ToastLength.Short).Show();
                return;
            }
        }
    }
}