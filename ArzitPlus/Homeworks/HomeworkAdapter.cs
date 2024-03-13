using Android.App;
using Android.Content;
using Android.Hardware;
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
    internal class HomeworkAdapter : BaseAdapter<Homework>, IShakeListener
    {
        Context context;
        List<Homework> objects;
        Button details, finish;
        Dialog dialog;
        ShakeDetector shakeDetector;
        SensorManager sensorManager;
        private bool isWaitingForShake = false;
        Homework remove;

        public HomeworkAdapter(Context context, List<Homework> objects, SensorManager sensorManager)
        {
            this.context = context;
            this.objects = objects;
            this.sensorManager = sensorManager;
            shakeDetector = new ShakeDetector(context, this);
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override Homework this[int index]
        {
            get { return this.objects[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((StudentHomeworkActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.homework_row_layout, parent, false);
            TextView teacher = view.FindViewById<TextView>(Resource.Id.homeworkTeacherRow);
            TextView subject = view.FindViewById<TextView>(Resource.Id.homeworkSubjectRow);
            
            details = view.FindViewById<Button>(Resource.Id.homeworkContentRow);
            finish = view.FindViewById<Button>(Resource.Id.homeworkFinish);
            Homework hm = objects[position];

            if (hm != null)
            {
                teacher.Text += hm.teacher;
                subject.Text += hm.subject;
                details.Tag = position;
                details.Click += Details_Click;
                finish.Tag = position;
                finish.Click += (sender, e) =>
                {
                    if (!isWaitingForShake)
                    {
                        // Start waiting for shake
                        Button btnOrigin = (Button)sender;
                        int pos = (int)btnOrigin.Tag;
                        remove = objects[pos];
                        isWaitingForShake = true;
                        Toast.MakeText(context, "shake to remove from list", ToastLength.Short).Show();
                        sensorManager.RegisterListener(shakeDetector, sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Ui);
                    }
                };
            }
            return view;
        }
        
        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }

        public async void OnShakeDetected()
        {
            if (isWaitingForShake)
            {
                // Shake detected
                isWaitingForShake = false;
                try
                {
                    if (await remove.HomeworkDone() == true)
                    {
                        Toast.MakeText(context, "Homework has been marked as done", ToastLength.Short).Show();
                        sensorManager.UnregisterListener(shakeDetector);
                        NotifyDataSetChanged();
                        if (Count == 0)
                        {
                            NotifyAll();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(context, "Error", ToastLength.Short).Show();
                }
            }
        }

        private void Details_Click(object sender, EventArgs e)
        {
            Button btnOrigin = (Button)sender;
            int pos = (int)btnOrigin.Tag;
            Homework hm = objects[pos];
            dialog = new Dialog(context);
            dialog.SetContentView(Resource.Layout.homework_details);
            dialog.SetCancelable(true);

            TextView details = dialog.FindViewById<TextView>(Resource.Id.homeworkDetails);
            details.Text = hm.content;

            Button back = dialog.FindViewById<Button>(Resource.Id.backFromDetails);
            back.Click += Back_Click;

            dialog.Show();
        }

        private void Back_Click(object sender, EventArgs e)
        {
            dialog.Dismiss();
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return objects.Count;
            }
        }

        public ShakeDetector ShakeDetector
        {
            get => default;
            set
            {
            }
        }
    }

    internal class HomeworkAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}