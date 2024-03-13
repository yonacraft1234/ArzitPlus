using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Systems;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Javax.Security.Auth;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

namespace ArzitPlus
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class StudentHomepageActivity : AppCompatActivity
    {
        TextView helloMsg;

        //detecting the current logged in student
        StudentsEventListener studentsEventListener;
        List<Student> students;
        internal static Student currentUser;
        ISharedPreferences sp;
        string password;

        Button msg, calendar, grades, homeworks, requests, profile;

        //grades dialog
        Dialog gradeDialog;
        EditText gradeNum;
        Spinner testTypeSpinner, gradeSubject;
        string testType, subject;

        //requests
        Dialog requestDialog;
        TeachersEventListener teachersEventListener;
        List<Teacher> teachers;
        Button date1, date2, start, end;
        Spinner teachersSpinner;
        string teacherName;
        int pos;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.homepage);

            var serviceIntent = new Intent(this, typeof(AudioService));
            StartService(serviceIntent);

            helloMsg = FindViewById<TextView>(Resource.Id.helloMsg);
            sp = this.GetSharedPreferences(Student.COLLECTION_USER_FILE, FileCreationMode.Private);
            password = sp.GetString("password", "");
            studentsEventListener = new StudentsEventListener();
            studentsEventListener.OnOrderRetrieved += StudenteventListener_OnOrderRetrieved;
            
            teachersEventListener = new TeachersEventListener();
            teachersEventListener.OnOrderRetrieved += TeachersEventListener_OnOrderRetrieved;

            msg = FindViewById<Button>(Resource.Id.msg);
            calendar = FindViewById<Button>(Resource.Id.calendar);
            grades = FindViewById<Button>(Resource.Id.grades);
            requests = FindViewById<Button>(Resource.Id.lessons);
            homeworks = FindViewById<Button>(Resource.Id.homeworks);
            profile = FindViewById<Button>(Resource.Id.profile);


            msg.Text = "ההודעות שלי";
            calendar.Text = "יומן שיעורים";
            profile.Text = "הפרופיל שלי";
            grades.Text = "הזנת ציונים";
            requests.Text = "בקשת שיעור";
            homeworks.Text = "שיעורי בית";


            msg.Click += Msg_Click;
            calendar.Click += Calendar_Click;
            grades.Click += Grades_Click;
            requests.Click += Requests_Click;
            homeworks.Click += Homeworks_Click;
            profile.Click += Profile_Click;
        }

        private void TeachersEventListener_OnOrderRetrieved(object sender, TeachersEventListener.TeacherEventArgs e)
        {
            teachers = e.Teachers;
        }

        public void FindCurrentUser()
        {
            bool isRetrieved = false;
            for (int i = 0; i < students.Count && !isRetrieved; i++)
            {
                if (students[i].password == password)
                {
                    currentUser = students[i];
                    isRetrieved = true;
                }
            }
            helloMsg.Text += currentUser.username;
        }

        private void StudenteventListener_OnOrderRetrieved(object sender, StudentsEventListener.StudentEventArgs e)
        {
            students = e.Students;
            FindCurrentUser();
        }

        //profile
        private void Profile_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(StudentProfileActivity));
            intent.PutExtra("id", currentUser.password);
            StartActivity(intent);
        }
        //homeworks
        private void Homeworks_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(StudentHomeworkActivity));
            intent.PutExtra("id", currentUser.password);
            StartActivity(intent);
        }

        //lessons (request and approve)
        private void Requests_Click(object sender, EventArgs e)
        {
            requestDialog = new Dialog(this);
            requestDialog.SetContentView(Resource.Layout.student_request_dialog);
            requestDialog.SetCancelable(true);

            date1 = requestDialog.FindViewById<Button>(Resource.Id.studentRequestDate1);
            date2 = requestDialog.FindViewById<Button>(Resource.Id.studentRequestDate2);
            date1.Click += Date1_Click;
            date2.Click += Date2_Click;

            start = requestDialog.FindViewById<Button>(Resource.Id.studentRequestStart);
            end = requestDialog.FindViewById<Button>(Resource.Id.studentRequestEnd);
            start.Click += Start_Click;
            end.Click += End_Click;

            teachersSpinner = requestDialog.FindViewById<Spinner>(Resource.Id.requestTeacherSpinner);
            List<string> names = new List<string>();
            names.Add("בחר מורה");
            for (int i = 0; i < teachers.Count; i++)
            {
                names.Add(teachers[i].username);
            }
            ArrayAdapter<string> adapter2 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, names);
            teachersSpinner.Adapter = adapter2;
            teachersSpinner.ItemSelected += TeachersSpinner_ItemSelected;

            Button request = requestDialog.FindViewById<Button>(Resource.Id.requestBtn);
            request.Click += Request_Click;

            requestDialog.Show();
        }
        private void TeachersSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            teacherName = teachersSpinner.GetItemAtPosition(e.Position).ToString();
            pos = e.Position;
        }
        private void End_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            TimePickerDialog timePickerDialog = new TimePickerDialog(this, OnEndSet, today.Hour, today.Minute, true);
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
            TimePickerDialog timePickerDialog = new TimePickerDialog(this, OnStartSet, today.Hour, today.Minute, true);
            timePickerDialog.Show();
        }
        void OnStartSet(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string str = e.HourOfDay + ":" + e.Minute;
            start.Text = str;
        }
        private void Date2_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            DatePickerDialog datePickerDialog = new DatePickerDialog(this, OnDate2Set, today.Year, today.Month - 1, today.Day);
            datePickerDialog.Show();
        }
        void OnDate2Set(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            string str = e.Date.ToShortDateString();
            date2.Text = str;

        }
        private void Date1_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            DatePickerDialog datePickerDialog = new DatePickerDialog(this, OnDate1Set, today.Year, today.Month - 1, today.Day);
            datePickerDialog.Show();
        }
        void OnDate1Set(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            string str = e.Date.ToShortDateString();
            date1.Text = str;

        }
        private async void Request_Click(object sender, EventArgs e)
        {
            if (date1.Text =="select date" || date2.Text == "select date" || start.Text == "start" || end.Text == "end" || teacherName == "בחר מורה")
            {
                Toast.MakeText(this, "Details are missing", ToastLength.Short).Show();
            }
            else
            {
                Request request = new Request(date1.Text, date2.Text, start.Text, end.Text, currentUser.password, currentUser.username, teachers[pos - 1].password, teachers[pos - 1].username);
                if (await request.CreateRequest() == true)
                {
                    Toast.MakeText(this, "Request added successfully", ToastLength.Short).Show();
                    requestDialog.Dismiss();
                }
                else
                {
                    Toast.MakeText(this, "Request is not added", ToastLength.Short).Show();
                }
            }
        }

        //grades
        private void Grades_Click(object sender, EventArgs e)
        {
            //דיאלוג הוספת ציון תלמיד
            gradeDialog = new Dialog(this);
            gradeDialog.SetContentView(Resource.Layout.student_addgrade_dialog);
            gradeDialog.SetCancelable(true);
            gradeSubject = gradeDialog.FindViewById<Spinner>(Resource.Id.gradeSubject);
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.subjects, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            gradeSubject.Adapter = adapter;
            gradeSubject.ItemSelected += GradeSubject_ItemSelected;

            gradeNum = gradeDialog.FindViewById<EditText>(Resource.Id.gradeNum);

            testTypeSpinner = gradeDialog.FindViewById<Spinner>(Resource.Id.testTypeSpinner);
            ArrayAdapter adapter2 = ArrayAdapter.CreateFromResource(this, Resource.Array.testType, Android.Resource.Layout.SimpleSpinnerItem);
            adapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            testTypeSpinner.Adapter = adapter2;
            testTypeSpinner.ItemSelected += TestTypeSpinner_ItemSelected;
            Button submit = gradeDialog.FindViewById<Button>(Resource.Id.addGradeSubmit);
            submit.Click += GradeSubmit_Click;

            gradeDialog.Show();
        }
        private async void GradeSubmit_Click(object sender, EventArgs e)
        {
            if (subject == "מקצוע" || testType == "סוג הבחינה")
            {
                Toast.MakeText(this, "Details are missing", ToastLength.Short).Show();
            }
            else
            {
                Grade grade = new Grade(subject, gradeNum.Text.ToString(), testType, currentUser.password);
                if (await grade.AddGrade() == true)
                {
                    Toast.MakeText(this, "Grade added successfully", ToastLength.Short).Show();
                    gradeDialog.Dismiss();
                }
                else
                {
                    Toast.MakeText(this, "Grade is not added", ToastLength.Short).Show();
                }
            }
        }
        private void GradeSubject_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            subject = gradeSubject.GetItemAtPosition(e.Position).ToString();
        }
        private void TestTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            testType = testTypeSpinner.GetItemAtPosition(e.Position).ToString();
        }

        //lessons list
        private void Calendar_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(StudentLessonsActivity));
            intent.PutExtra("id", currentUser.password);
            StartActivity(intent);
        }

        //messeges
        private void Msg_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(StudentMessageActivity));
            intent.PutExtra("id", currentUser.password);
            intent.PutExtra("name", currentUser.username);
            StartActivity(intent);
        }

        //logout
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.auth_menu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == Resource.Id.logout)
            {
                Logout();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        public async void Logout()
        {
            if (await currentUser.Logout() == true)
            {
                Toast.MakeText(this, "Logout Succeeded", ToastLength.Short).Show();
            }
            Finish();
        }
        protected override void OnPause()
        {
            base.OnPause();
            var serviceIntent = new Intent(this, typeof(AudioService));
            serviceIntent.PutExtra("Action", "Pause");
            StartService(serviceIntent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var serviceIntent = new Intent(this, typeof(AudioService));
            serviceIntent.PutExtra("Action", "Resume");
            StartService(serviceIntent);
        }
    }
}