using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitTrackerApp.Helpers
{
    public static class AppDataHelper
    {
        static ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        static ISharedPreferencesEditor editor;
        public static FirebaseFirestore GetFirestore()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseFirestore database;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("fittrackerapp-5c269")
                    .SetApplicationId("fittrackerapp-5c269")
                    .SetApiKey("AIzaSyCv6Auw11W0uUh2v5QyZJBZS2-VmxqODFI")
                    .SetDatabaseUrl("https://fittrackerapp-5c269-default-rtdb.europe-west1.firebasedatabase.app")
                    .SetStorageBucket("fittrackerapp-5c269.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                database = FirebaseFirestore.GetInstance(app);
            }
            else
            {
                database = FirebaseFirestore.GetInstance(app);
            }

            return database;
        }

        public static FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth userAuth;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("fittrackerapp-5c269")
                    .SetApplicationId("fittrackerapp-5c269")
                    .SetApiKey("AIzaSyCv6Auw11W0uUh2v5QyZJBZS2-VmxqODFI")
                    .SetDatabaseUrl("https://fittrackerapp-5c269-default-rtdb.europe-west1.firebasedatabase.app")
                    .SetStorageBucket("fittrackerapp-5c269.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                userAuth = FirebaseAuth.Instance;
            }
            else
            {
                userAuth = FirebaseAuth.Instance;
            }

            return userAuth;
        }

        public static void SaveUsername(string username)
        {
            editor = preferences.Edit();
            editor.PutString("username", username);
            editor.Apply();
        }

        public static string GetUsername()
        {
            string username = "";
            username = preferences.GetString("username", "");
            return username;
        }
    }
}