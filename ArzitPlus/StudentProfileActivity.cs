using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DE.Hdodenhof.CircleImageViewLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Support.Design.Widget.AppBarLayout;

namespace ArzitPlus
{
    [Activity(Label = "StudentProfileActivity", Theme = "@style/ProfileTheme")]
    public class StudentProfileActivity : AppCompatActivity, IOnOffsetChangedListener
    {
        AppBarLayout appBar;
        Android.Support.V7.Widget.Toolbar toolbar;
        int colorCode = 0;
        CircleImageView profileImage;
        TextView textView1, valueText1, textView2, valueText2, profileName;
        Student current = StudentHomepageActivity.currentUser;
        string id;
        List<Lesson> lessons;
        List<Homework> homeworks;
        List<Lesson> todayLessonsList;
        ListView todayLessons;
        StudentProfileAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile_layout);
            id = current.password;

            appBar = FindViewById<AppBarLayout>(Resource.Id.appbar);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = current.username;

            profileImage = FindViewById<CircleImageView>(Resource.Id.profile_image);
            profileImage.SetImageBitmap(current.profilePic);

            profileName = FindViewById<TextView>(Resource.Id.profileName);
            textView1 = FindViewById<TextView>(Resource.Id.textView1);
            valueText1 = FindViewById<TextView>(Resource.Id.valueText1);
            textView2 = FindViewById<TextView>(Resource.Id.textView2);
            valueText2 = FindViewById<TextView>(Resource.Id.valueText2);
            todayLessons = FindViewById<ListView>(Resource.Id.todayLessons);
            profileName.Text = current.username;

            LessonEventListener lessonListener = new LessonEventListener();
            lessonListener.OnOrderRetrieved += Lessonlistener_OnOrderRetrieved;

            HomeworkEventListener homeworkListener = new HomeworkEventListener();
            homeworkListener.OnOrderRetrieved += HomeworkListener_OnOrderRetrieved;

            textView1.Text = "Next Lesson";
            textView2.Text = "tasks";
            

            SetSupportActionBar(toolbar);

            appBar.AddOnOffsetChangedListener(this);

        }

        private void HomeworkListener_OnOrderRetrieved(object sender, HomeworkEventListener.HomeworkEventArgs e)
        {
            List<Homework> all = e.Homeworks;
            homeworks = new List<Homework>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].studentId == id)
                {
                    homeworks.Add(all[i]);
                }
            }
            
            valueText2.Text = homeworks.Count.ToString();
        }

        private void Lessonlistener_OnOrderRetrieved(object sender, LessonEventListener.LessonEventArgs e)
        {
            List<Lesson> all = e.Lessons;
            lessons = new List<Lesson>();
            todayLessonsList = new List<Lesson>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].studentId == id)
                {
                    lessons.Add(all[i]);
                    if (all[i].date == DateTime.Today)
                    {
                        todayLessonsList.Add(all[i]);
                    }
                }
            }
            adapter = new StudentProfileAdapter(this, todayLessonsList);
            todayLessons.Adapter = adapter;
            if (lessons.Count != 0)
            {
                valueText1.Text = lessons[0].date.ToShortDateString();
            }
            else
            {
                valueText1.Text = "";
            }
        }

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            colorCode = -verticalOffset;
            if (colorCode > 255)
            {
                colorCode = 255;
            }
            toolbar.Background.Alpha = colorCode;
            toolbar.Alpha = colorCode / 256f;
        }
    }
}