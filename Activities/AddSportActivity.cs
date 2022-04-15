using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using FitTrackerApp.Helpers;
using FitTrackerAppFinal.Fragments;
using Google.Android.Material.TextField;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitTrackerAppFinal.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class AddSportActivity : AppCompatActivity
    {
        AndroidX.AppCompat.Widget.Toolbar toolbarAdd;
        TextInputLayout activityName, activityDuration, activityDescription, activitySets, activityReps;
        EditText activityDate;
        Spinner activityTypeSpinner;
        ImageView addActivityImageView;
        string activityType;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.create_activity);
            ConnectViews();
            //Setup toolbar
            SetSupportActionBar(toolbarAdd);
            AndroidX.AppCompat.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Drawable.arrowback);
            actionBar.SetDisplayHomeAsUpEnabled(true);
            //Setup spinner
            activityTypeSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(activityTypeSpinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.type_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            activityTypeSpinner.Adapter = adapter;

            //Click handlers
            addActivityImageView.Click += AddActivityImageView_Click;
            activityDate.Click += ActivityDate_Click;
        }

        private void ActivityDate_Click(object sender, EventArgs e)
        {
            var trans = SupportFragmentManager.BeginTransaction();
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate(DateTime time) 
            {
                activityDate.Text = time.ToString("yyyy'-'MM'-'dd");
            });
            frag.Show(trans, DatePickerFragment.TAG);
        }

        private void activityTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            activityType = spinner.GetItemAtPosition(e.Position).ToString();
            if (activityType == "Bieganie")
            {
                activitySets.Visibility = ViewStates.Gone;
                activityReps.Visibility = ViewStates.Gone;
                activitySets.EditText.Text = "";
                activityReps.EditText.Text = "";
                activityDuration.Visibility = ViewStates.Visible;
            }
            else
            {
                activityDuration.Visibility = ViewStates.Gone;
                activityDuration.EditText.Text = "";
                activitySets.Visibility = ViewStates.Visible;
                activityReps.Visibility = ViewStates.Visible;
            }
        }

        private void AddActivityImageView_Click(object sender, EventArgs e)
        {
            string name, description;
            int duration, sets, reps; 
            name = activityName.EditText.Text;
            description = activityDescription.EditText.Text;

            if (name.Length > 20)
            {
                Toast.MakeText(this, "Your name is too long!", ToastLength.Short).Show();
                return;
            }
            else if (name == "")
            {
                Toast.MakeText(this, "Please set a name for your activity!", ToastLength.Short).Show();
                return;
            }
            else if (description.Length > 120)
            {
                Toast.MakeText(this, "Your description is too long!", ToastLength.Short).Show();
                return;
            }

            HashMap activityMap = new HashMap();
            activityMap.Put("user", AppDataHelper.GetUsername());
            activityMap.Put("owner_id", AppDataHelper.GetFirebaseAuth().CurrentUser.Uid);
            activityMap.Put("activity_name", name);
            activityMap.Put("activity_date", activityDate.Text);
            activityMap.Put("activity_type", activityType);
            activityMap.Put("activity_description", description);
            if (activityType == "Bieganie")
            {
                duration = Convert.ToInt32(activityDuration.EditText.Text);
                activityMap.Put("activity_duration", duration.ToString());
            }
            else
            {
                sets = Convert.ToInt32(activitySets.EditText.Text);
                reps = Convert.ToInt32(activityReps.EditText.Text);
                activityMap.Put("activity_sets", sets);
                activityMap.Put("activity_reps", reps);
            }
            DocumentReference newActivityRef = AppDataHelper.GetFirestore().Collection("activities").Document();
            newActivityRef.Set(activityMap);
            Toast.MakeText(this, "Activity Created! Gratulations", ToastLength.Short).Show();
            StartActivity(typeof(MainActivity));
            Finish();
        }

        void ConnectViews()
        {
            toolbarAdd = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbarAdd);
            activityName = FindViewById<TextInputLayout>(Resource.Id.activityAddName);
            activityDate = FindViewById<EditText>(Resource.Id.activityAddDate);
            activityDuration = FindViewById<TextInputLayout>(Resource.Id.activityAddDuration);
            activitySets = FindViewById<TextInputLayout>(Resource.Id.activitySets);
            activityReps = FindViewById<TextInputLayout>(Resource.Id.activityReps);
            activityTypeSpinner = FindViewById<Spinner>(Resource.Id.activityTypeSpinner);
            activityDescription = FindViewById<TextInputLayout>(Resource.Id.activityAddDescription);
            addActivityImageView = FindViewById<ImageView>(Resource.Id.addActivityImageView);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    StartActivity(typeof(MainActivity));
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

    }
}