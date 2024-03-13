using Android.Gms.Tasks;
using Android.Graphics;
using Android.Nfc;
using Android.Util;
using Android.Widget;
using DE.Hdodenhof.CircleImageViewLib;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Storage;
using Java.IO;
using Java.Lang;
using Java.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ArzitPlus
{
    internal class StudentsEventListener : Java.Lang.Object, IEventListener
    {
        List<Student> users = new List<Student>();

        public event EventHandler<StudentEventArgs> OnOrderRetrieved;

        public class StudentEventArgs : EventArgs
        {
            internal List<Student> Students { get; set; }
        }

        public StudentsEventListener()
        {
            AppDataHelper.GetFirestore().Collection(Student.COLLECTION_NAME).AddSnapshotListener(this);
        }

        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var snapshot = (QuerySnapshot)value;
            this.users = new List<Student>();
            foreach (DocumentSnapshot item in snapshot.Documents)
            {
                Student student = new Student();
                if (item.Get("email") != null)
                {
                    student.email = item.Get("email").ToString();
                }
                else
                {
                    student.email = "";
                }
                if (item.Get("phone") != null)
                {
                    student.phone = item.Get("phone").ToString();
                }
                else
                {
                    student.phone = "";
                }
                if (item.Get("name") != null)
                {
                    student.username = item.Get("name").ToString();
                }
                else
                {
                    student.username = "";
                }
                if (item.Get("password") != null)
                {
                    student.password = item.Get("password").ToString();
                }
                else
                {
                    student.password = "";
                }
                if (item.Get("imgUrl") != null)
                {
                    student.imageUrl = item.Get("imgUrl").ToString();
                }
                else 
                {
                    student.imageUrl = ""; 
                }
                Bitmap bitmap = null;
                using (var client = new WebClient())
                using (var stream = client.OpenRead(student.imageUrl))
                {
                    Bitmap image = BitmapFactory.DecodeStream(stream);
                    bitmap = Bitmap.CreateBitmap(image);
                }
                student.profilePic = bitmap;

                this.users.Add(student);
            }
            if (this.OnOrderRetrieved != null)
            {
                StudentEventArgs e = new StudentEventArgs();
                e.Students = this.users;
                OnOrderRetrieved.Invoke(this, e);
            }
        }
    }
}