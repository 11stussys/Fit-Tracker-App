using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using FitTrackerApp.Helpers;
using Google.Android.Material.TextField;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitTrackerAppFinal.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class AddBiometrics : AppCompatActivity, IOnSuccessListener
    {
        AndroidX.AppCompat.Widget.Toolbar toolbarBiometrics;
        TextInputLayout biometricsHeight, biometricsWeight, biometricsAge;
        Spinner sexSpinner;
        string userSex;
        ImageView addBiometricsImageView;
        TextView currentWeight, currentHeight, currentSex, currentAge;
        DocumentReference biometricsRef = AppDataHelper.GetFirestore().Collection("users").Document(AppDataHelper.GetFirebaseAuth().CurrentUser.Uid);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.add_biometrics);
            ConnectViews();
            //Setup toolbar
            SetSupportActionBar(toolbarBiometrics);
            AndroidX.AppCompat.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Drawable.arrowback);
            actionBar.SetDisplayHomeAsUpEnabled(true);
            //Setup type spinner
            sexSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(sexSpinner_ItemSelected);
            var adapterType = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.sex_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterType.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            sexSpinner.Adapter = adapterType;
            GetBiometrics();
            //Click handlers
            addBiometricsImageView.Click += AddBiometricsImageView_Click;
        }

        private void AddBiometricsImageView_Click(object sender, EventArgs e)
        {
            string age = biometricsAge.EditText.Text;
            string height = biometricsHeight.EditText.Text;
            string weight = biometricsWeight.EditText.Text;

            if (height == "")
            {
                Toast.MakeText(this, "Wprowadź wzrost", ToastLength.Short).Show();
                return;
            }
            else if (weight == "")
            {
                Toast.MakeText(this, "Wprowadź wagę", ToastLength.Short).Show();
                return;
            }
            else if (age == "")
            {
                Toast.MakeText(this, "Wprowadź wiek", ToastLength.Short).Show();
                return;
            }
            else if (Convert.ToInt32(age) < 0)
            {
                Toast.MakeText(this, "Wprowadź poprawny wiek", ToastLength.Short).Show();
                return;
            }
            else if(Convert.ToInt32(age) > 118)
            {
                Toast.MakeText(this, "Jesteś starszy od najstarszego człowieka na świecie? :)", ToastLength.Short).Show();
                return;
            }
            else if(Convert.ToInt32(height) < 0)
            {
                Toast.MakeText(this, "Wprowadź poprawny wzrost", ToastLength.Short).Show();
                return;
            }
            else if (Convert.ToInt32(height) > 251)
            {
                Toast.MakeText(this, "Jesteś wyższy od najwyższego człowieka na świecie? :)", ToastLength.Short).Show();
                return;
            }
            else if (Convert.ToDouble(weight) < 0)
            {
                Toast.MakeText(this, "Wprowadź poprawną wagę", ToastLength.Short).Show();
                return;
            }
            else if (Convert.ToDouble(weight) > 250)
            {
                Toast.MakeText(this, "Przy takiej wadze wskazane jest wybranie się do specjalisty", ToastLength.Short).Show();
                return;
            }
            
            
            biometricsRef.Update("user_height", Convert.ToInt32(height));
            biometricsRef.Update("user_weight", Convert.ToDouble(weight));
            biometricsRef.Update("user_age", Convert.ToInt32(age));
            biometricsRef.Update("user_sex", userSex.ToString());
            Toast.MakeText(this, "Dane biometryczne zaaktualizowane, gratulacje!", ToastLength.Short).Show();
            StartActivity(typeof(MainActivity));
            Finish();
        }

        private void sexSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            userSex = spinner.GetItemAtPosition(e.Position).ToString();
        }

        void ConnectViews()
        {
            toolbarBiometrics = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbarBiometrics);
            biometricsHeight = FindViewById<TextInputLayout>(Resource.Id.biometricsAddHeight);
            biometricsWeight = FindViewById<TextInputLayout>(Resource.Id.biometricsAddWeight);
            biometricsAge = FindViewById<TextInputLayout>(Resource.Id.biometricsAddAge);
            sexSpinner = FindViewById<Spinner>(Resource.Id.sexSpinner);
            addBiometricsImageView = FindViewById<ImageView>(Resource.Id.addBiometricsImageView);

            currentHeight = FindViewById<TextView>(Resource.Id.currentHeight);
            currentWeight = FindViewById<TextView>(Resource.Id.currentWeight);
            currentAge = FindViewById<TextView>(Resource.Id.currentAge);
            currentSex = FindViewById<TextView>(Resource.Id.currentSex);
        }
        
        void GetBiometrics()
        {
            biometricsRef.Get()
                .AddOnSuccessListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var snapshot = (DocumentSnapshot) result;

            if (snapshot != null)
            {
                    currentHeight.Text = snapshot.Get("user_height") != null ? snapshot.Get("user_height").ToString() : "";
                    currentWeight.Text = snapshot.Get("user_weight") != null ? snapshot.Get("user_weight").ToString() : "";
                    currentAge.Text = snapshot.Get("user_age") != null ? snapshot.Get("user_age").ToString() : "";
                    currentSex.Text = snapshot.Get("user_sex") != null ? snapshot.Get("user_sex").ToString() : "";
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