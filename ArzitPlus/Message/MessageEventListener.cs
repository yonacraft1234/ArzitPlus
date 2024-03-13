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
    internal class MessageEventListener : Java.Lang.Object, IEventListener
    {
        List<Message> messages = new List<Message>();

        public event EventHandler<MessageEventArgs> OnOrderRetrieved;

        public class MessageEventArgs : EventArgs
        {
            internal List<Message> Messages { get; set; }
        }

        public MessageEventListener()
        {
            AppDataHelper.GetFirestore().Collection(Message.COLLECTION_NAME).AddSnapshotListener(this);
        }
        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var snapshot = (QuerySnapshot)value;
            this.messages = new List<Message>();
            foreach (DocumentSnapshot item in snapshot.Documents)
            {
                Message message = new Message();
                if (item.Get("content") != null)
                {
                    message.content = item.Get("content").ToString();
                }
                else
                {
                    message.content = "";
                }
                if (item.Get("sender") != null)
                {
                    message.sender = item.Get("sender").ToString();
                }
                else
                {
                    message.sender = "";
                }
                if (item.Get("receiver") != null)
                {
                    message.receiver = item.Get("receiver").ToString();
                }
                else
                {
                    message.receiver = "";
                }
                if (item.Get("isRead") != null)
                {
                    message.isRead = (bool)item.Get("isRead");
                }
                else
                {
                    message.isRead = false;
                }
                if (item.Get("documentId") != null)
                {
                    message.documentId = item.Get("documentId").ToString();
                }
                else
                {
                    message.documentId = "";
                }
                this.messages.Add(message);
            }
            if (this.OnOrderRetrieved != null)
            {
                MessageEventArgs e = new MessageEventArgs();
                e.Messages = this.messages;
                OnOrderRetrieved.Invoke(this, e);
            }
        }
    }
}