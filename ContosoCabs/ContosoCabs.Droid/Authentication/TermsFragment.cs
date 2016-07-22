using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace ContosoCabs.Droid.Authentication
{

    public class TermsFragment : DialogFragment
    {

        private Button back;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_terms, container, false);
            back = view.FindViewById<Button>(Resource.Id.back);
            return view;
        }

        private void Signin_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }
    }
}