using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ArzitPlus
{
    internal class HomeworkEventListener : Java.Lang.Object, IEventListener
    {
        List<Homework> homeworks = new List<Homework>();

        public event EventHandler<HomeworkEventArgs> OnOrderRetrieved;

        public class HomeworkEventArgs : EventArgs
        {
            internal List<Homework> Homeworks { get; set; }
        }

        public HomeworkEventListener()
        {
            AppDataHelper.GetFirestore().Collection(Homework.COLLECTION_NAME).AddSnapshotListener(this);
        }

        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var snapshot = (QuerySnapshot)value;
            this.homeworks = new List<Homework>();
            foreach (DocumentSnapshot item in snapshot.Documents)
            {
                Homework homework = new Homework();
                if (item.Get("subject") != null)
                {
                    homework.subject = item.Get("subject").ToString();
                }
                else
                {
                    homework.subject = "";
                }
                if (item.Get("content") != null)
                {
                    homework.content = item.Get("content").ToString();
                }
                else
                {
                    homework.content = "";
                }
                if (item.Get("teacher") != null)
                {
                    homework.teacher = item.Get("teacher").ToString();
                }
                else
                {
                    homework.teacher = "";
                }
                if (item.Get("student") != null)
                {
                    homework.studentId = item.Get("student").ToString();
                }
                else
                {
                    homework.studentId = "";
                }
                if (item.Get("documentId") != null)
                {
                    homework.documentId = item.Get("documentId").ToString();
                }
                else
                {
                    homework.documentId = "";
                }
                this.homeworks.Add(homework);
            }
            if (this.OnOrderRetrieved != null)
            {
                HomeworkEventArgs e = new HomeworkEventArgs();
                e.Homeworks = this.homeworks;
                OnOrderRetrieved.Invoke(this, e);
            }
        }
    }
}