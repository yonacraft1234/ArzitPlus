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
    internal class GradeEventListener : Java.Lang.Object, IEventListener
    {
        List<Grade> grades = new List<Grade>();

        public event EventHandler<GradeEventArgs> OnOrderRetrieved;

        public class GradeEventArgs : EventArgs
        {
            internal List<Grade> Grades { get; set; }
        }

        public GradeEventListener()
        {
            AppDataHelper.GetFirestore().Collection(Grade.COLLECTION_NAME).AddSnapshotListener(this);
        }

        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var snapshot = (QuerySnapshot)value;
            this.grades = new List<Grade>();
            foreach (DocumentSnapshot item in snapshot.Documents)
            {
                Grade grade = new Grade();
                if (item.Get("subject") != null)
                {
                    grade.subject = item.Get("subject").ToString();
                }
                else
                {
                    grade.subject = "";
                }
                if (item.Get("grade") != null)
                {
                    grade.grade = item.Get("grade").ToString();
                }
                else
                {
                    grade.grade = "";
                }
                if (item.Get("type") != null)
                {
                    grade.type = item.Get("type").ToString();
                }
                else
                {
                    grade.type = "";
                }
                if (item.Get("student") != null)
                {
                    grade.studentId = item.Get("student").ToString();
                }
                else
                {
                    grade.studentId = "";
                }
                this.grades.Add(grade);
            }
            if (this.OnOrderRetrieved != null)
            {
                GradeEventArgs e = new GradeEventArgs();
                e.Grades = this.grades;
                OnOrderRetrieved.Invoke(this, e);
            }
        }
    }
}