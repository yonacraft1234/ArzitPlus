﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Icu.Text.Transliterator;

namespace ArzitPlus
{
    internal class StudentLessonsAdapter : BaseAdapter<Lesson>
    {

        Context context;
        List<Lesson> objects;
        Dialog lessonMsg;

        public StudentLessonsAdapter(Context context, List<Lesson> objects)
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
        public override Lesson this[int index]
        {
            get { return this.objects[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((StudentLessonsActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.lesson_row_layout, parent, false);
            TextView teacher = view.FindViewById<TextView>(Resource.Id.lessonName);
            TextView rangeHour = view.FindViewById<TextView>(Resource.Id.lessonHours);
            TextView date = view.FindViewById<TextView>(Resource.Id.lessonDate);
            Button details = view.FindViewById<Button>(Resource.Id.lessonDetails);
            Lesson lesson = objects[position];
            
            if (lesson != null)
            {
                teacher.Text = lesson.teacherName;
                rangeHour.Text = lesson.start + " - " + lesson.end;
                date.Text = lesson.date.ToShortDateString();
                details.Tag = position;
                details.Click += Details_Click;
            }
            return view;
        }

        private void Details_Click(object sender, EventArgs e)
        {
            Button btnOrigin = (Button)sender;
            int pos = (int)btnOrigin.Tag;
            Lesson lesson = objects[pos];
            lessonMsg = new Dialog(context);
            lessonMsg.SetContentView(Resource.Layout.lesson_details_dialog);
            TextView content = lessonMsg.FindViewById<TextView>(Resource.Id.lessonDetails);
            content.Text = lesson.msg;
            Button back = lessonMsg.FindViewById<Button>(Resource.Id.lessonDetailsBack);
            back.Click += Back_Click;
            lessonMsg.Show();
        }

        private void Back_Click(object sender, EventArgs e)
        {
            lessonMsg.Dismiss();
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

    internal class StudentLessonsAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}