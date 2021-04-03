using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase;
using Newtonsoft.Json;
using Firebase.Database;
using Firebase.Database.Query;
using Sayffer.Helper;
using Sayffer.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Auth;

namespace Sayffer
{
    public partial class MainPage : ContentPage
    {
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";
        Account account;
        AccountStore store;
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        public MainPage()
        {
            InitializeComponent();
            store = AccountStore.Create();

        }
        async void SignUp_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new EnterInfoPage());

        }



        async void Login_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            try
            {
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(UserEmail.Text, UserPassword.Text);
                var content = await auth.GetFreshAuthAsync();
                var serializedcontent = JsonConvert.SerializeObject(content);
                Preferences.Set("MyFirebaseRefreshToken", serializedcontent);
                await Navigation.PushAsync(new DashboardPage());


            }

            catch (Exception x)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Invalid username or password", "Ok");
            }

        }

    }
}
