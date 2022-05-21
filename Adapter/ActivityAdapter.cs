using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FitTrackerAppFinal.DataModels;
using System;
using System.Linq;
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
            var oList = items.OrderByDescending(x => x.ActivityDate.Date);
            List<SportActivity> orderedList = new List<SportActivity>();
            foreach (var orderedActivity in oList)
            {
                orderedList.Add(orderedActivity);
            }
            var item = orderedList[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as ActivityAdapterViewHolder;
            //holder.TextView.Text = items[position];

            holder.activityNameTextView.Text = item.Name;
            holder.activityDescription.Text = item.Description;
            holder.activityDateTextView.Text = item.ActivityDate.ToString("dd/MM/yyyy");
            holder.activityType.Text = item.Type;
            if (item.Type == "Bieganie") holder.activityTypeImageView.SetImageResource(Resource.Drawable.running);
            else if (item.Type == "Spacer") holder.activityTypeImageView.SetImageResource(Resource.Drawable.walking);
            else if (item.Type == "Trening siłowy") holder.activityTypeImageView.SetImageResource(Resource.Drawable.muscle);
            else if (item.Type == "Podciągnięcia") holder.activityTypeImageView.SetImageResource(Resource.Drawable.pullups);
            else if (item.Type == "Przysiady") holder.activityTypeImageView.SetImageResource(Resource.Drawable.squats);
            else if (item.Type == "Brzuszki") holder.activityTypeImageView.SetImageResource(Resource.Drawable.situps);            else if (item.Type == "Spacer") holder.activityTypeImageView.SetImageResource(Resource.Drawable.walking);
            else if (item.Type == "Pompki") holder.activityTypeImageView.SetImageResource(Resource.Drawable.pushups);
            holder.activitySubtype.Text = item.Subtype;
            holder.activityCalories.Text = item.CaloriesBurned.ToString();
            if(item.Type == "Bieganie" || item.Type == "Spacer")
            {
                RunningActivity castedItem = (RunningActivity)item;
                //Making duration and distance visible
                holder.activityDuration.Visibility = ViewStates.Visible;
                holder.activityDistance.Visibility = ViewStates.Visible;
                holder.activityDistanceTextView.Visibility = ViewStates.Visible;
                holder.activityDurationTextView.Visibility = ViewStates.Visible;
                //Making sets and reps invisible
                holder.activitySets.Visibility = ViewStates.Gone;
                holder.activityReps.Visibility = ViewStates.Gone;
                holder.activitySetsTextView.Visibility = ViewStates.Gone;
                holder.activityRepsTextView.Visibility = ViewStates.Gone;
                //Set values
                holder.activityDistance.Text = castedItem.Distance.ToString();
                holder.activityDuration.Text = castedItem.Duration.ToString();
            }
            else
            {
                RegularActivity castedItem = (RegularActivity)item;
                //Making duration and distance invisible
                holder.activityDuration.Visibility = ViewStates.Gone;
                holder.activityDistance.Visibility = ViewStates.Gone;
                holder.activityDistanceTextView.Visibility = ViewStates.Gone;
                holder.activityDurationTextView.Visibility = ViewStates.Gone;
                //Making sets and reps visible
                holder.activitySets.Visibility = ViewStates.Visible;
                holder.activityReps.Visibility = ViewStates.Visible;
                holder.activitySetsTextView.Visibility = ViewStates.Visible;
                holder.activityRepsTextView.Visibility = ViewStates.Visible;
                //Set values
                holder.activitySets.Text = castedItem.Sets.ToString();
                holder.activityReps.Text = castedItem.Reps.ToString();
            }
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
        public TextView activitySubtype { get; set; }
        public TextView activityCalories { get; set; }
        public TextView activityDescription { get; set; }

        //Additional with visibility
        public TextView activityDistance { get; set; }
        public TextView activitySets { get; set; }
        public TextView activityReps { get; set; }

        //Static info
        public TextView activityDurationTextView { get; set; }
        public TextView activityTypeTextView { get; set; }
        public TextView activitySubtypeTextView { get; set; }
        public TextView activityCaloriesTextView { get; set; }
        public TextView activityDescriptionTextView { get; set; }

        //Static with visibility
        public TextView activityDistanceTextView { get; set; }
        public TextView activitySetsTextView { get; set; }
        public TextView activityRepsTextView { get; set; }

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
            activitySubtype = itemView.FindViewById<TextView>(Resource.Id.activitySubtype);
            activityCalories = itemView.FindViewById<TextView>(Resource.Id.activityCalories);
            activityDescription = itemView.FindViewById<TextView>(Resource.Id.activityDescription);

            //Additional with visibility
            activityDistance = itemView.FindViewById<TextView>(Resource.Id.activityDistance);
            activitySets = itemView.FindViewById<TextView>(Resource.Id.activitySets);
            activityReps = itemView.FindViewById<TextView>(Resource.Id.activityReps);

            //Static info
            activityDurationTextView = itemView.FindViewById<TextView>(Resource.Id.activityDurationTextView);
            activityTypeTextView = itemView.FindViewById<TextView>(Resource.Id.activityTypeTextView);
            activitySubtypeTextView = itemView.FindViewById<TextView>(Resource.Id.activitySubtypeTextView);
            activityCaloriesTextView = itemView.FindViewById<TextView>(Resource.Id.activityCaloriesTextView);
            activityDescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.activityDescriptionTextView);

            //Static with visibility
            activityDistanceTextView = itemView.FindViewById<TextView>(Resource.Id.activityDistanceTextView);
            activitySetsTextView = itemView.FindViewById<TextView>(Resource.Id.activitySetsTextView);
            activityRepsTextView = itemView.FindViewById<TextView>(Resource.Id.activityRepsTextView);

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