using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Firebase.Database;
using Firebase.Database.Query;
using Sayffer;
using Sayffer.Helper;
using Sayffer.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Auth;
using Firebase.Auth;
using System.Data;
using System.Net.Http;
using System.Net;


namespace Sayffer
{
    public partial class EnterInfoPage : ContentPage
    {
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        public EnterInfoPage()
        {
            InitializeComponent();
            var roles = new List<string>()
                    {
                        "Parent",
                        "Teacher",
                        "Nurse",
                        "Admin"

                    };
            role.ItemsSource = roles;
            Submit2.SetValue(IsVisibleProperty, false);

        }

        async void Enter_Clicked(System.Object sender, System.EventArgs e)
        {
            if (checkbox.IsChecked)
            {
                try
                {

                    if (NewUserPassword.Text != confirmPassword.Text)
                    {
                        await DisplayAlert("Error", "Passwords should match", "Ok");
                    }
                    if (role.SelectedItem == null || UserName.Text == null || NewUserEmail.Text == null || School.Text == null || City.Text == null)
                    {
                        await App.Current.MainPage.DisplayAlert("Oops!", "Please fill all fields", "Ok");
                    }
                    else
                    {

                        if (role.SelectedItem.ToString() == "Nurse")

                        {
                            Submit2.SetValue(IsVisibleProperty, true);
                            NurseVerificationCode.SetValue(IsVisibleProperty, true);
                            Enter.SetValue(IsVisibleProperty, false);
                        }
                        else
                        {

                            await FirebaseHelper.AddPerson(role.SelectedItem.ToString(), UserName.Text, School.Text, City.Text, NewUserEmail.Text, NewUserPassword.Text);
                            UserName.Text = string.Empty;
                            School.Text = string.Empty;
                            City.Text = string.Empty;
                            NewUserEmail.Text = string.Empty;
                            await Navigation.PushAsync(new DashboardPage());

                        }

                    }

                    /*if (Device.RuntimePlatform == "Android")
                    {

                        MessagingCenter.Send(this, "Signed up, can now subscribe to school");
                        if (role.SelectedItem.ToString() == "Teacher")
                        {
                            MessagingCenter.Send(this, "Signed up, subscribe to teacher topic");

                        }
                        if (role.SelectedItem.ToString() == "Admin")
                        {
                            MessagingCenter.Send(this, "Signed up, subscribe to admin topic");

                        }
                        if (role.SelectedItem.ToString() == "Parent")
                        {
                            MessagingCenter.Send(this, "Signed up, subscribe to parent topic");

                        }
                    }*/



                }

                catch (Exception x)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", x.Message, "Ok");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "We require that you agree to the statement above and check the box for an account to be created", "Ok");
            }

        }
        async void Submit2_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
                var auth = await authProvider.SignInAnonymouslyAsync();
                string gettoken = auth.FirebaseToken;
                var content = await auth.GetFreshAuthAsync();
                var serializedcontent = JsonConvert.SerializeObject(content);
                Preferences.Set("MyFirebaseRefreshToken", serializedcontent);

                var code = await FirebaseHelper.GetNurseVerificationCode(School.Text, City.Text);
                if (NurseVerificationCode.Text.ToLower() == code.ToLower())
                {
                    await App.Current.MainPage.DisplayAlert("Sign Up Successful", " ", "Ok");


                    await FirebaseHelper.AddPerson(role.SelectedItem.ToString(), UserName.Text, School.Text, City.Text, NewUserEmail.Text, NewUserPassword.Text);
                    UserName.Text = string.Empty;
                    School.Text = string.Empty;
                    City.Text = string.Empty;
                    NewUserEmail.Text = string.Empty;
                    NewUserPassword.Text = string.Empty;
                    await Navigation.PushAsync(new DashboardPage());
                }
                else
                {
                    if (code == "Code not generated, please contact your administrator to generate it.")
                    {
                        await DisplayAlert("Oops!", "Code not generated, please contact your administrator to generate it.", "Ok");
                        Preferences.Remove("MyFirebaseRefreshToken");
                        App.Current.MainPage = new NavigationPage(new MainPage());
                    }
                    else
                    {
                        if (code == "School not in our system")
                        {
                            await DisplayAlert("Oops!", "School not in our system", "Ok");
                            Preferences.Remove("MyFirebaseRefreshToken");
                            App.Current.MainPage = new NavigationPage(new MainPage());

                        }
                        else
                        {
                            await DisplayAlert("Oops!", "Invalid verification code", "Ok");
                            Preferences.Remove("MyFirebaseRefreshToken");
                            App.Current.MainPage = new NavigationPage(new MainPage());
                        }
                    }

                }
            }
            catch (Exception x)
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Some error while signing up.  Please make sure you have not already created an account", "Ok");
                Console.WriteLine(x.Message);
            }

        }



    }
}
