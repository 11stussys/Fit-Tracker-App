using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using Firebase.Storage;
using FitTrackerApp.Helpers;
using FitTrackerAppFinal.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FitTrackerAppFinal.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class UserProfile : AppCompatActivity, IOnSuccessListener
    {
        TextView profileUsername, profileWeight, profileHeight, profileAge, profileSex, profileBMI;
        ImageView profilePictureImageView, editProfileImageView;
        AndroidX.AppCompat.Widget.Toolbar toolbarProfile;
        DocumentReference biometricsRef = AppDataHelper.GetFirestore().Collection("users").Document(AppDataHelper.GetFirebaseAuth().CurrentUser.Uid);
        StorageReference storageReference = FirebaseStorage.Instance.GetReference("profileImages/" + AppDataHelper.GetFirebaseAuth().CurrentUser.Uid.ToString() + "_profileImage.jpg");
        TaskCompletionListeners downloadUrlListener = new TaskCompletionListeners();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.user_profile);
            ConnectViews();
            GetBiometrics();
            //Setup toolbar
            SetSupportActionBar(toolbarProfile);
            AndroidX.AppCompat.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Drawable.arrowback);
            actionBar.SetDisplayHomeAsUpEnabled(true);
        }
        void ConnectViews()
        {
            profileUsername = FindViewById<TextView>(Resource.Id.profileUsername);
            profileWeight = FindViewById<TextView>(Resource.Id.profileWeight);
            profileHeight = FindViewById<TextView>(Resource.Id.profileHeight);
            profileAge = FindViewById<TextView>(Resource.Id.profileAge);
            profileSex = FindViewById<TextView>(Resource.Id.profileSex);
            profileBMI = FindViewById<TextView>(Resource.Id.profileBMI);
            profilePictureImageView = FindViewById<ImageView>(Resource.Id.profilePictureImageView);
            editProfileImageView = FindViewById<ImageView>(Resource.Id.editProfileImageView);
            toolbarProfile = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbarProfile);

            //Downloading current profile picture
            if (storageReference != null)
            {
                storageReference.GetDownloadUrl().AddOnSuccessListener(downloadUrlListener);
            }
            downloadUrlListener.Success += (obj, args) =>
            {
                string downloadUrl = args.Result.ToString();
                Bitmap profileImageBitmap = GetImageBitmapFromUrl(downloadUrl);
                profilePictureImageView.SetImageBitmap(profileImageBitmap);
            };

            editProfileImageView.Click += delegate
            {
                StartActivity(typeof(AddBiometrics));
                Finish();
            };


        }
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

        void GetBiometrics()
        {
            biometricsRef.Get()
                .AddOnSuccessListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var snapshot = (DocumentSnapshot)result;

            if (snapshot != null)
            {
                profileUsername.Text = "Hej, " + (snapshot.Get("username") != null ? snapshot.Get("username").ToString() : "");
                profileHeight.Text = snapshot.Get("user_height") != null ? snapshot.Get("user_height").ToString() : "";
                profileWeight.Text = snapshot.Get("user_weight") != null ? snapshot.Get("user_weight").ToString() : "";
                profileAge.Text = snapshot.Get("user_age") != null ? snapshot.Get("user_age").ToString() : "";
                profileSex.Text = snapshot.Get("user_sex") != null ? snapshot.Get("user_sex").ToString() : "";
                profileBMI.Text = snapshot.Get("user_BMI") != null ? snapshot.Get("user_BMI").ToString() : "";
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