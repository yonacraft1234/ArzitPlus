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
    internal class TeachersEventListener : Java.Lang.Object, IEventListener
    {
        List<Teacher> teachers = new List<Teacher>();

        public event EventHandler<TeacherEventArgs> OnOrderRetrieved;

        public class TeacherEventArgs : EventArgs
        {
            internal List<Teacher> Teachers { get; set; }
        }

        public TeachersEventListener()
        {
            AppDataHelper.GetFirestore().Collection(Teacher.COLLECTION_NAME).AddSnapshotListener(this);
        }

        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var snapshot = (QuerySnapshot)value;
            this.teachers = new List<Teacher>();
            foreach (DocumentSnapshot item in snapshot.Documents)
            {
                Teacher teacher = new Teacher();
                if (item.Get("email") != null)
                {
                    teacher.email = item.Get("email").ToString();
                }
                else
                {
                    teacher.email = "";
                }
                if (item.Get("phone") != null)
                {
                    teacher.phone = item.Get("phone").ToString();
                }
                else
                {
                    teacher.phone = "";
                }
                if (item.Get("name") != null)
                {
                    teacher.username = item.Get("name").ToString();
                }
                else
                {
                    teacher.username = "";
                }
                if (item.Get("password") != null)
                {
                    teacher.password = item.Get("password").ToString();
                }
                else
                {
                    teacher.password = "";
                }
                if (item.Get("imgUrl") != null)
                {
                    teacher.imageUrl = item.Get("imgUrl").ToString();
                }
                else
                {
                    teacher.imageUrl = "";
                }
                Bitmap bitmap = null;
                using (var client = new WebClient())
                using (var stream = client.OpenRead(teacher.imageUrl))
                {
                    Bitmap image = BitmapFactory.DecodeStream(stream);
                    bitmap = Bitmap.CreateBitmap(image);
                }
                teacher.profilePic = bitmap;

                this.teachers.Add(teacher);
            }
            if (this.OnOrderRetrieved != null)
            {
                TeacherEventArgs e = new TeacherEventArgs();
                e.Teachers = this.teachers;
                OnOrderRetrieved.Invoke(this, e);
            }
        }
    }
}