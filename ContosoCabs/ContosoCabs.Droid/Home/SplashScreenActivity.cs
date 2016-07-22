
using Android.App;
using Android.Content;
using Android.OS;
using ContosoCabs.Utils;
using ContosoCabs.Droid.NavigationFragments;
using ContosoCabs.Droid.Authentication;
using Android.Net;
using ContosoCabs.Droid.Dialogs;

namespace ContosoCabs.Droid.Home
{
    [Activity(Label = "Contoso Cabs", MainLauncher =true)]
    public class SplashScreenActivity : Activity
    {

        private ISharedPreferences mSharedPreferences;
        private ErrorDialog mErrorDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView (Resource.Layout.activity_splash);
            mSharedPreferences = GetSharedPreferences(Constants.MY_PREF, 0);
            mErrorDialog = new ErrorDialog(this);
            mErrorDialog.SetCancelable(false);
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                if (mSharedPreferences.GetBoolean("isLoggedIn", false))
                {
                    StartActivity(new Intent(this, typeof(GettingStartedActivity)));
                    Finish();
                }
                else
                {
                    StartActivity(new Intent(this, typeof(SignInorRegisterActivity)));
                    Finish();
                }
            }
        }
        public bool isOnline()
        {
            ConnectivityManager mConnectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            NetworkInfo mNetInfo = mConnectivityManager.ActiveNetworkInfo;
            if(mNetInfo != null && mNetInfo.IsConnectedOrConnecting)
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