using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using ContosoCabs.Models;
using UniversalImageLoader.Core;
using System;
using ContosoCabs.Droid.Confirm_Booking;
using ContosoCabs.Droid.NavigationFragments;
using Android.Graphics;
using ContosoCabs.ServiceModels;
using Android.OS;

namespace ContosoCabs.Droid.Adapters
{
    public class CabRecyclerAdapter : RecyclerView.Adapter
    {
        public List<Cab> mDataItems;
        public Context mContext;
        public Cab mCabDetails;
        private NavigationActivity mActivity;
        private Typeface typeface;
        public event EventHandler<int> ItemClick;
        public CabRecyclerAdapter(Context context, List<Cab> listItems, NavigationActivity activity)
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
            if (ItemClick != null)
            {
                ItemClick(this, position);
                
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListRowViewHolder mHolder = holder as ListRowViewHolder;
            mCabDetails = mDataItems[position];
            mHolder.provider.Text = mCabDetails.Provider;
            mHolder.eta.Text = mCabDetails.Eta;
            mHolder.type.Text = mCabDetails.Type;
            mHolder.basefare.Text = mCabDetails.FareData.BaseFare;
            mHolder.freefare.Text = "0";
            mHolder.cstperkm.Text = mCabDetails.FareData.CostPerKilometer;
            mHolder.cstpermin.Text = mCabDetails.FareData.CostPerMinute;
            mHolder.surge.Text = mCabDetails.FareData.Surge;
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
            ListRowViewHolder holder = new ListRowViewHolder(v, OnClick) {mCardView=myView, provider = provider, type = type, eta = eta, basefare = basefare, freefare = freefare, cstperkm = cstperkm, cstpermin = cstpermin, surge = surge ,imageview=imageView};
            return holder;
        }
    }
    public class ListRowViewHolder : RecyclerView.ViewHolder
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

        public ListRowViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            myView = itemView;
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
}