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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArzitPlus
{
    internal class Homework
    {
        public string subject { get; set; }
        public string content { get; set; }
        public string studentId { get; set; }
        public string teacher { get; set; }
        public string documentId { get; set; }

        FirebaseFirestore database;
        public const string COLLECTION_NAME = "homeworks";
        
        public Homework()
        {
            this.database = AppDataHelper.GetFirestore();
        }
        public Homework(string subject, string content, string teacher, string studentId)
        {
            this.subject = subject;
            this.content = content;
            this.teacher = teacher;
            this.studentId = studentId;
            this.database = AppDataHelper.GetFirestore();
        }
        public async Task<bool> AddHomework()
        {
            try
            {
                HashMap userhMap = new HashMap();
                userhMap.Put("subject", this.subject);
                userhMap.Put("content", this.content);
                userhMap.Put("teacher", this.teacher);
                userhMap.Put("student", this.studentId);
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
        public async Task<bool> HomeworkDone()
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