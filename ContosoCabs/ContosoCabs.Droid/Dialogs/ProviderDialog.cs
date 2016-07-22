using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ContosoCabs.Droid.NavigationFragments;

namespace ContosoCabs.Droid.Dialogs
{
    public class ProviderDialog : Dialog, View.IOnClickListener
    {
        private Context mContext;
        private RadioButton mUber;
        private RadioButton mOla;
        private TextView mTextView;
        private Intent mIntent;
        public ProviderDialog(Context context) : base(context)
        {
            this.mContext = context;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_dialog_select_provider);
            mTextView = FindViewById<TextView>(Resource.Id.errormessage);
            mOla = FindViewById<RadioButton>(Resource.Id.olaradio);
            mUber = FindViewById<RadioButton>(Resource.Id.uberradio);
            mOla.SetOnClickListener(this);
            mUber.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            switch(v.Id)
            {
                case Resource.Id.olaradio:
                    mIntent = new Intent(mContext, typeof(WebViewAccountsActivity));
                    mIntent.PutExtra("provider", "ola");
                    Dismiss();
                    mContext.StartActivity(mIntent);
                    break;
                case Resource.Id.uberradio:
                    mIntent = new Intent(mContext, typeof(WebViewAccountsActivity));
                    mIntent.PutExtra("provider", "uber");
                    Dismiss();
                    mContext.StartActivity(mIntent);
                    break;
            }
        }
    }
}