using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArzitPlus
{
    [Activity(Label = "TeacherLessonsActivity")]
    public class TeacherRequestsActivity : Activity
    {
        TeacherRequestsAdapter requestsAdapter;
        ListView requestsListView;
        string id;
        List<Request> list;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.requests_layout);
            requestsListView = FindViewById<ListView>(Resource.Id.requestsListView);
            id = Intent.GetStringExtra("id");

            RequestEventListener listener = new RequestEventListener();
            listener.OnOrderRetrieved += Listener_OnOrderRetrieved;
        }
        private void Listener_OnOrderRetrieved(object sender, RequestEventListener.RequestEventArgs e)
        {
            List<Request> all = e.Requests;
            list = new List<Request>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].teacherId == id)
                {
                    list.Add(all[i]);
                }
            }
            requestsAdapter = new TeacherRequestsAdapter(this, list);
            requestsListView.Adapter = requestsAdapter;
        }
    }
}