using System;
using System.Collections.Generic;
using Firebase.Auth;
using Newtonsoft.Json;
using Sayffer.Helper;
using Sayffer.Model;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace Sayffer
{
    public partial class Notifications : ContentPage
    {
        public static string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        public Notifications()
        {
            InitializeComponent();
            GetProfilInformationAndRefreshToken();


        }


        async void GetProfilInformationAndRefreshToken()
        {

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            try
            {
                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                string UsersEmailToDisplay = savedfirebaseauth.User.Email;
                Person person = await FirebaseHelper.GetPerson();
                //if(person.Role != "Nurse")
                //{
                var allnotifs = await FirebaseHelper.GetAllNotifications(person.City, person.School, UsersEmailToDisplay);
                NotificationsDisplay.ItemsSource = allnotifs;
                //}

            }

            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Oops!", "Token Expired", "OK");
            }
        }

        async void BacktoDashboard_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));

                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                string UsersEmailToDisplay = savedfirebaseauth.User.Email;
                Person person = await FirebaseHelper.GetPerson();
                App.Current.MainPage = new NavigationPage(new DashboardPage());
                await FirebaseHelper.MarkAsRead(person.City, person.School, UsersEmailToDisplay);

            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Oops!", "Token Expired", "OK");
            }

        }
    }
}
