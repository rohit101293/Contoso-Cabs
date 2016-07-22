
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace ContosoCabs.Droid.Dialogs
{
    public class OTPDialogFragment : DialogFragment,IDialogInterfaceOnClickListener
    {
        private EditText otpValue;
        public interface OtpDialogListener
        {
            void onDialogPositiveClick(DialogFragment dialog, string otpEntered);
        }
        OtpDialogListener otpDialogListener;
        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            try
            {
                otpDialogListener = (OtpDialogListener)activity;
            }
            catch(ClassCastException cl)
            {
                throw new ClassCastException(activity.ToString() + " must implement OtpDialogListener");
            }
        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            LayoutInflater inflater = Activity.LayoutInflater;
            View v = inflater.Inflate(Resource.Layout.layout_dialog_otp, null);
            otpValue = (EditText)v.FindViewById(Resource.Id.otprec);
            builder.SetView(v).SetPositiveButton("Verify", this);
            return builder.Create();
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            otpDialogListener.onDialogPositiveClick(this, otpValue.Text);
        }
    }
}
