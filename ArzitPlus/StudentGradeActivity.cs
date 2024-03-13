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
    [Activity(Label = "StudentGradesActivity")]
    public class StudentGradeActivity : Activity
    {
        ListView gradelistView;
        GradeAdapter gradeAdapter;
        string id;
        //List<Grade> studentGrades = new List<Grade>();
        //List<Grade> all;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.student_grades_layout);
            gradelistView = FindViewById<ListView>(Resource.Id.gradeListView);
            id = Intent.GetStringExtra("id");
            GradeEventListener gradeEventListener = new GradeEventListener();
            gradeEventListener.OnOrderRetrieved += GradeEventListener_OnOrderRetrieved;
        }

        private void GradeEventListener_OnOrderRetrieved(object sender, GradeEventListener.GradeEventArgs e)
        {
            List<Grade> grades = new List<Grade>(e.Grades.Count);
            for (int i = 0; i < e.Grades.Count; i++)
            {
                if (e.Grades[i].studentId == id)
                {
                    grades.Add(e.Grades[i]);
                }
            }
            gradeAdapter = new GradeAdapter(this, grades);
            gradelistView.Adapter = gradeAdapter;
        }
    }
}