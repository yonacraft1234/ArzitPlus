using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArzitPlus
{
    internal class RequestEventListener : Java.Lang.Object, IEventListener
    {
        List<Request> requests = new List<Request>();

        public event EventHandler<RequestEventArgs> OnOrderRetrieved;

        public class RequestEventArgs : EventArgs
        {
            internal List<Request> Requests { get; set; }
        }

        public RequestEventListener()
        {
            AppDataHelper.GetFirestore().Collection(Request.COLLECTION_NAME).AddSnapshotListener(this);
        }
        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var snapshot = (QuerySnapshot)value;
            this.requests = new List<Request>();
            foreach (DocumentSnapshot item in snapshot.Documents)
            {
                Request request = new Request();
                if (item.Get("studentId") != null)
                {
                    request.studentId = item.Get("studentId").ToString();
                }
                else
                {
                    request.studentId = "";
                }
                if (item.Get("student") != null)
                {
                    request.student = item.Get("student").ToString();
                }
                else
                {
                    request.student = "";
                }
                if (item.Get("teacherId") != null)
                {
                    request.teacherId = item.Get("teacherId").ToString();
                }
                else
                {
                    request.teacherId = "";
                }
                if (item.Get("teacher") != null)
                {
                    request.teacher = item.Get("teacher").ToString();
                }
                else
                {
                    request.teacher = "";
                }

                if (item.Get("start") != null)
                {
                    request.start = item.Get("start").ToString();
                }
                else
                {
                    request.start = "";
                }
                if (item.Get("end") != null)
                {
                    request.end = item.Get("end").ToString();
                }
                else
                {
                    request.end = "";
                }
                if (item.Get("date1") != null)
                {
                    request.date1 = item.Get("date1").ToString();
                }
                else
                {
                    request.date1 = "";
                }
                if (item.Get("date2") != null)
                {
                    request.date2 = item.Get("date2").ToString();
                }
                else
                {
                    request.date2 = "";
                }
                if (item.Get("documentId") != null)
                {
                    request.documentId = item.Get("documentId").ToString();
                }
                else
                {
                    request.documentId = "";
                }
                this.requests.Add(request);
            }
            if (this.OnOrderRetrieved != null)
            {
                RequestEventArgs e = new RequestEventArgs();
                e.Requests = this.requests;
                OnOrderRetrieved.Invoke(this, e);
            }
        }
    }
}