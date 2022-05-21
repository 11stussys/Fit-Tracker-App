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
    public class SportActivity
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public int CaloriesBurned { get; set; }
        public DateTime ActivityDate { get; set; }
        public string ID { get; set; }
        public string OwnerID { get; set; }
        public string Description { get; set; }
    }
}