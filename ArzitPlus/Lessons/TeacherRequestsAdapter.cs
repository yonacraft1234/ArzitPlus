using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Systems;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArzitPlus
{
    internal class TeacherRequestsAdapter : BaseAdapter<Request>
    {

        Context context;
        List<Request> objects;
        Button set;
        Dialog setLessonDialog;
        Button date, start, end;
        EditText msgText;
        Request request;
        DateTime dateTime;
        List<Lesson> lessons = MainActivity.all;

        public TeacherRequestsAdapter(Context context, List<Request> objects)
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
        public override Request this[int index]
        {
            get { return this.objects[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((TeacherRequestsActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.request_row_layout, parent, false);
            TextView student = view.FindViewById<TextView>(Resource.Id.requestStudent);
            TextView rangeHours = view.FindViewById<TextView>(Resource.Id.requestHours);
            TextView date1 = view.FindViewById<TextView>(Resource.Id.requestDate1);
            TextView date2 = view.FindViewById<TextView>(Resource.Id.requestDate2);

            set = view.FindViewById<Button>(Resource.Id.SetLessonBtn);

            Request request = objects[position];

            if (request != null)
            {
                student.Text = request.student;
                rangeHours.Text = request.start + " - " + request.end;
                date1.Text = request.date1;
                date2.Text = request.date2;

                set.Tag = position;
                set.Click += Set_Click;
            }
            return view;
        }

        private void Set_Click(object sender, EventArgs e)
        {
            Button btnOrigin = (Button)sender;
            int pos = (int)btnOrigin.Tag;
            request = objects[pos];
            setLessonDialog = new Dialog(context);
            setLessonDialog.SetContentView(Resource.Layout.teacher_request_dialog);
            date = setLessonDialog.FindViewById<Button>(Resource.Id.teacherRequestDate);
            start = setLessonDialog.FindViewById<Button>(Resource.Id.teacherRequestStart);
            end = setLessonDialog.FindViewById<Button>(Resource.Id.teacherRequestEnd);
            msgText = setLessonDialog.FindViewById<EditText>(Resource.Id.lessonMsg);

            date.Click += Date_Click;
            start.Click += Start_Click;
            end.Click += End_Click;

            Button setLesson = setLessonDialog.FindViewById<Button>(Resource.Id.requestSetBtn);
            setLesson.Click += SetLesson_Click;
            
            setLessonDialog.Show();
        }
        private async void SetLesson_Click(object sender, EventArgs e)
        {
            if (date.Text == "select date" || start.Text == "start" || end.Text == "end" || msgText.Text == "")
            {
                Toast.MakeText(context, "Details are missing", ToastLength.Short).Show();
            }
            else
            {
                bool isExist = false;
                for (int i = 0; i < lessons.Count && !isExist; i++)
                {
                    if (lessons[i].start == start.Text || lessons[i].end == end.Text)
                    {
                        isExist = true;
                    }
                }
                if (isExist)
                {
                    Toast.MakeText(context, "The time you selected is taken", ToastLength.Short).Show();
                }
                else
                {
                    Lesson lesson = new Lesson(dateTime, start.Text, end.Text, request.studentId, request.student, request.teacherId, request.teacher, msgText.Text);
                    if (await lesson.CreateLesson() == true)
                    {
                        Toast.MakeText(context, "Lesson added successfully", ToastLength.Short).Show();
                        setLessonDialog.Dismiss();
                        await request.RequestApproved();
                    }
                    else
                    {
                        Toast.MakeText(context, "Lesson is not added", ToastLength.Short).Show();
                    }
                }
            }
        }
        private void End_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            TimePickerDialog timePickerDialog = new TimePickerDialog(context, OnEndSet, today.Hour, today.Minute, true);
            timePickerDialog.Show();
        }
        void OnEndSet(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string str = e.HourOfDay + ":" + e.Minute;
            end.Text = str;
        }
        private void Start_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            TimePickerDialog timePickerDialog = new TimePickerDialog(context, OnStartSet, today.Hour, today.Minute, true);
            timePickerDialog.Show();
        }
        void OnStartSet(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string str = e.HourOfDay + ":" + e.Minute;
            start.Text = str;

        }
        private void Date_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            DatePickerDialog datePickerDialog = new DatePickerDialog(context, OnDateSet, today.Year, today.Month - 1, today.Day);
            datePickerDialog.Show();
        }
        void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            string str = e.Date.ToShortDateString();
            date.Text = str;
            dateTime = e.Date;
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

    internal class TeacherRequestsAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}