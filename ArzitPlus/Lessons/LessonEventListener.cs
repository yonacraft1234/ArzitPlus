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
    internal class LessonEventListener : Java.Lang.Object, IEventListener
    {
        List<Lesson> lessons = new List<Lesson>();

        public event EventHandler<LessonEventArgs> OnOrderRetrieved;

        public class LessonEventArgs : EventArgs
        {
            internal List<Lesson> Lessons { get; set; }
        }

        public LessonEventListener()
        {
            AppDataHelper.GetFirestore().Collection(Lesson.COLLECTION_NAME).AddSnapshotListener(this);
        }
        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var snapshot = (QuerySnapshot)value;
            this.lessons = new List<Lesson>();
            foreach (DocumentSnapshot item in snapshot.Documents)
            {
                Lesson lesson = new Lesson();
                if (item.Get("msg") != null)
                {
                    lesson.msg = item.Get("msg").ToString();
                }
                else
                {
                    lesson.msg = "";
                }
                if (item.Get("studentId") != null)
                {
                    lesson.studentId = item.Get("studentId").ToString();
                }
                else
                {
                    lesson.studentId = "";
                }
                if (item.Get("teacherId") != null)
                {
                    lesson.teacherId = item.Get("teacherId").ToString();
                }
                else
                {
                    lesson.teacherId = "";
                }
                if (item.Get("studentName") != null)
                {
                    lesson.studentName = item.Get("studentName").ToString();
                }
                else
                {
                    lesson.studentName = "";
                }
                if (item.Get("teacherName") != null)
                {
                    lesson.teacherName = item.Get("teacherName").ToString();
                }
                else
                {
                    lesson.teacherName = "";
                }
                if (item.Get("start") != null)
                {
                    lesson.start = item.Get("start").ToString();
                }
                else
                {
                    lesson.start = "";
                }
                if (item.Get("end") != null)
                {
                    lesson.end = item.Get("end").ToString();
                }
                else
                {
                    lesson.end = "";
                }
                if (item.Get("day") != null && item.Get("month") != null && item.Get("year") != null)
                {
                    DateTime date = new DateTime((int)item.Get("year"), (int)item.Get("month"), (int)item.Get("day"));
                    lesson.date = date;
                }
                
                if (item.Get("documentId") != null)
                {
                    lesson.documentId = item.Get("documentId").ToString();
                }
                else
                {
                    lesson.documentId = "";
                }
                this.lessons.Add(lesson);
            }
            if (this.OnOrderRetrieved != null)
            {
                LessonEventArgs e = new LessonEventArgs();
                e.Lessons = this.lessons;
                OnOrderRetrieved.Invoke(this, e);
            }
        }
    }
}