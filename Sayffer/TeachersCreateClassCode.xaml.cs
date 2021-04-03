using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Sayffer.Helper;
using Sayffer.Model;

namespace Sayffer
{
    public partial class TeachersCreateClassCode : ContentPage
    {
        public TeachersCreateClassCode()
        {
            InitializeComponent();
            GetInfoForCode();


        }
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        async private void GetInfoForCode()
        {

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            try
            {
                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                string UsersEmailToDisplay = savedfirebaseauth.User.Email;

                FirebaseHelper firebaseHelper = new FirebaseHelper();
                string usableEmail = UsersEmailToDisplay.Replace(".", ",");
                Person person = await FirebaseHelper.GetPerson();



                string personName = await FirebaseHelper.GetPersonName((person.City + person.School).ToLower().Replace(" ", ""));
                string classcodename = personName.Replace(" ", "");
                Random random = new Random();
                var classcodelist = await FirebaseHelper.GetAllClassCodes((person.City + person.School).ToLower().Replace(" ", ""));

                string number = random.Next(999).ToString();
                if (classcodename.Length <= 4)
                {
                    string classcode = classcodename + number + random.Next(99).ToString();
                    string personSchool = person.School;
                    string personCity = person.City;

                    await FirebaseHelper.AddClass(classcode.ToLower(), await FirebaseHelper.GetPersonName((person.City + person.School).Replace(" ", "").ToLower()), person.School, person.City, await FirebaseHelper.GetEmail(), person.Role);
                    //RandomCode.Text = classcode.ToLower();
                    /*if (Device.RuntimePlatform == "Android")
                    {
                        MessagingCenter.Send(this, "Joined class");
                    }*/
                    if (classcodelist.Contains(classcode))
                    {
                        GetInfoForCode();
                    }
                    else
                    {
                        RandomCode.Text = classcode.ToLower();

                    }
                }
                else
                {
                    string classcode = classcodename + number;
                    string personSchool = person.School;
                    string personCity = person.City;
                    await FirebaseHelper.AddClass(classcode.ToLower(), await FirebaseHelper.GetPersonName((person.City + person.School).Replace(" ", "").ToLower()), person.School, person.City, await FirebaseHelper.GetEmail(), person.Role);
                    //RandomCode.Text = classcode.ToLower();
                    /*if (Device.RuntimePlatform == "Android")
                    {
                        MessagingCenter.Send(this, "Joined class");
                    }*/
                    if (classcodelist.Contains(classcode))
                    {
                        GetInfoForCode();
                    }
                    else
                    {
                        RandomCode.Text = classcode.ToLower();

                    }
                }


            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Alert", "Oh no! Token expired!", "Ok");




            }


        }

        void BackButton_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());

        }
    }
}
