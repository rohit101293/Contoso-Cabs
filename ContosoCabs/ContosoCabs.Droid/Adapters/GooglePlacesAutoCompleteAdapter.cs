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

namespace ContosoCabs.Droid.Adapters
{
    public class GooglePlacesAutoCompleteAdapter
    {
        private NavigationActivity mActivity;
        private MapViewFragment mapViewFragment;

        public GooglePlacesAutoCompleteAdapter(NavigationActivity mActivity, MapViewFragment mapViewFragment)
        {
            this.mActivity = mActivity;
            this.mapViewFragment = mapViewFragment;
        }
    }
}