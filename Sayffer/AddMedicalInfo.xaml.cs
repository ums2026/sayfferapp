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
    public partial class AddMedicalInfo : ContentPage
    {
        public AddMedicalInfo()
        {
            InitializeComponent();
            var commonmedicalallergies = new List<string>()
                    {
                        "Latex",
                        "Other"

                    };
            commonmedicalallergiespicker.ItemsSource = commonmedicalallergies;
            GetInfo();

        }
        async void GetInfo()
        {
            var person = await FirebaseHelper.GetPerson();
            if (person.Role == "Parent")
            {
                studentname.SetValue(IsVisibleProperty, true);
                studentname.ItemsSource = await FirebaseHelper.GetPersonStudentName();
            }
        }
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        async void Enter_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            try
            {
                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                string UsersEmailToDisplay = savedfirebaseauth.User.Email;
                string selectedallergy = commonmedicalallergiespicker.SelectedItem.ToString();
                if (selectedallergy == "Other")
                {

                    other.SetValue(IsVisibleProperty, true);
                    Enter.SetValue(IsVisibleProperty, false);
                    EnterOther.SetValue(IsVisibleProperty, true);
                }
                else
                {
                    other.SetValue(IsVisibleProperty, false);
                    EnterOther.SetValue(IsVisibleProperty, false);
                    FirebaseHelper firebaseHelper = new FirebaseHelper();
                    string usableEmail = UsersEmailToDisplay.Replace(".", ",");
                    Person person = await FirebaseHelper.GetPerson();
                    if (person.Role.ToLower() == "parent")
                    {
                        string studentName = studentname.SelectedItem.ToString();
                        string personrole = "Student";
                        foreach (var code in await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()))
                        {
                            await FirebaseHelper.AddMedicalAllergyStudent(await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")), personrole, studentName, person.School, person.City, code, selectedallergy, await FirebaseHelper.GetPersonEmail());

                        }
                        await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
                        await FirebaseHelper.GetNurseAddNotifs(person.School, person.City, selectedallergy, studentName);
                    }
                    else
                    {
                        foreach (var code in await FirebaseHelper.GetPersonsClassCodes())
                        {
                            await FirebaseHelper.AddMedicalAllergyTeacher(await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")), person.Role, person.School, person.City, code, selectedallergy, await FirebaseHelper.GetPersonEmail());

                        }
                        await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
                        await FirebaseHelper.GetNurseAddNotifs(person.School, person.City, selectedallergy, await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")));

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

        async void EnterOther_Clicked(System.Object sender, System.EventArgs e)
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
                if (person.Role.ToLower() == "parent")
                {
                    Person personname = await FirebaseHelper.GetStudentName(person.City, person.ClassCode, person.School, person.Name);
                    string studentName = studentname.SelectedItem.ToString();
                    string personrole = "Student";
                    foreach (var code in await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()))
                    {
                        await FirebaseHelper.AddMedicalAllergyStudent(await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")), personrole, studentName, person.School, person.City, code, other.Text, await FirebaseHelper.GetPersonEmail());

                    }
                    await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
                    await FirebaseHelper.GetNurseAddNotifs(person.School, person.City, other.Text, studentName);

                }
                else
                {
                    foreach (var code in await FirebaseHelper.GetPersonsClassCodes())
                    {
                        await FirebaseHelper.AddMedicalAllergyTeacher(await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")), person.Role, person.School, person.City, code, other.Text, await FirebaseHelper.GetPersonEmail());

                    }
                    await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
                    await FirebaseHelper.GetNurseAddNotifs(person.School, person.City, other.Text, person.Name);

                }

            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Alert", "Oh no! Token expired!", "Ok");
            }
        }
    }
}
