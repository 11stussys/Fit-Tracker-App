using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase;
using Firebase.Auth;
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
    public class RegisterActivity : AppCompatActivity, IOnSuccessListener, IOnFailureListener
    {
        // XML Instances
        TextInputLayout usernameRegisterText, emailRegisterText, passwordRegisterText, passwordConfirmRegisterText;
        Button registerButton;
        TextView backToLoginTextView;

        //Firebase instances
        FirebaseFirestore database;
        FirebaseAuth registerAuth;

        //Variables
        ProgressDialogueFragment progressDialogue; 

        string username, email, password, passwordConfirm;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register);
            ConnectViews();

            database = AppDataHelper.GetFirestore();
            registerAuth = AppDataHelper.GetFirebaseAuth();
        }

        void ConnectViews()
        {
            usernameRegisterText = FindViewById<TextInputLayout>(Resource.Id.usernameRegisterText);
            emailRegisterText = FindViewById<TextInputLayout>(Resource.Id.emailRegisterText);
            passwordRegisterText = FindViewById<TextInputLayout>(Resource.Id.passwordRegisterText);
            passwordConfirmRegisterText = FindViewById<TextInputLayout>(Resource.Id.passwordConfirmRegisterText);

            registerButton = FindViewById<Button>(Resource.Id.registerButton);

            backToLoginTextView = FindViewById<TextView>(Resource.Id.backToLoginTextView);

            //Click handlers
            registerButton.Click += RegisterButton_Click;
            backToLoginTextView.Click += delegate { 
                StartActivity(typeof(LoginActivity));
                Finish();
            };
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            username = usernameRegisterText.EditText.Text;
            email = emailRegisterText.EditText.Text;
            password = passwordRegisterText.EditText.Text;
            passwordConfirm = passwordConfirmRegisterText.EditText.Text;

            if (username.Length < 4)
            {
                Toast.MakeText(this, "Username has to be longer! Please use at least 4 digits.", ToastLength.Short).Show();
                return;
            }
            else if (!email.Contains("@"))
            {
                Toast.MakeText(this, "Please enter a valid email address!", ToastLength.Short).Show();
                return;
            }
            else if (password != passwordConfirm)
            {
                Toast.MakeText(this, "Passwords must be the same!", ToastLength.Short).Show();
                return;
            }
            else if (password == username)
            {
                Toast.MakeText(this, "Username and password should be different!", ToastLength.Short).Show();
                return;
            }
            else if (password.Length < 6)
            {
                Toast.MakeText(this, "Password has to be longer! Please use at least 6 digits.", ToastLength.Short).Show();
                return;
            }
            ShowProgressDialogue("Registering...");
            registerAuth.CreateUserWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
                
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            HashMap userMap = new HashMap();
            userMap.Put("username", username);
            userMap.Put("email", email);
            DocumentReference userReference = database.Collection("users").Document(registerAuth.CurrentUser.Uid);
            userReference.Set(userMap);
            
            CloseProgressDialogue();
            Toast.MakeText(this, "Thank you for registering, you can log in now!", ToastLength.Short).Show();
            StartActivity(typeof(LoginActivity));
            Finish();
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            CloseProgressDialogue();
            Toast.MakeText(this, e.Message, ToastLength.Short).Show(); Toast.MakeText(this, e.Message, ToastLength.Short).Show();
        }

        //Progress bar functions
        void ShowProgressDialogue(string status)
        {
            progressDialogue = new ProgressDialogueFragment(status);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(trans, "Progress");
        }

        void CloseProgressDialogue()
        {
            if (progressDialogue != null)
            {
                progressDialogue.Dismiss();
                progressDialogue = null;
            }
        }
    }

}