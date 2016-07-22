using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Webkit;
using Android.Net;
using ContosoCabs.Droid.Dialogs;

namespace ContosoCabs.Droid.NavigationFragments
{
    public class PaymentFragment : Fragment,View.IOnClickListener
    {
        private NavigationActivity navigationActivity;
        private TextView mContosoText, mWalletAmount, mAddMoney, mSelectMode, mCash, mCard, mWallet;
        private Typeface typeface;
        private ErrorDialog mErrorDialog;

        public PaymentFragment(NavigationActivity navigationActivity)
        {
            this.navigationActivity = navigationActivity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_payment_layout, container, false);
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
                SpannableString s = new SpannableString("Payment Options");
                typeface = Typeface.CreateFromAsset(navigationActivity.Assets, "JosefinSans-SemiBold.ttf");
                s.SetSpan(new TypefaceSpan("Amaranth-Regular.ttf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                s.SetSpan(new ForegroundColorSpan(navigationActivity.Resources.GetColor(Resource.Color.title)), 0, s.Length(), SpanTypes.ExclusiveExclusive);
                navigationActivity.TitleFormatted = s;
                mContosoText = view.FindViewById<TextView>(Resource.Id.contosocabsmoney);
                mWalletAmount = view.FindViewById<TextView>(Resource.Id.price);
                mAddMoney = view.FindViewById<TextView>(Resource.Id.addmoney);
                mSelectMode = view.FindViewById<TextView>(Resource.Id.paymentopttext);
                mCash = view.FindViewById<TextView>(Resource.Id.textcash);
                mCard = view.FindViewById<TextView>(Resource.Id.textcard);
                mWallet = view.FindViewById<TextView>(Resource.Id.textwallet);
                mContosoText.SetTypeface(typeface, TypefaceStyle.Normal);
                mWallet.SetTypeface(typeface, TypefaceStyle.Normal);
                mWalletAmount.SetTypeface(typeface, TypefaceStyle.Normal);
                mAddMoney.SetTypeface(typeface, TypefaceStyle.Normal);
                mCard.SetTypeface(typeface, TypefaceStyle.Normal);
                mCash.SetTypeface(typeface, TypefaceStyle.Normal);
                mSelectMode.SetTypeface(typeface, TypefaceStyle.Normal);
                mAddMoney.SetOnClickListener(this);
            }
          
        }

        public void OnClick(View v)
        {
            switch(v.Id)
            {
                case Resource.Id.addmoney:
                    navigationActivity.StartActivity(new Intent(navigationActivity, typeof(WebViewActivity)));             
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