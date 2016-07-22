
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Text;
using Android.Graphics;
using Android.Text.Style;
using ContosoCabs.Utils;
using ContosoCabs.Droid.Home;
using ContosoCabs.Droid.Dialogs;
using Android.Graphics.Drawables;
using ContosoCabs.Service;
using ContosoCabs.ResponseModels.Private;
using Android.Net;

namespace ContosoCabs.Droid.NavigationFragments
{
    public class ProfileFragment : Fragment,View.IOnClickListener
    {
        private NavigationActivity navigationActivity;
        private Typeface typeface;
        private TextView phone, email, emailtext, phonetext, invite, contosomoney, moneyamnt, dispcontact, mIdtext1, mIdtext2;
        private Button mbutton, mAddAccount;
        private LinearLayout mLinear1, mLinear2, mLinearempty;
        private Fragment mFragment;
        private ISharedPreferences mSharedPreferences;
        private string namerec, emailrec, mobilerec;
        private LoadingDialog mLoadingDialog;
        private CabsAPI mCabsApi;
        private string token;
        private ErrorDialog mErrorDialog;

        public ProfileFragment(NavigationActivity navigationActivity)
        {
            this.navigationActivity = navigationActivity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_profile_layout, container, false);
            init(view);
            return view;
        }

        private async void init(View view)
        {
            if(!isOnline())
            {
                mErrorDialog.Show();
            }
            else
            {
                mLoadingDialog = new LoadingDialog(navigationActivity, Resource.Drawable.main);
                mLoadingDialog.SetCancelable(false);
                Window window = mLoadingDialog.Window;
                mCabsApi = new CabsAPI();
                window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
                window.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.trans)));
                phone = view.FindViewById<TextView>(Resource.Id.phonetobeset);
                phonetext = view.FindViewById<TextView>(Resource.Id.disp1);
                email = view.FindViewById<TextView>(Resource.Id.emailtobeset);
                emailtext = view.FindViewById<TextView>(Resource.Id.disp2);
                invite = view.FindViewById<TextView>(Resource.Id.inv);
                contosomoney = view.FindViewById<TextView>(Resource.Id.contosomoney);
                moneyamnt = view.FindViewById<TextView>(Resource.Id.price);
                mbutton = view.FindViewById<Button>(Resource.Id.logout);
                dispcontact = view.FindViewById<TextView>(Resource.Id.dispcontact);
                mIdtext1 = view.FindViewById<TextView>(Resource.Id.idtext1);
                mIdtext2 = view.FindViewById<TextView>(Resource.Id.idtext2);
                mAddAccount = view.FindViewById<Button>(Resource.Id.addAcount);
                mLinear1 = view.FindViewById<LinearLayout>(Resource.Id.linear1);
                mLinear2 = view.FindViewById<LinearLayout>(Resource.Id.linear2);
                mLinearempty = view.FindViewById<LinearLayout>(Resource.Id.whenempty);
                mSharedPreferences = Activity.GetSharedPreferences(Constants.MY_PREF, 0);
                namerec = mSharedPreferences.GetString("name", " ");
                mobilerec = mSharedPreferences.GetString("mobile", " ");
                emailrec = mSharedPreferences.GetString("email", " ");
                token = mSharedPreferences.GetString("token", " ");
                email.Text = emailrec;
                phone.Text = mobilerec;
                namerec += " ";
                if (!mSharedPreferences.GetBoolean("hasAccounts", false))
                {
                    mLinear1.Visibility = ViewStates.Visible;
                    mLinear2.Visibility = ViewStates.Visible;
                    mIdtext1.Text = "Not Authorized";
                    mIdtext2.Text = "Not Authorized";
                    mLinearempty.Visibility = ViewStates.Gone;
                }
                else
                {
                    mLinearempty.Visibility = ViewStates.Gone;
                    mLinear2.Visibility = ViewStates.Visible;
                    mLinear1.Visibility = ViewStates.Visible;
                    mLoadingDialog.Show();
                    mIdtext2.Text = "Not Authorized";
                    mIdtext1.Text = "Not Authorized";
                    UserResponse mRes = await mCabsApi.GetProfile(token);
                    System.Diagnostics.Debug.WriteLine("In profile resp code " + mRes.Code);
                    if (mRes.Code == ResponseCode.SUCCESS)
                    {
                        mLoadingDialog.Dismiss();
                        mIdtext1.Text = mRes.User.Accounts.Count > 0 ? "Connected" : "Not Authorized";
                    }
                }
                SpannableString s = new SpannableString(namerec.Substring(0, namerec.IndexOf(' ')));
                typeface = Typeface.CreateFromAsset(navigationActivity.Assets, "JosefinSans-SemiBold.ttf");
                s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                s.SetSpan(new ForegroundColorSpan(navigationActivity.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                navigationActivity.TitleFormatted = s;
                phone.SetTypeface(typeface, TypefaceStyle.Normal);
                phonetext.SetTypeface(typeface, TypefaceStyle.Normal);
                email.SetTypeface(typeface, TypefaceStyle.Normal);
                emailtext.SetTypeface(typeface, TypefaceStyle.Normal);
                invite.SetTypeface(typeface, TypefaceStyle.Normal);
                mbutton.SetTypeface(typeface, TypefaceStyle.Normal);
                contosomoney.SetTypeface(typeface, TypefaceStyle.Normal);
                dispcontact.SetTypeface(typeface, TypefaceStyle.Normal);
                mAddAccount.SetTypeface(typeface, TypefaceStyle.Normal);
                mAddAccount.SetOnClickListener(this);
                invite.SetOnClickListener(this);
                mbutton.SetOnClickListener(this);
            }

        }
        public void OnClick(View v)
        {
            switch(v.Id)
            {
                case Resource.Id.inv:
                    mFragment = new InviteFragment(navigationActivity);
                    navigationActivity.FragmentManager.BeginTransaction().Replace(Resource.Id.nav_frame, mFragment).Commit();
                    break;
                case Resource.Id.logout:
                    mSharedPreferences.Edit().Remove("email").Apply();
                    mSharedPreferences.Edit().Remove("mobile").Apply();
                    mSharedPreferences.Edit().Remove("name").Apply();
                    mSharedPreferences.Edit().Remove("token").Apply();
                    mSharedPreferences.Edit().Remove("isLoggedIn").Apply();
                    mSharedPreferences.Edit().Remove("hasAccounts").Apply();
                    StartActivity(new Intent(navigationActivity, typeof(SplashScreenActivity)));
                    navigationActivity.Finish();
                    break;
                case Resource.Id.addAcount:
                    ProviderDialog mProviderDialog = new ProviderDialog(navigationActivity);
                    mProviderDialog.Show();
                    break;
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