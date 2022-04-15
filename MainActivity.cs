using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using FitTrackerApp.Helpers;
using FitTrackerAppFinal.Activities;
using FitTrackerAppFinal.Adapter;
using FitTrackerAppFinal.DataModels;
using FitTrackerAppFinal.EventListeners;
using Java.Util;
using System.Collections.Generic;

namespace FitTrackerAppFinal
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        ImageView addActivityImageView;
        AndroidX.AppCompat.Widget.Toolbar toolbar;
        AndroidX.DrawerLayout.Widget.DrawerLayout drawerLayout;
        Google.Android.Material.Navigation.NavigationView navigationView;
        TextView navBarUsername;
        RecyclerView activityRecyclerView;
        ActivityAdapter activityAdapter;
        List<SportActivity> listOfActivities;

        FirebaseFirestore database;
        FirebaseAuth userAuth;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            ConnectViews();
            CreateData();
            SetupRecyclerView();
            //Setup toolbar
            SetSupportActionBar(toolbar);
            AndroidX.AppCompat.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Drawable.menuaction);
            actionBar.SetDisplayHomeAsUpEnabled(true);
            
            //Retrieves username on login
            UsernameListener usernameListener = new UsernameListener();
            usernameListener.FetchUser();
        }

        void ConnectViews()
        {
            addActivityImageView = FindViewById<ImageView>(Resource.Id.addActivityImageView);
            toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            drawerLayout = FindViewById<AndroidX.DrawerLayout.Widget.DrawerLayout>(Resource.Id.drawerLayout);
            navigationView = FindViewById<Google.Android.Material.Navigation.NavigationView>(Resource.Id.navView);
            navBarUsername = FindViewById<TextView>(Resource.Id.navBarUsername);
            database = AppDataHelper.GetFirestore();
            userAuth = AppDataHelper.GetFirebaseAuth();

            activityRecyclerView = FindViewById<RecyclerView>(Resource.Id.activityRecycleView);

            //Click handlers
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
            addActivityImageView.Click += delegate{
                StartActivity(typeof(AddSportActivity));
                Finish();
            };
        }

        private void NavigationView_NavigationItemSelected(object sender, Google.Android.Material.Navigation.NavigationView.NavigationItemSelectedEventArgs e)
        {
            int id = e.MenuItem.ItemId;
            if (id == Resource.Id.profileNav)
            {
                drawerLayout.CloseDrawers();
            }
            else if (id == Resource.Id.ownActivityNav)
            {
                drawerLayout.CloseDrawers();
            }
            else if(id == Resource.Id.inviteFriendNav)
            {
                Toast.MakeText(this, "Not available yet :(", ToastLength.Short).Show();
                drawerLayout.CloseDrawers();
            }
            else if(id == Resource.Id.logoutNav)
            {             
                drawerLayout.CloseDrawers();
                userAuth.SignOut();
                StartActivity(typeof(LoginActivity));
                Finish();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }      
        }

        void CreateData()
        {
            listOfActivities = new List<SportActivity>();
            listOfActivities.Add(new SportActivity { Name = "Bieganko", Description = "Testowy opis", Username = "Nazwa usera", ActivityDate = System.DateTime.Now });
            listOfActivities.Add(new SportActivity { Name = "Pompeczki", Description = "Testowy opis", Username = "Nazwa usera", ActivityDate = System.DateTime.Today });
        }

        void SetupRecyclerView()
        {
            activityRecyclerView.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(activityRecyclerView.Context));
            activityAdapter = new ActivityAdapter(listOfActivities);
            activityRecyclerView.SetAdapter(activityAdapter);
        }

    }

}

