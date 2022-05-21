using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Android.Material.TextField;
using Android;
using Firebase.Auth;
using Android.Gms.Tasks;
using FitTrackerApp.Helpers;
using FitTrackerAppFinal.Fragments;

namespace FitTrackerAppFinal.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class LoginActivity : AppCompatActivity, IOnSuccessListener, IOnFailureListener
    {
        // XML instances
        TextInputLayout emailLoginText, passwordLoginText;
        Button loginButton, signUpButton;
        ImageView facebookLogin, twitterLogin, linkedinLogin, googleLogin;

        // Variables
        FirebaseAuth loginAuth;
        ProgressDialogueFragment progressDialogue; 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);

            loginAuth = AppDataHelper.GetFirebaseAuth();
            ConnectViews();
        }

        void ConnectViews()
        {
            emailLoginText = FindViewById<TextInputLayout>(Resource.Id.emailLoginText);
            passwordLoginText = FindViewById<TextInputLayout>(Resource.Id.passwordLoginText);

            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            signUpButton = FindViewById<Button>(Resource.Id.signUpButton);

            facebookLogin = FindViewById<ImageView>(Resource.Id.facebookLoginImageView);
            twitterLogin = FindViewById<ImageView>(Resource.Id.twitterLoginImageView);
            linkedinLogin = FindViewById<ImageView>(Resource.Id.linkedinLoginImageView);
            googleLogin = FindViewById<ImageView>(Resource.Id.googleLoginImageView);

            //Click handlers

            loginButton.Click += LoginButton_Click;
            signUpButton.Click += delegate { 
                StartActivity(typeof(RegisterActivity));
                Finish();
            };

            facebookLogin.Click += FacebookLogin_Click;
            twitterLogin.Click += TwitterLogin_Click;
            linkedinLogin.Click += LinkedinLogin_Click;
            googleLogin.Click += GoogleLogin_Click;
        }

        private void GoogleLogin_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Jeszcze niedostępne :(", ToastLength.Short).Show();
        }

        private void LinkedinLogin_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Jeszcze niedostępne :(", ToastLength.Short).Show();
        }

        private void TwitterLogin_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Jeszcze niedostępne :(", ToastLength.Short).Show();
        }

        private void FacebookLogin_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Jeszcze niedostępne :(", ToastLength.Short).Show();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string email, password;
            email = emailLoginText.EditText.Text;
            password = passwordLoginText.EditText.Text;


            if (password == "" || email == "")
            {
                Toast.MakeText(this, "Wpisz email lub hasło!", ToastLength.Short).Show();
                return;
            }
            else if (!email.Contains("@"))
            {
                Toast.MakeText(this, "Podaj poprawny adres email!", ToastLength.Short).Show();
                return;
            }
            else if (password.Length < 6)
            {
                Toast.MakeText(this, "Hasło jest nieprawidłowe.", ToastLength.Short).Show();
                return;
            }
            ShowProgressDialogue("Logging in...");
            loginAuth.SignInWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            CloseProgressDialogue();
            StartActivity(typeof(MainActivity));
            Toast.MakeText(this, "Logowanie powiodło się!", ToastLength.Short).Show();
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            CloseProgressDialogue();
            Toast.MakeText(this, e.Message, ToastLength.Short).Show();
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
            if(progressDialogue != null)
            {
                progressDialogue.Dismiss();
                progressDialogue = null;
            }
        }
    }
}