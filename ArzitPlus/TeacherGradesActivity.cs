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
    [Activity(Label = "TeacherGradesActivity")]
    public class TeacherGradesActivity : Activity
    {
        ListView studentsListView;
        StudentAdapter studentAdapter;
        StudentsEventListener studentsEL;
        List<Student> students;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.students_list_layout);
            studentsListView = FindViewById<ListView>(Resource.Id.studentListView);
            studentsEL = new StudentsEventListener();
            studentsEL.OnOrderRetrieved += StudentEventListener_OnOrderRetrieved;
            
        }
        private void StudentEventListener_OnOrderRetrieved(object sender, StudentsEventListener.StudentEventArgs e)
        {
            students = e.Students;
            studentAdapter = new StudentAdapter(this, this.students);
            studentsListView.Adapter = studentAdapter;
        }
    }
}