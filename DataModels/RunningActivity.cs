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
    public class RunningActivity : SportActivity
    {
        public int Duration { get; set; }

        public RunningActivity(string username, string name, string type, DateTime date, string ID, string ownerID, string description, int duration)
        {
            this.Username = username;
            this.Name = name;
            this.Type = type;
            this.ID = ID;
            this.OwnerID = ownerID;
            this.ActivityDate = date;
            this.Description = description;
            this.Duration = duration;
        }

    }
}