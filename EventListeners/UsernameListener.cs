using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using FitTrackerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitTrackerAppFinal.EventListeners
{
    public class UsernameListener : Java.Lang.Object, IOnSuccessListener
    { 
        public void FetchUser()
        {
            AppDataHelper.GetFirestore().Collection("users").Document(AppDataHelper.GetFirebaseAuth().CurrentUser.Uid).Get()
                .AddOnSuccessListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            DocumentSnapshot snapshot = (DocumentSnapshot)result;
            if (snapshot.Exists())
            {
                string username = snapshot.Get("username").ToString();
                AppDataHelper.SaveUsername(username);
            }
        }
    }

}