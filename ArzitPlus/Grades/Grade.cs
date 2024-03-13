using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArzitPlus
{
    internal class Grade
    {
        public string subject { get; set; }
        public string grade { get; set; }
        public string type { get; set; }
        public string studentId { get; set; }

        FirebaseFirestore database;
        public const string COLLECTION_NAME = "grades";
        public Grade()
        {
            this.database = AppDataHelper.GetFirestore();
        }
        public Grade(string subject, string grade, string type, string studentId)
        {
            this.subject = subject;
            this.grade = grade;
            this.type = type;
            this.studentId = studentId;
            this.database = AppDataHelper.GetFirestore();
        }
        public async Task<bool> AddGrade()
        {
            try
            {
                HashMap userhMap = new HashMap();
                userhMap.Put("subject", this.subject);
                userhMap.Put("grade", this.grade);
                userhMap.Put("type", this.type);
                userhMap.Put("student", this.studentId);

                DocumentReference userReference = this.database.Collection(COLLECTION_NAME).Document();
                await userReference.Set(userhMap);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }
            return true;
        }
    }
}