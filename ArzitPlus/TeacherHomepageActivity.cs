using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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
    public class TeacherHomepageActivity : AppCompatActivity
    {
        TextView helloMsg;
        //detecting the current logged in teacher
        TeachersEventListener eventListener;
        List<Teacher> teachers;
        internal static Teacher currentUser;
        ISharedPreferences sp;
        string password;


        Button msg, calendar, grades, homeworks, requests, profile;

        //list of all registered students
        StudentsEventListener studentsEL;
        internal static List<Student> students;
        //homeworks
        Dialog homeworkDialog;
        EditText homeworkMsg;
        Spinner studentSpinner, homeworkSubjectSpinner;
        string studentName, homeworkSubject;
        int pos;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.homepage);

            var serviceIntent = new Intent(this, typeof(AudioService));
            StartService(serviceIntent);

            helloMsg = FindViewById<TextView>(Resource.Id.helloMsg);
            sp = this.GetSharedPreferences(Teacher.COLLECTION_USER_FILE, FileCreationMode.Private);
            password = sp.GetString("password", "");
            eventListener = new TeachersEventListener();
            eventListener.OnOrderRetrieved += TeacherEventListener_OnOrderRetrieved;

            studentsEL = new StudentsEventListener();
            studentsEL.OnOrderRetrieved += StudentEventListener_OnOrderRetrieved;

            
            msg = FindViewById<Button>(Resource.Id.msg);
            calendar = FindViewById<Button>(Resource.Id.calendar);
            grades = FindViewById<Button>(Resource.Id.grades);
            requests = FindViewById<Button>(Resource.Id.lessons);
            homeworks = FindViewById<Button>(Resource.Id.homeworks);
            profile = FindViewById<Button>(Resource.Id.profile);


            msg.Text = "ההודעות שלי";
            calendar.Text = "יומן שיעורים";
            profile.Text = "הפרופיל שלי";
            grades.Text = "רשימת ציונים";
            requests.Text = "קביעת שיעורים";
            homeworks.Text = "יצירת מטלה";


            msg.Click += Msg_Click;
            calendar.Click += Calendar_Click;
            grades.Click += Grades_Click;
            requests.Click += Requests_Click;
            homeworks.Click += Homeworks_Click;
            profile.Click += Profile_Click;
        }

        private void Profile_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TeacherProfileActivity));
            intent.PutExtra("id", currentUser.password);
            StartActivity(intent);
        }
        //homeworks
        private void StudentEventListener_OnOrderRetrieved(object sender, StudentsEventListener.StudentEventArgs e)
        {
            students = e.Students;
        }
        private void Homeworks_Click(object sender, EventArgs e)
        {
            //init dialog
            homeworkDialog = new Dialog(this);
            homeworkDialog.SetContentView(Resource.Layout.homework_dialog);
            studentSpinner = homeworkDialog.FindViewById<Spinner>(Resource.Id.studentSpinner);
            homeworkSubjectSpinner = homeworkDialog.FindViewById<Spinner>(Resource.Id.homeworkSubject);
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.subjects, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            homeworkSubjectSpinner.Adapter = adapter;
            homeworkSubjectSpinner.ItemSelected += HomeworkSubjectSpinner_ItemSelected;

            homeworkMsg = homeworkDialog.FindViewById<EditText>(Resource.Id.homeworkMsg);

            //List of names of all students
            List<string> names = new List<string>();
            names.Add("בחר תלמיד");
            for (int i = 0; i < students.Count; i++)
            {
                names.Add(students[i].username);
            }
            ArrayAdapter<string> adapter2 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, names);
            studentSpinner.Adapter = adapter2;
            studentSpinner.ItemSelected += StudentSpinner_ItemSelected;

            Button submit = homeworkDialog.FindViewById<Button>(Resource.Id.homeworkSubmit);
            submit.Click += HomeworkSubmit_Click;
            homeworkDialog.Show();
        }
        private void HomeworkSubjectSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            homeworkSubject = homeworkSubjectSpinner.GetItemAtPosition(e.Position).ToString();
        }
        private void StudentSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            studentName = studentSpinner.GetItemAtPosition(e.Position).ToString();
            pos = e.Position;
        }
        private async void HomeworkSubmit_Click(object sender, EventArgs e)
        {
            if (homeworkSubject == "מקצוע" || studentName == "בחר תלמיד")
            {
                Toast.MakeText(this, "Details are missing", ToastLength.Short).Show();
            }
            else
            {
                Homework hm = new Homework(homeworkSubject, homeworkMsg.Text.ToString(), currentUser.username, students[pos-1].password);
                if (await hm.AddHomework() == true)
                {
                    Toast.MakeText(this, "Homework added successfully", ToastLength.Short).Show();
                    homeworkDialog.Dismiss();
                }
                else
                {
                    Toast.MakeText(this, "Homework is not added", ToastLength.Short).Show();
                }
            }
        }

        //grades
        private void Grades_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TeacherGradesActivity));
            StartActivity(intent);
        }
        //***LESSSONS***//

        //lessons requests
        private void Requests_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TeacherRequestsActivity));
            intent.PutExtra("id", currentUser.password);
            StartActivity(intent);
        }

        //lessons calendar
        private void Calendar_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TeacherLessonsActivity));
            intent.PutExtra("id", currentUser.password);
            StartActivity(intent);
        }

        //messages
        private void Msg_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TeacherMessageActivity));
            intent.PutExtra("id", currentUser.password);
            intent.PutExtra("name", currentUser.username);
            StartActivity(intent);
        }

        //finding the current login teacher
        private void TeacherEventListener_OnOrderRetrieved(object sender, TeachersEventListener.TeacherEventArgs e)
        {
            bool isRetrieved = false;
            teachers = e.Teachers;

            for (int i = 0; i < teachers.Count && !isRetrieved; i++)
            {
                if (teachers[i].password == password)
                {
                    currentUser = teachers[i];
                    isRetrieved = true;
                }
            }
            helloMsg.Text += currentUser.username;
        }
       
        //logout menu
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
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
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