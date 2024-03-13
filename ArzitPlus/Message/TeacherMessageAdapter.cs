using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArzitPlus
{
    internal class TeacherMessageAdapter : BaseAdapter<Message>
    {
        Context context;
        List<Message> objects;
        Button details;
        Dialog contentDialog;

        public TeacherMessageAdapter(Context context, List<Message> objects)
        {
            this.context = context;
            this.objects = objects;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override Message this[int index]
        {
            get { return this.objects[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((TeacherMessageActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.message_row_layout, parent, false);
            TextView sender = view.FindViewById<TextView>(Resource.Id.messageSenderRow);
            details = view.FindViewById<Button>(Resource.Id.messageDetailsRow);
            
            Message msg = objects[position];

            if (msg != null)
            {
                sender.Text += msg.sender;
                details.Tag = position;
                details.Click += Details_Click;
            }
            return view;
        }

        private void Details_Click(object sender, EventArgs e)
        {
            Button btnOrigin = (Button)sender;
            int pos = (int)btnOrigin.Tag;
            Message msg = objects[pos];
            contentDialog = new Dialog(context);

            contentDialog.SetContentView(Resource.Layout.message_details);
            TextView content = contentDialog.FindViewById<TextView>(Resource.Id.messageContent);
            content.Text = msg.content;
            
            Button seen = contentDialog.FindViewById<Button>(Resource.Id.messageSeenBtn);
            seen.Click += Seen_Click;
            if (msg.isRead)
            {
                seen.Visibility = ViewStates.Invisible;
            }
            contentDialog.Show();
        }

        private async void Seen_Click(object sender, EventArgs e)
        {
            Button btnOrigin = (Button)sender;
            int pos = (int)btnOrigin.Tag;
            Message msg = objects[pos];
            try
            {
                if (await msg.MessageWasRead() == true)
                {
                    NotifyDataSetChanged();
                    if (Count == 0)
                    {
                        NotifyAll();
                    }
                    contentDialog.Dismiss();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(context, "Error", ToastLength.Short).Show();
            }
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return objects.Count;
            }
        }

    }

    internal class TeacherMessageAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}