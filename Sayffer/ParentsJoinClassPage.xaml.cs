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
    public partial class ParentsJoinClassPage : ContentPage
    {
        public ParentsJoinClassPage()
        {
            InitializeComponent();
            //GetProfilInformationAndRefreshToken();
        }

        /*async void GetProfilInformationAndRefreshToken() {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
        var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
        Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                string UsersEmailToDisplay = savedfirebaseauth.User.Email;

        string usableEmail = UsersEmailToDisplay.Replace(".", ",");
        Person person = await FirebaseHelper.GetPerson(usableEmail);
            
            ClassCode.ItemsSource = await FirebaseHelper.GetAllClassCodes(person.City, person.School);

        }*/


        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        async void BackButton_Clicked(System.Object sender, System.EventArgs e)

        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());

        }

        async void Join_Clicked(System.Object sender, System.EventArgs e)
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
                await FirebaseHelper.AddToClass(ClassCode.Text, person.School, Name.Text, person.City, savedfirebaseauth.User.Email, person.Role);
                await App.Current.MainPage.DisplayAlert("Success", "Added!", "Ok");
                Name.Text = string.Empty;
                /*if (Device.RuntimePlatform == "Android")
                {
                    MessagingCenter.Send(this, "Joined class");
                }*/


            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Alert", "Oh no! Token expired!", "Ok");
            }
        }
    }
}
