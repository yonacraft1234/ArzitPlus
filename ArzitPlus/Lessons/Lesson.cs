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
    internal class Lesson
    {
        public DateTime date { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string studentId { get; set; }
        public string studentName { get; set; }
        public string teacherId { get; set; }
        public string teacherName { get; set; }
        public string msg { get; set; }
        public string documentId { get; set; }

        FirebaseFirestore database;
        public const string COLLECTION_NAME = "lessons";
        public Lesson()
        {
            this.database = AppDataHelper.GetFirestore();
        }
        public Lesson(DateTime date, string start, string end, string studentId, string studentName, string teacherId, string teacherName, string msg)
        {
            this.date = date;
            this.studentId = studentId;
            this.teacherId = teacherId;
            this.start = start;
            this.end = end;
            this.studentName = studentName;
            this.teacherName = teacherName;
            this.msg = msg;
            this.database = AppDataHelper.GetFirestore();
        }
        public async Task<bool> CreateLesson()
        {
            try
            {
                HashMap userhMap = new HashMap();
                userhMap.Put("day", this.date.Day);
                userhMap.Put("month", this.date.Month);
                userhMap.Put("year", this.date.Year);
                userhMap.Put("start", this.start);
                userhMap.Put("end", this.end);
                userhMap.Put("teacherId", this.teacherId);
                userhMap.Put("teacherName", this.teacherName);
                userhMap.Put("studentId", this.studentId);
                userhMap.Put("studentName", this.studentName);
                userhMap.Put("msg", this.msg);
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
        public async Task<bool> LessonDone()
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