using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Google.Android.Material.TextField;
using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;

namespace ArzitPlus
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        Button submitRegister;
        EditText usernameRegister, passwordRegister, emailRegister, phoneRegister, addressRegister;
        TextView usernameMsg, passwordMsg, emailMsg, phoneMsg, cityMsg;
        Spinner cities;
        string city = "";

        //take profile pic
        Dialog picDialog;
        Button submitPic;
        Button openCam;
        ImageView img;
        Bitmap bitmap;

        //user type dialog - student, teacher, parent
        Dialog typeDialog;
        string userType = "";
        Button submit;
        RadioGroup rp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register);
            submitRegister = FindViewById<Button>(Resource.Id.registerSubmit);
            usernameRegister = FindViewById<TextInputEditText>(Resource.Id.registerUsername);
            usernameRegister = FindViewById<TextInputEditText>(Resource.Id.registerUsername);
            passwordRegister = FindViewById<TextInputEditText>(Resource.Id.registerPassword);
            emailRegister = FindViewById<TextInputEditText>(Resource.Id.registerEmail);
            phoneRegister = FindViewById<TextInputEditText>(Resource.Id.registerPhone);
            addressRegister = FindViewById<TextInputEditText>(Resource.Id.registerAdress);

            //Error Msg
            usernameMsg = FindViewById<TextView>(Resource.Id.usernameMsg);
            passwordMsg = FindViewById<TextView>(Resource.Id.passwordMsg);
            emailMsg = FindViewById<TextView>(Resource.Id.emailMsg);
            phoneMsg = FindViewById<TextView>(Resource.Id.phoneMsg);
            cityMsg = FindViewById<TextView>(Resource.Id.cityMsg);

            //spinner
            cities = FindViewById<Spinner>(Resource.Id.registerCitySpinner);
            cities.ItemSelected += Cities_ItemSelected;
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Cities, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            cities.Adapter = adapter;
            submitRegister.Click += Submit_Click;
        }

        private void Cities_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            city = cities.GetItemAtPosition(e.Position).ToString();
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            //username
            if (usernameRegister.Length() == 0)
            {
                usernameMsg.Text = "הכנס שם פרטי";
                isValid = false;
            }
            else if (usernameRegister.Length() < 2)
            {
                usernameMsg.Text = "שם משתמש קצר מ2 תווים";
                isValid = false;
            }
            else
            {
                usernameMsg.Text = "";
            }
            //password
            if (passwordRegister.Length() == 0)
            {
                passwordMsg.Text = "הכנס סיסמא";
                isValid = false;
            }
            else if (passwordRegister.Length() < 6)
            {
                passwordMsg.Text = "סיסמא קצרה מידי";
                isValid = false;
            }
            else
            {
                passwordMsg.Text = "";
            }
            //Phone
            if (phoneRegister.Length() == 0)
            {
                phoneMsg.Text = "הכנס מספר טלפון";
                isValid = false;
            }
            else if (phoneRegister.Length() < 10)
            {
                phoneMsg.Text = "מספר טלפון קצר מידי";
                isValid = false;
            }
            else if (phoneRegister.Length() < 6)
            {
                phoneMsg.Text = "מספר טלפון צריך להיות מעל 6 ספרות";
                isValid = false;
            }
            else if (!phoneRegister.Text.All(char.IsDigit))
            {
                phoneMsg.Text = "מספר טלפון חייב להכיל ספרות בלבד";
                isValid = false;
            }
            else
            {
                phoneMsg.Text = "";
            }
            //email
            var errorMessage = "";
            var email = emailRegister.Text;
            if (email != "")
            {
                var atSignIndex = email.IndexOf('@'); // @
                var dotSign = email.IndexOf('.', atSignIndex); // נקודה
                                                               //email 6 chars - working
                if (email.Length < 6)
                {
                    errorMessage = "אימייל צריך להיות לפחות 6 תווים";
                }
                // " " - working
                else if (email.IndexOf(" ") != -1)
                {
                    errorMessage = "אימייל לא יכול להכיל רווחים";
                }
                //@ - working
                else if (atSignIndex == -1)
                {
                    errorMessage = "אימייל צריך להכיל @ אחד";
                }
                //@ after and befor chars - working
                else if (atSignIndex < 2 || email.LastIndexOf('@') == email.Length - 1)
                {
                    errorMessage = "מיקום ה - @ לא יהיה לפני תו שלישי ולא תו אחרון";
                }
                //@@ - working
                else if (atSignIndex != email.LastIndexOf('@'))
                {
                    errorMessage = "אימייל צריך רק להכיל @ אחד";
                }
                // . - working
                else if (email.IndexOf('.') == -1)
                {
                    errorMessage = "אימייל חייב להכיל נקודה";
                }
                // . after chars - working
                else if (email.IndexOf('.') == 0 || email.LastIndexOf('.') == email.Length - 1)
                {
                    errorMessage = "נקודה לא יכולה להיות התו הראשון או האחרון באימייל";
                }
                // . after @ - working
                else if (dotSign - atSignIndex < 2)
                {
                    errorMessage = "אימייל יכיל נקודה רק במרחק של שני תווים אחרי @";
                }
                //bad string - working
                else if (IsValidString(email) == false)
                {
                    errorMessage = "נמצאו תווים אסורים";
                }
            }
            else
            {
                errorMessage = "הכנס אימייל";
            }
            if (errorMessage != "")
            {
                emailMsg.Text = errorMessage;
                isValid = false;
            }
            else
            {
                emailMsg.Text = "";
            }
            //city
            if (city == "Choose your city")
            {
                cityMsg.Text = "לא נבחרה עיר";
                isValid = false;
            }
            else
            {
                cityMsg.Text = "";
            }


            if (isValid)
            {
                //choosing profile image
                picDialog = new Dialog(this);
                picDialog.SetContentView(Resource.Layout.profile_pic_dialog);
                picDialog.SetCancelable(true);

                openCam = picDialog.FindViewById<Button>(Resource.Id.openCam);
                openCam.Click += OpenCam_Click;

                img = picDialog.FindViewById<ImageView>(Resource.Id.imgDisplay);

                submitPic = picDialog.FindViewById<Button>(Resource.Id.submitPic);
                submitPic.Click += SubmitProfileImage_Click;

                picDialog.Show();
            }
            else
            {
                Toast.MakeText(this, "Wrong details, Try again!", ToastLength.Short).Show();
            }
        }
        
        private void OpenCam_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
            submitPic.Visibility = ViewStates.Visible;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            bitmap = (Bitmap)data.Extras.Get("data");
            img.SetImageBitmap(bitmap);
        }

        private void SubmitProfileImage_Click(object sender, EventArgs e)
        {
            picDialog.Dismiss();
                    
            typeDialog = new Dialog(this);
            typeDialog.SetContentView(Resource.Layout.user_type_dialog);
            typeDialog.SetCancelable(true);

            rp = typeDialog.FindViewById<RadioGroup>(Resource.Id.user_radio_group);
            rp.CheckedChange += Rp_CheckedChange;

            submit = typeDialog.FindViewById<Button>(Resource.Id.submit_user_type);
            submit.Click += SubmitRegister_Click;

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

       
        private async void SubmitRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (userType == "תלמיד")
                { 
                    Student student = new Student(usernameRegister.Text, emailRegister.Text, passwordRegister.Text, phoneRegister.Text, addressRegister.Text, city, bitmap);
                    if (await student.Register() == true)
                    {
                        Toast.MakeText(this, "Registration Succeeded", ToastLength.Short).Show();
                        Thread.Sleep(1000);
                        Finish();
                        Intent intent = new Intent(this, typeof(StudentLoginActivity));
                        StartActivity(intent);
                    }
                }
                else if (userType == "מורה")
                { 
                    Teacher teacher = new Teacher(usernameRegister.Text, emailRegister.Text, passwordRegister.Text, phoneRegister.Text, addressRegister.Text, city, bitmap);
                    if (await teacher.Register() == true)
                    {
                        Toast.MakeText(this, "Registration Succeeded", ToastLength.Short).Show();
                        Thread.Sleep(1000);
                        Finish();
                        Intent intent = new Intent(this, typeof(TeacherLoginActivity));
                        StartActivity(intent);
                    }
                }
                else
                {
                    Toast.MakeText(this, "choose your type", ToastLength.Short).Show();
                }
                
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Registration failed", ToastLength.Short).Show();
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
        }

        public static bool IsValidString(string str)
        {
            //אם מכילה גרשיים - לא תקין
            var quot = "\"";
            if (str.IndexOf(quot) != -1)
                return false;
            //אם מכילה את אחד התווים הבאים - לא תקין
            string badStr = "$%^&*()_+-/[]{}<>?אבגדהוזחטיכךלמםנןסעפצקרשת;";
            int p;
            char ch;
            for (int i = 0; i < badStr.Length; i++)
            {
                ch = badStr[i];
                p = str.IndexOf(ch);
                if (p != -1)
                    return false;
            }
            return true; //אם הכל תקין
        }
    }
}