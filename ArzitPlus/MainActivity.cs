using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;

namespace ArzitPlus
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button login, register;
        string userType = "";
        Dialog typeDialog;
        internal static List<Lesson> all;
        ChargingStateReceiver receiver;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            receiver = new ChargingStateReceiver();
            RegisterReceiver(receiver, new IntentFilter(Intent.ActionBatteryChanged));

            login = FindViewById<Button>(Resource.Id.login);
            register = FindViewById<Button>(Resource.Id.register);

            login.Click += Login_Click;
            register.Click += Register_Click;

            LessonEventListener listener = new LessonEventListener();
            listener.OnOrderRetrieved += Listener_OnOrderRetrieved;
        }
        //charge alert
        protected override void OnDestroy()
        {
            UnregisterReceiver(receiver);
            base.OnDestroy();
        }
        private async void Listener_OnOrderRetrieved(object sender, LessonEventListener.LessonEventArgs e)
        {
            all = e.Lessons;
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].date < DateTime.Today)
                {
                    await all[i].LessonDone();
                }
            }
        }

        private void Register_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }

        private void Login_Click(object sender, System.EventArgs e)
        {
            typeDialog = new Dialog(this);
            typeDialog.SetContentView(Resource.Layout.user_type_dialog);
            Button submit = typeDialog.FindViewById<Button>(Resource.Id.submit_user_type);
            submit.Click += Submit_Click;

            RadioGroup rp = typeDialog.FindViewById<RadioGroup>(Resource.Id.user_radio_group);
            rp.CheckedChange += Rp_CheckedChange;

            typeDialog.Show();
        }

        private void Rp_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            if (e.CheckedId == Resource.Id.studentOption)
            {
                userType = "תלמיד";
            }
            else if (e.CheckedId == Resource.Id.teacherOption)
            {
                userType = "מורה";
            }
        }

        private void Submit_Click(object sender, System.EventArgs e)
        {
            if (userType == "תלמיד")
            {
                typeDialog.Dismiss();
                Finish();
                Intent intent = new Intent(this, typeof(StudentLoginActivity));
                StartActivity(intent);
            }
            else if (userType == "מורה")
            {
                typeDialog.Dismiss();
                Finish();
                Intent intent = new Intent(this, typeof(TeacherLoginActivity));
                StartActivity(intent);
            }
            else
            {
                Toast.MakeText(this, "choose your type", ToastLength.Short).Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}