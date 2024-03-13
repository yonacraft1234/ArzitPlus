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
    internal class Message
    {
        public string sender { get; set; }
        public string receiver { get; set; }
        public string content { get; set; }
        public bool isRead { get; set; }
        public string documentId { get; set; }

        FirebaseFirestore database;
        public const string COLLECTION_NAME = "messages";
        public Message()
        {
            this.database = AppDataHelper.GetFirestore();
        }
        public Message(string content, string sender, string receiver)
        {
            this.content = content;
            this.sender = sender;
            this.receiver = receiver;
            this.isRead = false;
            this.database = AppDataHelper.GetFirestore();
        }

        public async Task<bool> SendMessage()
        {
            try
            {
                HashMap userhMap = new HashMap();
                userhMap.Put("content", this.content);
                userhMap.Put("sender", this.sender);
                userhMap.Put("receiver", this.receiver);
                userhMap.Put("isRead", this.isRead);
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
        public async Task<bool> MessageWasRead()
        {
            try
            {
                DocumentReference userReference = database.Collection(COLLECTION_NAME).Document(documentId);
                this.isRead = true;
                await userReference.Update("isRead", isRead);
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