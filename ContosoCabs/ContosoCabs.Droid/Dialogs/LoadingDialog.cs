
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace ContosoCabs.Droid.Dialogs
{
    public class LoadingDialog : Dialog
    {
        private Activity a;
        private int res;
        private ProgressBar mProgressbar;
        private ImageView mImageView;
        public LoadingDialog(Activity context,int res):base(context)
        {
            this.a = context;
            this.res = res;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.custom_progress_dialog);
            mProgressbar = FindViewById<ProgressBar>(Resource.Id.progressloader);
            mImageView = FindViewById<ImageView>(Resource.Id.loadingdailogimage);
            mImageView.SetImageResource(res);
            mProgressbar.Visibility = ViewStates.Visible;
        }
    }
}