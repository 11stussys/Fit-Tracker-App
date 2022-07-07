using Android;
using Android.App;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Storage;
using FitTrackerApp.Helpers;
using FitTrackerAppFinal.Activities;
using FitTrackerAppFinal.Adapter;
using FitTrackerAppFinal.DataModels;
using FitTrackerAppFinal.EventListeners;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FitTrackerAppFinal
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, IOnSuccessListener
    {
        ImageView addActivityImageView;
        AndroidX.AppCompat.Widget.Toolbar toolbar;
        AndroidX.DrawerLayout.Widget.DrawerLayout drawerLayout;
        Google.Android.Material.Navigation.NavigationView navigationView;
        //Navbar
        View navHeader;
        TextView navBarUsername;
        CardView navBarBiometrics;
        ImageView navBarPictureImageView;

        RecyclerView activityRecyclerView;
        ActivityAdapter activityAdapter;
        List<SportActivity> listOfActivities;
        TextView currentBMI, describeBMI, todayCalories;
        DocumentReference biometricsRef = AppDataHelper.GetFirestore().Collection("users").Document(AppDataHelper.GetFirebaseAuth().CurrentUser.Uid);
        StorageReference storageReference = FirebaseStorage.Instance.GetReference("profileImages/" + AppDataHelper.GetFirebaseAuth().CurrentUser.Uid.ToString() + "_profileImage.jpg");
        FirebaseAuth userAuth;
        //Permissions
        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };
        int allCaloriesByDay = 0;
        byte[] fileBytes;
        TaskCompletionListeners taskCompletionListeners = new TaskCompletionListeners();
        TaskCompletionListeners downloadUrlListener = new TaskCompletionListeners();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            ConnectViews();
            FetchSportActivity();
            
            //Setup toolbar
            SetSupportActionBar(toolbar);
            AndroidX.AppCompat.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Drawable.menuaction);
            actionBar.SetDisplayHomeAsUpEnabled(true);
          
            //Retrieves username on login
            UsernameListener usernameListener = new UsernameListener();
            usernameListener.FetchUser();
            navBarUsername.Text = "Hej, " + AppDataHelper.GetUsername() + "!";

            GetBiometrics();

            //Request permissions for camera and gallery
            RequestPermissions(permissionGroup, 0);

        }

        //Permissions to camera and gallery
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        void ConnectViews()
        {
            addActivityImageView = FindViewById<ImageView>(Resource.Id.addActivityImageView);
            toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            drawerLayout = FindViewById<AndroidX.DrawerLayout.Widget.DrawerLayout>(Resource.Id.drawerLayout);
            navigationView = FindViewById<Google.Android.Material.Navigation.NavigationView>(Resource.Id.navView);
            navBarBiometrics = FindViewById<CardView>(Resource.Id.navBarBiometrics);
            navHeader = navigationView.GetHeaderView(0);
            navBarUsername = navHeader.FindViewById<TextView>(Resource.Id.navBarUsername);
            navBarPictureImageView = navHeader.FindViewById<ImageView>(Resource.Id.navBarPictureImageView);
            userAuth = AppDataHelper.GetFirebaseAuth();
            activityRecyclerView = FindViewById<RecyclerView>(Resource.Id.activityRecycleView);
            currentBMI = FindViewById<TextView>(Resource.Id.currentBMI);
            describeBMI = FindViewById<TextView>(Resource.Id.describeBMI);
            todayCalories = FindViewById<TextView>(Resource.Id.todayCalories);
            

            //Downloading current profile picture
            if (storageReference != null)
            {
                storageReference.GetDownloadUrl().AddOnSuccessListener(downloadUrlListener);
            }
            downloadUrlListener.Success += (obj, args) =>
            {
                string downloadUrl = args.Result.ToString();
                Bitmap profileImageBitmap = GetImageBitmapFromUrl(downloadUrl);
                navBarPictureImageView.SetImageBitmap(profileImageBitmap);
            };
            //Click handlers
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
            addActivityImageView.Click += delegate{
                StartActivity(typeof(AddSportActivity));
                Finish();
            };

            navBarBiometrics.Click += delegate {
                StartActivity(typeof(AddBiometrics));
                Finish();
            };

            navBarPictureImageView.Click += NavBarPictureImageView_Click;
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
        private void NavBarPictureImageView_Click(object sender, EventArgs e)
        {
            AndroidX.AppCompat.App.AlertDialog.Builder profilePhotoAlert = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
            profilePhotoAlert.SetMessage("Zmień zdjęcie profilowe");

            profilePhotoAlert.SetNegativeButton("Zrób zdjęcie", (thisalert, args) =>
            {
                //Capture Image
                TakePhoto();
            });

            profilePhotoAlert.SetPositiveButton("Wybierz zdjęcie", (thisalert, args) =>
            {
                //Select Image
                SelectPhoto();
            });
            profilePhotoAlert.Show();
        }
        void SaveImageToDb()
        {
            

            if(fileBytes != null)
            {
                storageReference.PutBytes(fileBytes)
                    .AddOnSuccessListener(taskCompletionListeners)
                    .AddOnFailureListener(taskCompletionListeners);
            }

            taskCompletionListeners.Success += (obj, args) =>
            {
                Toast.MakeText(this, "Udało się zmienić zdjęcie!", ToastLength.Short);

            };

            taskCompletionListeners.Failure += (obj, args) =>
            {
                Toast.MakeText(this, "Nie udało się ustawić zdjęcia!", ToastLength.Short);
            };
        }

        async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 20,
                Directory = "ProfileImage",
                Name = AppDataHelper.GetFirebaseAuth().CurrentUser.Uid.ToString() + "_profileImage.jpg"
            });

            if (file == null) return;

            //Converting file.path to byte array and set a bitmap to imageview
            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            fileBytes = imageArray;
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            navBarPictureImageView.SetImageBitmap(bitmap);
            SaveImageToDb();
        }

        async void SelectPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                Toast.MakeText(this, "Upload zdjęcia nie obsługiwany", ToastLength.Short).Show();
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 30,
            });

            if (file == null) return;

            //Converting file.path to byte array and set a bitmap to imageview
            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            fileBytes = imageArray;
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            navBarPictureImageView.SetImageBitmap(bitmap);
            SaveImageToDb();
        }
        private void NavigationView_NavigationItemSelected(object sender, Google.Android.Material.Navigation.NavigationView.NavigationItemSelectedEventArgs e)
        {
            int id = e.MenuItem.ItemId;
            if (id == Resource.Id.profileNav)
            {
                drawerLayout.CloseDrawers();
                StartActivity(typeof(UserProfile));
                Finish();
            }
            else if (id == Resource.Id.editBiometrics)
            {
                drawerLayout.CloseDrawers();
                StartActivity(typeof(AddBiometrics));
                Finish();
            }
            else if(id == Resource.Id.addActivityDrawer)
            {
                drawerLayout.CloseDrawers();
                StartActivity(typeof(AddSportActivity));
                Finish();
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

        void FetchSportActivity()
        {
            SportActivityEventListener sportActivityEventListener = new SportActivityEventListener();
            sportActivityEventListener.FetchSportActivity();
            sportActivityEventListener.OnActivityRetrieved += SportActivityEventListener_OnActivityRetrieved;
        }

        private void SportActivityEventListener_OnActivityRetrieved(object sender, SportActivityEventListener.ActivityEventArgs e)
        {
            listOfActivities = new List<SportActivity>();
            listOfActivities = e.SportActivities;           
            foreach(var item in listOfActivities)
            {
                if(item.ActivityDate == DateTime.Today) allCaloriesByDay += item.CaloriesBurned;
            }
            todayCalories.Text = allCaloriesByDay + " kalorii";
            SetupRecyclerView();

        }
        void SetupRecyclerView()
        {
            activityRecyclerView.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(activityRecyclerView.Context));
            activityAdapter = new ActivityAdapter(listOfActivities);
            activityRecyclerView.SetAdapter(activityAdapter);
            activityAdapter.ItemLongClick += ActivityAdapter_ItemLongClick;
        }

        private void ActivityAdapter_ItemLongClick(object sender, ActivityAdapterClickEventArgs e)
        {
            List<SportActivity> sortedList = listOfActivities.OrderByDescending(x => x.ActivityDate.Date).ToList();
            string activityID = sortedList[e.Position].ID;
            string ownerID = sortedList[e.Position].OwnerID;

            if (AppDataHelper.GetFirebaseAuth().CurrentUser.Uid == ownerID)
            {
                AndroidX.AppCompat.App.AlertDialog.Builder activityAlert = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
                activityAlert.SetTitle("Usuń aktywność");
                activityAlert.SetMessage("Jesteś pewny?");

                activityAlert.SetNegativeButton("Anuluj", (o, args) =>
                {
                    activityAlert.Dispose();
                });

                activityAlert.SetPositiveButton("Usuń", (o, args) =>
                {
                    AppDataHelper.GetFirestore().Collection("activities").Document(activityID).Delete();
                });
                activityAlert.Show();
            }
        }

        void GetBiometrics()
        {
            AppDataHelper.GetFirestore().Collection("users").Document(AppDataHelper.GetFirebaseAuth().CurrentUser.Uid).Get()
                .AddOnSuccessListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var snapshot = (DocumentSnapshot)result;
            try { 
                if (snapshot != null)
                {
                    int height = snapshot.Get("user_height") != null ? Convert.ToInt32(snapshot.Get("user_height")) : 0;
                    double weight = snapshot.Get("user_weight") != null ? Convert.ToDouble(snapshot.Get("user_weight")) : 0.0;
                    string age = snapshot.Get("user_age") != null ? snapshot.Get("user_age").ToString() : "";
                    string sex = snapshot.Get("user_sex") != null ? snapshot.Get("user_sex").ToString() : "";
                    double heightInMeters = height / 100.0;
                    double BMI = Math.Round(weight / (heightInMeters * heightInMeters), 2);
                    if (BMI < 0 || Double.IsNaN(BMI))
                    {
                        navBarBiometrics.Visibility = ViewStates.Visible;
                        describeBMI.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        navBarBiometrics.Visibility = ViewStates.Gone;
                        describeBMI.Visibility = ViewStates.Visible;
                    }
                    if (BMI < 16.99)
                    {
                        currentBMI.SetTextColor(Android.Graphics.Color.Red);
                        describeBMI.Text = "Twoje BMI jest zdecydowanie za niskie!";
                    }
                    else if (BMI < 18.49)
                    {
                        currentBMI.SetTextColor(Android.Graphics.Color.Yellow);
                        describeBMI.Text = "Twoje BMI jest za niskie!";
                    }
                    else if (BMI < 24.99)
                    {
                        currentBMI.SetTextColor(Android.Graphics.Color.Green);
                        describeBMI.Text = "Twoje BMI jest prawidłowe, tak trzymaj!";
                    }
                    else if (BMI < 29.99)
                    {
                        currentBMI.SetTextColor(Android.Graphics.Color.Yellow);
                        describeBMI.Text = "Twoje BMI jest za wysokie!";
                    }
                    else if (BMI >= 30)
                    {
                        currentBMI.SetTextColor(Android.Graphics.Color.Red);
                        describeBMI.Text = "Twoje BMI jest zdecydowanie za wysokie!";
                    }
                    biometricsRef.Update("user_BMI", Convert.ToDouble(BMI));
                    currentBMI.Text = BMI.ToString();
                }
            }
            catch(Exception e)
            { 
                currentBMI.Text = "0";
                biometricsRef.Update("user_BMI", 0);
                Toast.MakeText(this, "Uzupełnij dane biometryczne, aby zobaczyć swoje BMI! " + e.Message, ToastLength.Short).Show();
                return;
            }
        }

    }

}

