using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Hardware;
using System.Runtime.Remoting.Contexts;

namespace ArzitPlus
{
    [Activity(Label = "StudentHomeworkActivity")]
    public class StudentHomeworkActivity : Activity
    {
        ListView homeworkListView;
        HomeworkAdapter hmAdapter;
        List<Homework> hms = new List<Homework>();
        string id; 
        public SensorManager sensorManager;
        public ShakeDetector shakeDetector;
        public static bool isShakeDetectorActive = false;
        internal static Homework homeworkRemove;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.student_homeworks_layout);
            sensorManager = (SensorManager)GetSystemService(SensorService);
            homeworkListView = FindViewById<ListView>(Resource.Id.homeworkListView);
            id = Intent.GetStringExtra("id");
            HomeworkEventListener homeworkEventListener = new HomeworkEventListener();
            homeworkEventListener.OnOrderRetrieved += HomeworkEventListener_OnOrderRetrieved;
        }

        private void HomeworkEventListener_OnOrderRetrieved(object sender, HomeworkEventListener.HomeworkEventArgs e)
        {
            hms = e.Homeworks;
            List<Homework> homeworks = new List<Homework>();
            for (int i = 0; i < hms.Count; i++)
            {
                if (hms[i].studentId == id)
                {
                    homeworks.Add(hms[i]);
                }
            }
            if (homeworks.Count == 0)
            {
                Finish();
                Toast.MakeText(this, "You don't have homework", ToastLength.Short).Show();
            }
            else 
            { 
                hmAdapter = new HomeworkAdapter(this, homeworks, sensorManager);
                homeworkListView.Adapter = hmAdapter;
            }
        }
    }
}