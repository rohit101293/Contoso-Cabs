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
using System.Text.RegularExpressions;
using ContosoCabs.Utils;
using Android.Speech.Tts;
using ContosoCabs.ResponseModels.Auth;
using Android.Net;

namespace ContosoCabs.Droid.Authentication
{
    [Activity(Label = "signup" ,ParentActivity =typeof(SignInorRegisterActivity))]
    public class SignUpActivity : AppCompatActivity, TextToSpeech.IOnInitListener
    {
        private EditText name;
        private EditText conpass;
        private EditText password;
        private EditText mobile;
        private EditText email;
        private Button signupbtn;
        private CheckBox terms;
        private LoadingDialog mLoadingDialog;
        private TextView pleaseEnter, tandc, nmbrverify;
        private Typeface typeface;
        private ISharedPreferences mSharedPreferences;
        private ISharedPreferencesEditor mEditor;
        private TextToSpeech mTextToSpeech;
        private Context mContext;
        private readonly int MyCheckCode = 101, NeedLang = 103;
        Java.Util.Locale lang;
        private String mSucLog = "Successfuly Signed Up";
        private ErrorDialog mErrorDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_signup);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mErrorDialog = new ErrorDialog(this);
            mErrorDialog.SetCancelable(false);
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                name = FindViewById<EditText>(Resource.Id.siname);
                conpass = FindViewById<EditText>(Resource.Id.siconpass);
                password = FindViewById<EditText>(Resource.Id.sipass);
                mobile = FindViewById<EditText>(Resource.Id.simobile);
                email = FindViewById<EditText>(Resource.Id.siemail);
                signupbtn = FindViewById<Button>(Resource.Id.signupbtn);
                terms = FindViewById<CheckBox>(Resource.Id.term1);
                pleaseEnter = FindViewById<TextView>(Resource.Id.textView1);
                nmbrverify = FindViewById<TextView>(Resource.Id.nmbrverified);
                tandc = FindViewById<TextView>(Resource.Id.term2);
                mLoadingDialog = new LoadingDialog(this, Resource.Drawable.main);
                mLoadingDialog.SetCancelable(false);
                Window window = mLoadingDialog.Window;
                window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
                window.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.trans)));
                SpannableString s = new SpannableString("SignUp");
                typeface = Typeface.CreateFromAsset(this.Assets, "JosefinSans-SemiBold.ttf");
                s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                s.SetSpan(new ForegroundColorSpan(this.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                this.TitleFormatted = s;
                mSharedPreferences = GetSharedPreferences(Constants.MY_PREF, 0);
                mEditor = mSharedPreferences.Edit();
                pleaseEnter.SetTypeface(typeface, TypefaceStyle.Normal);
                nmbrverify.SetTypeface(typeface, TypefaceStyle.Normal);
                tandc.SetTypeface(typeface, TypefaceStyle.Normal);
                signupbtn.SetTypeface(typeface, TypefaceStyle.Normal);
                signupbtn.Click += Signupbtn_Click;
                tandc.Click += Tandc_Click;

            }
        }

        private void Tandc_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("http://contosocabs.azurewebsites.net/tc");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent); 
        }

        private async void Signupbtn_Click(object sender, EventArgs e)
        {
            string nametext, mobiletext, emailtext, passtext, cnfpasstext;
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
            nametext = name.Text;
            mobiletext = mobile.Text;
            emailtext = email.Text;
            passtext = password.Text;
            cnfpasstext = conpass.Text;
            if (!checkEmpty(nametext, "Name"))
                return;
            else if (!checkEmpty(mobiletext, "Phone number"))
                return;
            else if (!checkEmpty(emailtext, "Email"))
                return;
            else if (!checkEmpty(passtext, "Password"))
                return;
            else if (!checkEmpty(cnfpasstext, "Confirm Password"))
                return;
            else
            {
                if (isNameVaid(nametext) && isMobileValid(mobiletext) && isEmailValid(emailtext) && areTermsAccepted() && isPassValid(passtext) && checkPassValidity(passtext, cnfpasstext))
                {
                    mLoadingDialog.Show();
                    CabsAPI api = new CabsAPI();
                    SignupResponse response = await api.RegisterUser(nametext, emailtext, mobiletext, passtext);
                    if (response.Code == Utils.ResponseCode.SUCCESS)
                    {
                        mLoadingDialog.Dismiss();
                        mEditor.PutString("email", emailtext);
                        mEditor.PutString("mobile", mobiletext);
                        mEditor.PutString("name", nametext);
                        mEditor.PutString("token", response.Token);
                        mEditor.PutBoolean("isLoggedIn", true);
                        mEditor.Apply();
                        mTextToSpeech = new TextToSpeech(this, this, "com.google.android.tts");
                        //  new TextToSpeech(con, this, "com.google.android.tts");
                        lang = Java.Util.Locale.Default;
                        //setting language , pitch and speed rate to the voice
                        mTextToSpeech.SetLanguage(lang);
                        mTextToSpeech.SetPitch(1f);
                        mTextToSpeech.SetSpeechRate(1f);
                        mContext = signupbtn.Context;
                        mTextToSpeech.Speak(mSucLog, QueueMode.Flush, null, null);
                        StartActivity(new Intent(this, typeof(NavigationActivity)));
                        
                        Finish();
                    }
                    else if (response.Code == Utils.ResponseCode.MYSQL_DUPLICATES)
                    {
                        mLoadingDialog.Dismiss();
                        Toast.MakeText(this, "User with same number is already present", ToastLength.Short).Show();
                        mobile.Text = "";
                    }
                    else
                    {
                        mLoadingDialog.Dismiss();
                        Toast.MakeText(this, "Server Error Try Again!", ToastLength.Short).Show();
                    }
                }
            }
        }

        private bool checkPassValidity(string passtext, string cnfpasstext)
        {
            if (!passtext.Equals(cnfpasstext))
            {
                Toast.MakeText(ApplicationContext, "Passwords do not match", ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        private bool checkEmpty(string nametext, string v)
        {
            if(nametext.Equals(""))
            {
                Toast.MakeText(ApplicationContext, "Field " + v + " cannot be empty", ToastLength.Short).Show();
                return false ;
            }
            else
            {
                return true;
            }
        }

        private bool isPassValid(string passtext)
        {
            if(passtext.Length>5)
            {
                return true;
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Password must contain at least 6 characters", ToastLength.Short).Show();
                return false;
            }
        }

        private bool areTermsAccepted()
        {
            if(!terms.Checked)
            {
                Toast.MakeText(ApplicationContext, "You have to accept our T&C", ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        private bool isEmailValid(string emailtext)
        {
            string pattern = "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";
            Match match = Regex.Match(emailtext, pattern);
            if(match.Success)
            {
                return true;
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Please enter a valid email", ToastLength.Short).Show();
                return false;
            }      
        }

        private bool isMobileValid(string mobiletext)
        {
            string pattern = "^[0-9]{10}$";
            Match match = Regex.Match(mobiletext, pattern);
            if (match.Success)
            {
                return true;
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Please enter a valid phone number", ToastLength.Short).Show();
                return false;
            }
        }

        private bool isNameVaid(string nametext)
        {
            //string pattern = "";
            //Match match = Regex.Match(nametext, pattern);
            //if (match.Success)
            //{
            //    return true;
            //}
            //else
            //{
            //    Toast.MakeText(ApplicationContext, "Please enter a valid Name", ToastLength.Short).Show();
            //    return false;
            //}
            return true;
        }
        void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
        {
            if (status == OperationResult.Error)
                mTextToSpeech.SetLanguage(Java.Util.Locale.Default);
            if (status == OperationResult.Success)
                mTextToSpeech.SetLanguage(lang);
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