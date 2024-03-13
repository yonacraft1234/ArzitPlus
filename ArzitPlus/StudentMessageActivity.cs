using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Systems;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArzitPlus
{
    [Activity(Label = "StudentMessageActivity")]
    public class StudentMessageActivity : Activity
    {
        //message list
        bool seenChecked, unseenChecked;
        RadioGroup rg;
        Button sendMsg;
        ListView msgListView;
        StudentMessageAdapter msgAdapter;
        List<Message> Msgs = new List<Message>();
        string id;
        List<Message> seenList, unseenList;
        MessageEventListener messageEventListener;

        //send message dialog
        Dialog sendMsgDialog;
        EditText content;
        Spinner receiverSpinner;
        string senderName;
        string receiver;
        List<Teacher> teachers;
        int pos;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.messages_layout);
            id = Intent.GetStringExtra("id");
            senderName = Intent.GetStringExtra("name");
            seenChecked = false;
            unseenChecked = true;

            msgListView = FindViewById<ListView>(Resource.Id.messageListView);
            sendMsg = FindViewById<Button>(Resource.Id.sendMsg);
            sendMsg.Click += SendMsg_Click;
            rg = FindViewById<RadioGroup>(Resource.Id.msgRadioGroup);
            rg.CheckedChange += Rg_CheckedChange;
            TeachersEventListener teachersEventLister = new TeachersEventListener();
            teachersEventLister.OnOrderRetrieved += TeachersEventLister_OnOrderRetrieved;

            messageEventListener = new MessageEventListener();
            messageEventListener.OnOrderRetrieved += MessageEventListener_OnOrderRetrieved;

        }

        private void Rg_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            if (e.CheckedId == Resource.Id.seenMsg)
            {
                seenChecked = true;
                unseenChecked = false;
                messageEventListener = new MessageEventListener();
                messageEventListener.OnOrderRetrieved += MessageEventListener_OnOrderRetrieved;
            }
            else if (e.CheckedId == Resource.Id.unseenMsg)
            {
                seenChecked = false;
                unseenChecked = true;
                messageEventListener = new MessageEventListener();
                messageEventListener.OnOrderRetrieved += MessageEventListener_OnOrderRetrieved;
            }
        }
        private void MessageEventListener_OnOrderRetrieved(object sender, MessageEventListener.MessageEventArgs e)
        {
            List<Message> all = e.Messages;
            Msgs = new List<Message>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].receiver == id)
                {
                    Msgs.Add(all[i]);
                }
            }

            if (unseenChecked)
            {
                UnseenList();
                msgAdapter = new StudentMessageAdapter(this, unseenList);
                msgListView.Adapter = msgAdapter;
            }
            if (seenChecked)
            {
                SeenList();
                msgAdapter = new StudentMessageAdapter(this, seenList);
                msgListView.Adapter = msgAdapter;
            }
        }
        public void SeenList()
        {
            seenList = new List<Message>();
            for (int i = 0; i < Msgs.Count; i++)
            {
                if (Msgs[i].isRead)
                {
                    seenList.Add(Msgs[i]);
                }
            }
        }
        public void UnseenList()
        {
            unseenList = new List<Message>();
            for (int i = 0; i < Msgs.Count; i++)
            {
                if (!Msgs[i].isRead)
                {
                    unseenList.Add(Msgs[i]);
                }
            }
        }

        private void TeachersEventLister_OnOrderRetrieved(object sender, TeachersEventListener.TeacherEventArgs e)
        {
            teachers = e.Teachers;
        }

        private void SendMsg_Click(object sender, EventArgs e)
        {
            sendMsgDialog = new Dialog(this);
            sendMsgDialog.SetContentView(Resource.Layout.send_message_dialog);
            sendMsgDialog.SetCancelable(true);

            content = sendMsgDialog.FindViewById<EditText>(Resource.Id.msgContent);

            //List of names of all teachers
            receiverSpinner = sendMsgDialog.FindViewById<Spinner>(Resource.Id.receiverMsgSpinner);
            List<string> names = new List<string>();
            names.Add("בחר מורה");
            for (int i = 0; i < teachers.Count; i++)
            {
                names.Add(teachers[i].username);
            }
            ArrayAdapter<string> adapter2 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, names);
            receiverSpinner.Adapter = adapter2;
            receiverSpinner.ItemSelected += ReceiverSpinner_ItemSelected;

            Button submit = sendMsgDialog.FindViewById<Button>(Resource.Id.sendFinal);
            submit.Click += Submit_Click;

            sendMsgDialog.Show();
        }

        private async void Submit_Click(object sender, EventArgs e)
        {
            if (content.Text.ToString() == "" || receiver == "בחר מורה")
            {
                Toast.MakeText(this, "Details are missing", ToastLength.Short).Show();
            }
            else
            {
                Message msg = new Message(content.Text.ToString(), senderName, teachers[pos - 1].password);
                if (await msg.SendMessage() == true)
                {
                    Toast.MakeText(this, "Message was sent", ToastLength.Short).Show();
                    sendMsgDialog.Dismiss();
                }
                else
                {
                    Toast.MakeText(this, "Message was not sent", ToastLength.Short).Show();
                }
            }
        }

        private void ReceiverSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            receiver = receiverSpinner.GetItemAtPosition(e.Position).ToString();
            pos = e.Position;
        }
    }
}