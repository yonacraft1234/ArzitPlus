using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Uri = Android.Net.Uri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Java.Util;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Android.Graphics;
using Firebase.Storage;
using Android.Gms.Tasks;
using System.IO;

namespace ArzitPlus
{
    internal class Teacher : Java.Lang.Object, IOnSuccessListener
    {
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public Bitmap profilePic { get; set; }


        FirebaseAuth firebaseAuthentication;
        FirebaseFirestore database;
        StorageReference images;
        public string imageUrl;
        public const string COLLECTION_NAME = "teachers";
        public const string COLLECTION_USER_FILE = "currentTeacherFile";

        public Teacher()
        {
            this.firebaseAuthentication = AppDataHelper.GetFirebaseAuthentication();
            this.database = AppDataHelper.GetFirestore();
        }
        public Teacher(string email, string password)
        {
            this.password = password;
            this.email = email;
            this.firebaseAuthentication = AppDataHelper.GetFirebaseAuthentication();
            this.database = AppDataHelper.GetFirestore();
        }

        public Teacher(string username, string email, string password, string phone, string address, string city, Bitmap profilePic)
        {
            this.username = username;
            this.email = email;
            this.password = password;
            this.phone = phone;
            this.address = address;
            this.city = city;
            this.profilePic = profilePic;
            this.firebaseAuthentication = AppDataHelper.GetFirebaseAuthentication();
            this.database = AppDataHelper.GetFirestore();
        }
        public async Task<bool> Login()
        {
            try
            {
                await this.firebaseAuthentication.SignInWithEmailAndPassword(this.email, this.password);
                var editor = Application.Context.GetSharedPreferences(COLLECTION_USER_FILE, FileCreationMode.Private).Edit();
                editor.PutString("email", this.email);
                editor.PutString("password", this.password);
                editor.Apply();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }
            return true;
        }

        public async Task<bool> Register()
        {
            try
            {
                await this.firebaseAuthentication.CreateUserWithEmailAndPassword(this.email, this.password);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }
            try
            {
                HashMap userhMap = new HashMap();
                userhMap.Put("email", this.email);
                userhMap.Put("password", this.password);
                userhMap.Put("name", this.username);
                userhMap.Put("phone", this.phone);
                userhMap.Put("address", this.address);
                userhMap.Put("city", this.city);

                //Convert the bitmap image to a byte array
                Bitmap profileImage = this.profilePic;
                MemoryStream stream = new MemoryStream();
                profileImage.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                byte[] imageBytes = stream.ToArray();
                StorageReference storageRef = FirebaseStorage.Instance.GetReferenceFromUrl("gs://arzitplus-13988.appspot.com");
                images = storageRef.Child("/images/teachers/" + this.password);
                var task = await images.PutBytes(imageBytes);
                await images.DownloadUrl.AddOnSuccessListener(this);
                userhMap.Put("imgUrl", imageUrl);
                stream.Close();
                
                DocumentReference userReference = this.database.Collection(COLLECTION_NAME).Document(this.firebaseAuthentication.CurrentUser.Uid);
                await userReference.Set(userhMap);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }
            return true;
        }
        public async Task<bool> Logout()
        {
            try
            {
                var editor = Application.Context.GetSharedPreferences(Teacher.COLLECTION_USER_FILE, FileCreationMode.Private).Edit();
                editor.PutString("email", "");
                editor.PutString("password", "");
                editor.Apply();
                firebaseAuthentication.SignOut();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public void OnSuccess(Java.Lang.Object result)
        {
            var downloadUrl = result.JavaCast<Uri>();
            imageUrl = downloadUrl.ToString();
        }
    }
}