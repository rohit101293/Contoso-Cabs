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
using ContosoCabs.Models;
using ContosoCabs.Droid.NavigationFragments;
using Android.Graphics;
using Android.Support.V7.Widget;
using UniversalImageLoader.Core;
using ContosoCabs.ServiceModels;

namespace ContosoCabs.Droid.Adapters
{
    public class CabRecyclerAdapterWithDestination : RecyclerView.Adapter
    {
        public List<CabEstimate> mDataItems;
        public Context mContext;
        public CabEstimate mCabDetails;
        private NavigationActivity mActivity;
        private Typeface typeface;
        public event EventHandler<int> ItemClick1;
        public CabRecyclerAdapterWithDestination(Context context, List<CabEstimate> listItems, NavigationActivity activity)
        {
            this.mContext = context;
            this.mDataItems = listItems;
            this.mActivity = activity;
        }

        public override int ItemCount
        {
            get
            {
                return (null != mDataItems) ? mDataItems.Count : 0;
            }
        }
        public void OnClick(int position)
        {
            if (ItemClick1 != null)
            {
                ItemClick1(this, position);

            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListRowViewHoldernew mHolder = holder as ListRowViewHoldernew;
            mCabDetails = mDataItems[position];
            mHolder.provider.Text = mCabDetails.Provider;
            mHolder.eta.Text = mCabDetails.Eta;
            mHolder.type.Text = mCabDetails.Type;
            mHolder.basefare.Text = mCabDetails.CurrentEstimate.FareData.BaseFare;
            mHolder.freefare.Text = "0";
            mHolder.cstperkm.Text = mCabDetails.CurrentEstimate.FareData.CostPerKilometer;
            mHolder.cstpermin.Text = mCabDetails.CurrentEstimate.FareData.CostPerMinute;
            mHolder.surge.Text = mCabDetails.CurrentEstimate.FareData.Surge;
            ImageLoader.Instance.DisplayImage(mCabDetails.ImageURL, mHolder.imageview);
            mHolder.imageview.SetImageResource(Resource.Drawable.Icon);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From(mContext).Inflate(Resource.Layout.layout_cabview, parent, false);
            typeface = Typeface.CreateFromAsset(mActivity.Assets, "JosefinSans-SemiBold.ttf");
            TextView provider = v.FindViewById<TextView>(Resource.Id.provider);
            provider.SetTypeface(typeface, TypefaceStyle.Normal);
            TextView type = v.FindViewById<TextView>(Resource.Id.type);
            type.SetTypeface(typeface, TypefaceStyle.Normal);
            TextView eta = v.FindViewById<TextView>(Resource.Id.eta);
            eta.SetTypeface(typeface, TypefaceStyle.Normal);
            TextView basefare = v.FindViewById<TextView>(Resource.Id.basefare);
            basefare.SetTypeface(typeface, TypefaceStyle.Normal);
            TextView freefare = v.FindViewById<TextView>(Resource.Id.nooffree);
            freefare.SetTypeface(typeface, TypefaceStyle.Normal);
            TextView cstperkm = v.FindViewById<TextView>(Resource.Id.costperkm);
            cstperkm.SetTypeface(typeface, TypefaceStyle.Normal);
            TextView cstpermin = v.FindViewById<TextView>(Resource.Id.costpermin);
            cstpermin.SetTypeface(typeface, TypefaceStyle.Normal);
            TextView surge = v.FindViewById<TextView>(Resource.Id.surge);
            surge.SetTypeface(typeface, TypefaceStyle.Normal);
            ImageView imageView = v.FindViewById<ImageView>(Resource.Id.imageView);
            CardView myView = v.FindViewById<CardView>(Resource.Id.mCardView);
            ListRowViewHoldernew holder = new ListRowViewHoldernew(v, OnClick) { mCardView = myView, provider = provider, type = type, eta = eta, basefare = basefare, freefare = freefare, cstperkm = cstperkm, cstpermin = cstpermin, surge = surge, imageview = imageView };
            return holder;
        }
    }
    public class ListRowViewHoldernew : RecyclerView.ViewHolder
    {
        public Cab mCabs;
        public CardView mCardView { get; set; }
        public View myView { get; set; }
        public TextView provider { get; set; }
        public TextView type { get; set; }
        public TextView eta { get; set; }
        public TextView basefare { get; set; }
        public TextView freefare { get; set; }
        public TextView cstperkm { get; set; }
        public TextView cstpermin { get; set; }
        public TextView surge { get; set; }
        public ImageView imageview { get; set; }

        public ListRowViewHoldernew(View itemView, Action<int> listenernew) : base(itemView)
        {
            myView = itemView;
            itemView.Click += (sender, e) => listenernew(base.Position);
        }
    }
}
