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
    internal class GradeAdapter : BaseAdapter<Grade>
    {
        Context context;
        List<Grade> objects;

        public GradeAdapter(Context context, List<Grade> objects)
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
        public override Grade this[int index]
        {
            get { return this.objects[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((StudentGradeActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.grade_row_layout, parent, false);
            TextView subject = view.FindViewById<TextView>(Resource.Id.gradeSubjectRow);
            TextView gradeNum = view.FindViewById<TextView>(Resource.Id.gradeNumRow);
            TextView type = view.FindViewById<TextView>(Resource.Id.gradeTypeRow);
            
           
            Grade grade = objects[position];

            if (grade != null)
            {
                subject.Text += grade.subject;
                gradeNum.Text += grade.grade;
                type.Text += grade.type;
            }
            return view;
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

    internal class GradeAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}