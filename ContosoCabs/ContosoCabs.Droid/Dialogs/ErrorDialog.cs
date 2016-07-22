using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Provider;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace ContosoCabs.Droid.Dialogs
{
    public class ErrorDialog : Dialog, View.IOnClickListener
    {
        private Context mContext;
        private TextView mTextError;
        private Typeface mTypeface;
        private Button mSettingsBtn;
        public ErrorDialog(Context context) : base(context)
        {
            this.mContext = context;
        }

        public void OnClick(View v)
        {
            switch(v.Id)
            {
                case Resource.Id.ok:
                    mContext.StartActivity(new Intent(Settings.ActionSettings));
                    break;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_dialog_error);
            mTextError = FindViewById<TextView>(Resource.Id.errormessage);
            mTypeface = Typeface.CreateFromAsset(mContext.Assets, "JosefinSans-SemiBold.ttf");
            mTextError.SetTypeface(mTypeface,TypefaceStyle.Normal);
            mSettingsBtn = FindViewById<Button>(Resource.Id.ok);
            mSettingsBtn.SetTypeface(mTypeface, TypefaceStyle.Normal);
            mSettingsBtn.SetOnClickListener(this);
        }
    }
}