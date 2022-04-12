using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using FitTrackerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitTrackerAppFinal.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/Mytheme.Splash", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();

            FirebaseUser currentUser = AppDataHelper.GetFirebaseAuth().CurrentUser;
            if(currentUser != null)
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }
            else
            {
                StartActivity(typeof(LoginActivity));
            }
        }
    }
}