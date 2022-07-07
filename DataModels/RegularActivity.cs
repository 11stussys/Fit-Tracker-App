using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitTrackerAppFinal.DataModels
{
    public class RegularActivity : SportActivity
    {
        public int Reps { get; set; }
        public int Sets { get; set; }
        public RegularActivity(string username, string name, string type, string subtype, int calories, DateTime date, string activityID,string ownerID, string description, int reps, int sets)
        {
            this.Username = username;
            this.Name = name;
            this.Type = type;
            this.Subtype = subtype;
            this.CaloriesBurned = calories;
            this.ID = activityID;
            this.OwnerID = ownerID;
            this.ActivityDate = date;
            this.Description = description;
            this.Reps = reps;
            this.Sets = sets;
        }
    }
}