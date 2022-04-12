using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.TextField;
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
        TextInputLayout activityName, activityDuration, activityType, activityDescription;
        ImageView addActivityImageView;

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

            addActivityImageView.Click += AddActivityImageView_Click;
        }

        private void AddActivityImageView_Click(object sender, EventArgs e)
        {
            string name, type, description, duration;
            int durationInt;
            string[] types = new string[] { "Cardio", "Running", "Aerobics", "Strenght", "Durability" };
            name = activityName.EditText.Text;
            type = activityType.EditText.Text;
            description = activityDescription.EditText.Text;
            duration = activityDuration.EditText.Text;

            
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
            else if (!types.Contains(type))
            {
                Toast.MakeText(this, "You have to enter correct type: Cardio, Running, Aerobics, Strenght, Durability", ToastLength.Short).Show();
                return;
            }
            else if (description.Length > 120)
            {
                Toast.MakeText(this, "Your description is too long!", ToastLength.Short).Show();
                return;
            }
            else if (!int.TryParse(duration, out durationInt) || duration == "" || durationInt < 1)
            {
                Toast.MakeText(this, "You have to set a correct duration!", ToastLength.Short).Show();
                return;
            }
            Toast.MakeText(this, "Activity Created! Gratulations", ToastLength.Short).Show();
            //TODO
            StartActivity(typeof(MainActivity));
            Finish();
        }

        void ConnectViews()
        {
            toolbarAdd = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbarAdd);
            activityName = FindViewById<TextInputLayout>(Resource.Id.activityAddName);
            activityDuration = FindViewById<TextInputLayout>(Resource.Id.activityAddDuration);
            activityType = FindViewById<TextInputLayout>(Resource.Id.activityAddType);
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