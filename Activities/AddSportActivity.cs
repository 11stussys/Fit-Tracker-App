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
using System.Globalization;
using System.Linq;
using System.Text;

namespace FitTrackerAppFinal.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class AddSportActivity : AppCompatActivity
    {
        AndroidX.AppCompat.Widget.Toolbar toolbarAdd;
        TextInputLayout activityName, activityDuration, activityDistance, activityDescription, activitySets, activityReps, activityCaloriesPerRep, activityCaloriesPerKm;
        EditText activityDate;
        Spinner activityTypeSpinner, activitySubtypeSpinner;
        ImageView addActivityImageView;
        string activityType, activitySubtype;

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
            //Setup type spinner
            activityTypeSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(activityTypeSpinner_ItemSelected);
            var adapterType = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.type_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterType.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            activityTypeSpinner.Adapter = adapterType;

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

        private void AddActivityImageView_Click(object sender, EventArgs e)
        {
            string name, description;
            int duration, sets, reps;
            double? distance;
            double? calories = null;
            NumberFormatInfo doubleSeparator = new NumberFormatInfo();
            doubleSeparator.NumberDecimalSeparator = ".";
            name = activityName.EditText.Text;
            description = activityDescription.EditText.Text;
            if (name.Length > 20)
            {
                Toast.MakeText(this, "Twoja nazwa jest zbyt długa. Użyj maksymalnie 20 znaków.", ToastLength.Short).Show();
                return;
            }
            else if (name == "")
            {
                Toast.MakeText(this, "Ustaw nazwę dla swojej aktywności.", ToastLength.Short).Show();
                return;
            }
            else if (description.Length > 120)
            {
                Toast.MakeText(this, "Twój opis jest zbyt długi, użyj maksymalnie 120 znaków.", ToastLength.Short).Show();
                return;
            }

            HashMap activityMap = new HashMap();
            activityMap.Put("user", AppDataHelper.GetUsername());
            activityMap.Put("owner_id", AppDataHelper.GetFirebaseAuth().CurrentUser.Uid);
            activityMap.Put("activity_name", name);
            activityMap.Put("activity_date", activityDate.Text);
            activityMap.Put("activity_type", activityType);
            activityMap.Put("activity_subtype", activitySubtype);
            activityMap.Put("activity_description", description);
            //calories settings using subtype

            if (activityType == "Bieganie" || activityType == "Spacer" || activityType == "Trening siłowy")
            {
                if (activityType == "Bieganie" || activityType == "Spacer")
                {
                    if (activityDistance.EditText.Text == "")
                    {
                        Toast.MakeText(this, "Ustaw dystans aktywności", ToastLength.Short).Show();
                        return;
                    }
                    distance = Convert.ToDouble(activityDistance.EditText.Text, doubleSeparator);
                    activityMap.Put("activity_distance", distance);                    
                }
                if (activityDuration.EditText.Text == "")
                {
                    Toast.MakeText(this, "Ustaw czas aktywności", ToastLength.Short).Show();
                    return;
                }
                duration = Convert.ToInt32(activityDuration.EditText.Text);
                activityMap.Put("activity_duration", duration); 
                if (activityType == "Bieganie") calories = duration * 11;
                else if (activityType == "Spacer") calories = duration * 5;
                else if (activityType == "Trening siłowy") calories = duration * 4;
                if (activityCaloriesPerKm.EditText.Text != "") calories = Convert.ToDouble(activityCaloriesPerKm.EditText.Text, doubleSeparator) * duration;
                activityMap.Put("calories_burned", calories);
            }
            else
            {
                if (activitySets.EditText.Text == "")
                {
                    Toast.MakeText(this, "Ustaw ilość serii", ToastLength.Short).Show();
                    return;
                }
                else if (activityReps.EditText.Text == "")
                {
                    Toast.MakeText(this, "Ustaw ilość powtórzeń", ToastLength.Short).Show();
                    return;
                }
                sets = Convert.ToInt32(activitySets.EditText.Text);
                reps = Convert.ToInt32(activityReps.EditText.Text);
                if (activityType == "Podciągnięcia") calories = sets * reps;
                else if (activityType == "Przysiady") calories = sets * reps / 3;
                else if (activityType == "Brzuszki") calories = sets * reps / 3;
                else if (activityType == "Pompki") calories = sets * reps / 2;
                if (activityCaloriesPerRep.EditText.Text != "") calories = Convert.ToDouble(activityCaloriesPerRep.EditText.Text, doubleSeparator) * reps * sets;
                activityMap.Put("calories_burned", calories);
                activityMap.Put("activity_sets", sets);
                activityMap.Put("activity_reps", reps);
                
            }
            DocumentReference newActivityRef = AppDataHelper.GetFirestore().Collection("activities").Document();
            string activityID = newActivityRef.Id;
            newActivityRef.Set(activityMap);
            Toast.MakeText(this, "Aktywność utworzona, gratulacje!", ToastLength.Short).Show();
            StartActivity(typeof(MainActivity));
            Finish();
        }

        void ConnectViews()
        {
            toolbarAdd = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbarAdd);
            activityName = FindViewById<TextInputLayout>(Resource.Id.activityAddName);
            activityDate = FindViewById<EditText>(Resource.Id.activityAddDate);
            activityDuration = FindViewById<TextInputLayout>(Resource.Id.activityAddDuration);
            activityDistance = FindViewById<TextInputLayout>(Resource.Id.activityAddDistance);
            activitySets = FindViewById<TextInputLayout>(Resource.Id.activitySets);
            activityReps = FindViewById<TextInputLayout>(Resource.Id.activityReps);
            activityCaloriesPerKm = FindViewById<TextInputLayout>(Resource.Id.activityAddCaloriesPerKm);
            activityCaloriesPerRep = FindViewById<TextInputLayout>(Resource.Id.activityAddCaloriesPerRep);
            activityTypeSpinner = FindViewById<Spinner>(Resource.Id.activityTypeSpinner);
            activitySubtypeSpinner = FindViewById<Spinner>(Resource.Id.activitySubtypeSpinner);
            activityDescription = FindViewById<TextInputLayout>(Resource.Id.activityAddDescription);
            addActivityImageView = FindViewById<ImageView>(Resource.Id.addActivityImageView);
        }

        void SubtypeSpinnerHandler(string typeName)
        {
            int subtypeArray = 0;
            if (typeName == "Bieganie") subtypeArray = Resource.Array.running_array;
            else if (typeName == "Spacer") subtypeArray = Resource.Array.walking_array;
            else if (typeName == "Trening siłowy") subtypeArray = Resource.Array.strenght_array;
            else if (typeName == "Podciągnięcia") subtypeArray = Resource.Array.pullups_array;
            else if (typeName == "Brzuszki") subtypeArray = Resource.Array.situps_array;
            else if (typeName == "Pompki") subtypeArray = Resource.Array.pushups_array;
            else if (typeName == "Przysiady") subtypeArray = Resource.Array.squats_array;
            try
            {
                //Setup subtype spinner
                activitySubtypeSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(activitySubtypeSpinner_ItemSelected);
                var adapterSubtype = ArrayAdapter.CreateFromResource(
                        this, subtypeArray, Android.Resource.Layout.SimpleSpinnerItem);
                adapterSubtype.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                activitySubtypeSpinner.Adapter = adapterSubtype;
            }
            catch
            {
                Toast.MakeText(this, "Wystąpił nieoczekiwany błąd AddSportActivity(SubtypeSpinnerHandler).", ToastLength.Short).Show();
            }
         }

        private void activityTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            activityType = spinner.GetItemAtPosition(e.Position).ToString();
            SubtypeSpinnerHandler(activityType);
            if (activityType == "Bieganie" || activityType == "Spacer")
            {
                activitySets.Visibility = ViewStates.Gone;
                activityReps.Visibility = ViewStates.Gone;
                activitySets.EditText.Text = "";
                activityReps.EditText.Text = "";
                activityDuration.Visibility = ViewStates.Visible;
                activityDistance.Visibility = ViewStates.Visible;
            }
            else if(activityType == "Trening siłowy")
            {
                activitySets.Visibility = ViewStates.Gone;
                activityReps.Visibility = ViewStates.Gone;
                activityDistance.Visibility = ViewStates.Gone;
                activityDistance.EditText.Text = "";
                activitySets.EditText.Text = "";
                activityReps.EditText.Text = "";
                activityDuration.Visibility = ViewStates.Visible;
            }
            else
            {
                activityDuration.Visibility = ViewStates.Gone;
                activityDistance.Visibility = ViewStates.Gone;
                activityDuration.EditText.Text = "";
                activityDistance.EditText.Text = "";
                activitySets.Visibility = ViewStates.Visible;
                activityReps.Visibility = ViewStates.Visible;
            }
        }

        private void activitySubtypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            activitySubtype = spinner.GetItemAtPosition(e.Position).ToString();
            if(activitySubtype == "Inny")
            {
                activityCaloriesPerKm.Visibility = ViewStates.Visible;
                activityCaloriesPerRep.Visibility = ViewStates.Gone;
                activityCaloriesPerRep.EditText.Text = null;
            }
            else if(activitySubtype == "Inne")
            {
                activityCaloriesPerRep.Visibility = ViewStates.Visible;
                activityCaloriesPerKm.Visibility = ViewStates.Gone;
                activityCaloriesPerKm.EditText.Text = null;
            }
            else
            {
                activityCaloriesPerRep.Visibility = ViewStates.Gone;
                activityCaloriesPerKm.Visibility = ViewStates.Gone;
                activityCaloriesPerRep.EditText.Text = null;
                activityCaloriesPerKm.EditText.Text = null;
            }
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