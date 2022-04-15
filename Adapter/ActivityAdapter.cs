using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FitTrackerAppFinal.DataModels;
using System;
using System.Collections.Generic;

namespace FitTrackerAppFinal.Adapter
{
    internal class ActivityAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ActivityAdapterClickEventArgs> ItemClick;
        public event EventHandler<ActivityAdapterClickEventArgs> ItemLongClick;
        List<SportActivity> items;

        public ActivityAdapter(List<SportActivity> data)
        {
            items = data;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = null;
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.activity, parent, false);

            var vh = new ActivityAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as ActivityAdapterViewHolder;
            //holder.TextView.Text = items[position];

            holder.activityNameTextView.Text = item.Name;
            holder.activityDescription.Text = item.Description;
            holder.activityDateTextView.Text = item.ActivityDate.ToString();
        }

        public override int ItemCount => items.Count;

        void OnClick(ActivityAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ActivityAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class ActivityAdapterViewHolder : RecyclerView.ViewHolder
    {
        //Main Card
        public RelativeLayout moreDetailsLayout { get; set; }
        public ImageView activityTypeImageView { get; set; }
        public TextView activityNameTextView { get; set; }
        public TextView activityDateTextView { get; set; }
        public ImageView moreDetailsImageView { get; set; }

        //Additional info
        public TextView activityDuration { get; set; }
        public TextView activityType { get; set; }
        public TextView activityCalories { get; set; }
        public TextView activityDescription { get; set; }

        //Static info
        public TextView activityDurationTextView { get; set; }
        public TextView activityTypeTextView { get; set; }
        public TextView activityCaloriesTextView { get; set; }
        public TextView activityDescriptionTextView { get; set; }
        public ActivityAdapterViewHolder(View itemView, Action<ActivityAdapterClickEventArgs> clickListener,
                            Action<ActivityAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //Main Card
            activityNameTextView = itemView.FindViewById<TextView>(Resource.Id.activityNameTextView);
            activityTypeImageView = itemView.FindViewById<ImageView>(Resource.Id.activityTypeImageView);
            activityDateTextView = itemView.FindViewById<TextView>(Resource.Id.activityDateTextView);
            moreDetailsImageView = itemView.FindViewById<ImageView>(Resource.Id.moreDetailsImageView);
            moreDetailsLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.moreDetailsLayout);
            //Additional info
            activityDuration = itemView.FindViewById<TextView>(Resource.Id.activityDuration);
            activityType = itemView.FindViewById<TextView>(Resource.Id.activityType);
            activityCalories = itemView.FindViewById<TextView>(Resource.Id.activityCalories);
            activityDescription = itemView.FindViewById<TextView>(Resource.Id.activityDescription);

            //Static info
            activityDurationTextView = itemView.FindViewById<TextView>(Resource.Id.activityDurationTextView);
            activityTypeTextView = itemView.FindViewById<TextView>(Resource.Id.activityTypeTextView);
            activityCaloriesTextView = itemView.FindViewById<TextView>(Resource.Id.activityCaloriesTextView);
            activityDescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.activityDescriptionTextView);

            itemView.Click += (sender, e) => clickListener(new ActivityAdapterClickEventArgs { View = itemView});
            itemView.LongClick += (sender, e) => longClickListener(new ActivityAdapterClickEventArgs { View = itemView});
            moreDetailsImageView.Click += MoreDetailsImageView_Click;
        }

        private void MoreDetailsImageView_Click(object sender, EventArgs e)
        {
            if (moreDetailsLayout.Visibility == ViewStates.Visible){
                moreDetailsLayout.Visibility = ViewStates.Gone;
            }
            else if (moreDetailsLayout.Visibility == ViewStates.Gone){
                moreDetailsLayout.Visibility = ViewStates.Visible;
            }
        }
    }

    public class ActivityAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}