using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.TextField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArzitPlus
{
    [Activity(Label = "TeacherLoginActivity")]
    public class TeacherLoginActivity : Activity
    {
        Button submitLogin;
        EditText emailLogin, passwordLogin;
        ISharedPreferences sp;
        List<Teacher> teachers;
        string id, email;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);
            submitLogin = FindViewById<Button>(Resource.Id.loginSubmit);

            emailLogin = FindViewById<TextInputEditText>(Resource.Id.loginEmail);
            passwordLogin = FindViewById<TextInputEditText>(Resource.Id.loginPassword);
            sp = this.GetSharedPreferences(Teacher.COLLECTION_USER_FILE, FileCreationMode.Private);

            string mail = sp.GetString("email", "");
            string password = sp.GetString("password", "");

            if (mail != "" && password != "")
            {
                emailLogin.Text = mail;
                passwordLogin.Text = password;
            }

            TeachersEventListener listener = new TeachersEventListener();
            listener.OnOrderRetrieved += Listener_OnOrderRetrieved;

            submitLogin.Click += SubmitLogin_Click;
        }

        private async void SubmitLogin_Click(object sender, EventArgs e)
        {
            if (emailLogin.Text == "" && passwordLogin.Text == "")
            {
                Toast.MakeText(this, "Please enter email and password", ToastLength.Short).Show();
                return;
            }
            else
            {
                try
                {
                    id = passwordLogin.Text;
                    email = emailLogin.Text;
                    if (SearchUser())
                    {
                        Teacher user = new Teacher(emailLogin.Text, passwordLogin.Text);
                        if (await user.Login() == true)
                        {
                            Toast.MakeText(this, "Login Succeeded", ToastLength.Short).Show();
                            Finish();
                            Intent intent = new Intent(this, typeof(TeacherHomepageActivity));
                            StartActivity(intent);
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "User doesn't exist", ToastLength.Short).Show();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Error login", ToastLength.Short).Show();
                }
            }
        }
        public bool SearchUser()
        {
            for (int i = 0; i < teachers.Count; i++)
            {
                if (teachers[i].password == id && teachers[i].email == email)
                {
                    return true;
                }
            }
            return false;

        }
        private void Listener_OnOrderRetrieved(object sender, TeachersEventListener.TeacherEventArgs e)
        {
            teachers = e.Teachers;
        }
    }
}