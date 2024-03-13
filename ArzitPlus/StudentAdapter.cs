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
    internal class StudentAdapter : BaseAdapter<Student>
    {
        Context context;
        List<Student> objects;
        Button details;
        Student choosen;

        public StudentAdapter(Context context, List<Student> objects)
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
        public override Student this[int index]
        {
            get { return this.objects[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((TeacherGradesActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.student_row_layout, parent, false);
            TextView studentName = view.FindViewById<TextView>(Resource.Id.studentNameRow);
            
            details = view.FindViewById<Button>(Resource.Id.studentGradesRow);
            Student student = objects[position];

            if (student != null)
            {
                studentName.Text += student.username;
                details.Tag = position;
                details.Click += Details_Click;
            }
            return view;
        }

        private void Details_Click(object sender, EventArgs e)
        {
            Button btnOrigin = (Button)sender;
            int pos = (int)btnOrigin.Tag;
            choosen = objects[pos];
            Intent intent = new Intent(context, typeof(StudentGradeActivity));
            intent.PutExtra("id", choosen.password);
            context.StartActivity(intent);
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return objects.Count;
            }
        }

        internal GradeAdapter GradeAdapter
        {
            get => default;
            set
            {
            }
        }
    }

    internal class StudentAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}