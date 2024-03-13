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
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using static Android.Support.Design.Widget.AppBarLayout;
using Android.Graphics;
using DE.Hdodenhof.CircleImageViewLib;

namespace ArzitPlus
{
    [Activity(Label = "TeacherProfileActivity", Theme = "@style/ProfileTheme")]
    public class TeacherProfileActivity : AppCompatActivity, IOnOffsetChangedListener
    {
        AppBarLayout appBar;
        Android.Support.V7.Widget.Toolbar toolbar;
        int colorCode = 0;
        CircleImageView profileImage;
        TextView textView1, valueText1, textView2, valueText2, profileName;
        Teacher current = TeacherHomepageActivity.currentUser;
        string id;
        List<Request> requests;
        List<Message> unreadMessages;
        List<Lesson> lessons;
        ListView todayLessons;
        TeacherProfileAdapter adapter;

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

            RequestEventListener requestListener = new RequestEventListener();
            requestListener.OnOrderRetrieved += RequestListener_OnOrderRetrieved;

            MessageEventListener msgListener = new MessageEventListener();
            msgListener.OnOrderRetrieved += MsgListener_OnOrderRetrieved;

            LessonEventListener lessonEventListener = new LessonEventListener();
            lessonEventListener.OnOrderRetrieved += LessonEventListener_OnOrderRetrieved;

            textView1.Text = "waiting for approvement";
            textView2.Text = "Unread messages";
            

            SetSupportActionBar(toolbar);

            appBar.AddOnOffsetChangedListener(this);
        }

        private void LessonEventListener_OnOrderRetrieved(object sender, LessonEventListener.LessonEventArgs e)
        {
            List<Lesson> all = e.Lessons;
            lessons = new List<Lesson>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].teacherId == id && all[i].date == DateTime.Today)
                {
                    lessons.Add(all[i]);
                }
            }
            adapter = new TeacherProfileAdapter(this, lessons);
            todayLessons.Adapter = adapter;
            
        }

        private void RequestListener_OnOrderRetrieved(object sender, RequestEventListener.RequestEventArgs e)
        {
            List<Request> all = e.Requests;
            requests = new List<Request>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].teacherId == id)
                {
                    requests.Add(all[i]);
                }
            }
            
            valueText1.Text = requests.Count.ToString();
        }

        private void MsgListener_OnOrderRetrieved(object sender, MessageEventListener.MessageEventArgs e)
        {
            List<Message> all = e.Messages;
            unreadMessages = new List<Message>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].receiver == id && all[i].isRead == false)
                {
                    unreadMessages.Add(all[i]);
                }
            }
            
            valueText2.Text = unreadMessages.Count.ToString();
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