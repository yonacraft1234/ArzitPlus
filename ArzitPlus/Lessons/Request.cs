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
    internal class Request
    {
        public string date1 { get; set; }
        public string date2 { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string student { get; set; }
        public string teacher { get; set; }
        public string studentId { get; set; }
        public string teacherId { get; set; }
        public string documentId { get; set; }

        FirebaseFirestore database;
        public const string COLLECTION_NAME = "requests";
        public Request()
        {
            this.database = AppDataHelper.GetFirestore();
        }
        public Request(string date1, string date2, string startHour, string endHour, string studentId, string student, string teacherId, string teacher)
        {
            this.date1 = date1;
            this.date2 = date2;
            this.student = student;
            this.studentId = studentId;
            this.teacher = teacher;
            this.teacherId = teacherId;
            this.start = startHour;
            this.end = endHour;
            this.database = AppDataHelper.GetFirestore();
        }
        public async Task<bool> CreateRequest()
        {
            try
            {
                HashMap userhMap = new HashMap();
                userhMap.Put("date1", this.date1);
                userhMap.Put("date2", this.date2);
                userhMap.Put("start", this.start);
                userhMap.Put("end", this.end);
                userhMap.Put("teacherId", this.teacherId);
                userhMap.Put("teacher", this.teacher);
                userhMap.Put("studentId", this.studentId);
                userhMap.Put("student", this.student);
                userhMap.Put("documentId", "");

                DocumentReference userReference = this.database.Collection(COLLECTION_NAME).Document();
                await userReference.Set(userhMap);
                await userReference.Update("documentId", userReference.Id);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }
            return true;
        }
        public async Task<bool> RequestApproved()
        {
            try
            {
                DocumentReference userReference = database.Collection(COLLECTION_NAME).Document(documentId);
                await userReference.Delete();
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
