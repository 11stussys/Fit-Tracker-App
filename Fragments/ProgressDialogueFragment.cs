using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitTrackerAppFinal.Fragments
{
    public class ProgressDialogueFragment : AndroidX.Fragment.App.DialogFragment
    {
        string status;
        public ProgressDialogueFragment(string _status)
        {
            status = _status;
        }
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.progress_bar, container, false);
            TextView progressStatus = view.FindViewById<TextView>(Resource.Id.progressStatus);
            progressStatus.Text = status;
            return view;
        }
    }
}