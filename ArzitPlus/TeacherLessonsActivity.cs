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

namespace ArzitPlus
{
    [Activity(Label = "TeacherCalendarActivity")]
    public class TeacherLessonsActivity : Activity
    {
        TeacherLessonsAdapter lessonsAdapter;
        ListView lessonsListView;
        string id;
        List<Lesson> list;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.lessons_layout);
            lessonsListView = FindViewById<ListView>(Resource.Id.lessonsListView);
            id = Intent.GetStringExtra("id");

            LessonEventListener listener = new LessonEventListener();
            listener.OnOrderRetrieved += Listener_OnOrderRetrieved;
        }
        private void Listener_OnOrderRetrieved(object sender, LessonEventListener.LessonEventArgs e)
        {
            List<Lesson> all = e.Lessons;
            list = new List<Lesson>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].teacherId == id)
                {
                    list.Add(all[i]);
                }
            }
            lessonsAdapter = new TeacherLessonsAdapter(this, list);
            lessonsListView.Adapter = lessonsAdapter;
        }
    }
}