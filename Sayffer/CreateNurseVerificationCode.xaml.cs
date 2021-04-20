using System;
using System.Collections.Generic;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Sayffer.Helper;
using Sayffer.Model;

namespace Sayffer
{
    public partial class CreateNurseVerificationCode : ContentPage
    {
        public CreateNurseVerificationCode()
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
                string person = await FirebaseHelper.GetPersonSchool();
                Person personrole = await FirebaseHelper.GetPerson();
                if(personrole.Role != "Admin")
                {                    App.Current.MainPage = new NavigationPage(new DashboardPage());

                    await App.Current.MainPage.DisplayAlert("Oops!", "Error", "OK");
                }

                else
                {
                    Random random = new Random();
                    string number = random.Next(10000).ToString();
                    string city = await FirebaseHelper.GetPersonCity();
                    string verificationcode = city + person + number;
                    RandomCode.Text = verificationcode.ToLower();
                    await FirebaseHelper.AddNurseVerificationCode(person, city, verificationcode);
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

