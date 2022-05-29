using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using FitTrackerApp.Helpers;
using FitTrackerAppFinal.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitTrackerAppFinal.EventListeners
{
    public class SportActivityEventListener : Java.Lang.Object, IOnSuccessListener
    {
        public List<SportActivity> listOfActivities = new List<SportActivity>();
        public event EventHandler<ActivityEventArgs> OnActivityRetrieved;

        public class ActivityEventArgs : EventArgs
        {
            public List<SportActivity> SportActivities { get; set; }
        }

        public void FetchSportActivity()
        {
            AppDataHelper.GetFirestore().Collection("activities").Get()
                .AddOnSuccessListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var snapshot = (QuerySnapshot)result;

            if (!snapshot.IsEmpty)
            {
                foreach (DocumentSnapshot item in snapshot.Documents)
                {
                    string userId = AppDataHelper.GetFirebaseAuth().CurrentUser.Uid;

                    string username = item.Get("user").ToString();
                    string name = item.Get("activity_name").ToString();
                    string type = item.Get("activity_type").ToString();
                    string subtype = item.Get("activity_subtype").ToString();
                    int calories = (int)item.Get("calories_burned");
                    string dateString = item.Get("activity_date").ToString();
                    DateTime date = Convert.ToDateTime(dateString);
                    string activityOwnerId = item.Get("owner_id").ToString();
                    string description = item.Get("activity_description").ToString();
                    try
                    {
                        if (userId == activityOwnerId)
                        {
                            if (type == "Bieganie" || type == "Spacer" || type == "Trening siłowy")
                            {
                                int duration = Convert.ToInt32(item.Get("activity_duration"));
                                double distance = Convert.ToDouble(item.Get("activity_distance"));
                                RunningActivity runningActivity = new RunningActivity(username, name, type, subtype, calories, date, activityOwnerId, description, duration, distance);
                                listOfActivities.Add(runningActivity);
                            }
                            else
                            {
                                int sets = Convert.ToInt32(item.Get("activity_sets"));
                                int reps = Convert.ToInt32(item.Get("activity_reps"));
                                RegularActivity regularActivity = new RegularActivity(username, name, type, subtype, calories, date, activityOwnerId, description, sets, reps);
                                listOfActivities.Add(regularActivity);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Wystąpił błąd w odczytywaniu aktywności: " + ex.ToString());
                    }
                }

                OnActivityRetrieved?.Invoke(this, new ActivityEventArgs { SportActivities = listOfActivities });
            }
        }
    }
}