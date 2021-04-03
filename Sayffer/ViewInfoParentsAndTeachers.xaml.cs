using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Sayffer.Model;
using Sayffer.Helper;

namespace Sayffer
{
    public partial class ViewInfoParentsAndTeachers : ContentPage
    {
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        public ViewInfoParentsAndTeachers()
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

                FirebaseHelper firebaseHelper = new FirebaseHelper();
                string usableEmail = UsersEmailToDisplay.Replace(".", ",");
                Person person = await FirebaseHelper.GetPerson();

                var COVIDcaseslistschool = await FirebaseHelper.GetCovidNumbersSchool(person.City, person.School);
                if (COVIDcaseslistschool == null)
                {
                    COVIDCasesInSchool.Text = "No COVID-19 Cases have been entered for this school";
                }
                else
                {
                    string COVIDcasesinschool = COVIDcaseslistschool.NumberOfCases;
                    if (COVIDcasesinschool == "1")
                    {
                        COVIDCasesInSchool.Text = "There is " + COVIDcasesinschool + " current case of COVID-19 reported in your school.";

                    }
                    else
                    {
                        COVIDCasesInSchool.Text = "There are " + COVIDcasesinschool + " current cases of COVID-19 reported in your school.";

                    }
                }

                var getinfo = await FirebaseHelper.GetCovidNumbersSchool(person.City, person.School);
                if (getinfo != null)
                {


                    MostRecentCOVIDDate.Text = "Most recent COVID-19 case in your school was reported on " + getinfo.Time;


                }
                else
                {
                    MostRecentCOVIDDate.SetValue(IsVisibleProperty, false);
                }


                var Flucasesinschool = await FirebaseHelper.GetFluNumbersSchool(person.City, person.School);
                if (Flucasesinschool == null)
                {
                    FluCasesInSchool.Text = "No Viral Flu Cases have been entered for this school";
                }
                else
                {
                    if (Flucasesinschool.NumberOfCases == "1")
                    {
                        FluCasesInSchool.Text = "There is " + Flucasesinschool.NumberOfCases + " current case of Viral Flu reported in your school.";

                    }
                    else
                    {
                        FluCasesInSchool.Text = "There are " + Flucasesinschool.NumberOfCases + " current cases of the flu reported in your school.";

                    }
                }

                var getinfoflu = await FirebaseHelper.GetFluNumbersSchool(person.City, person.School);
                if (getinfoflu != null)
                {

                    MostRecentFluDate.Text = "Most recent case of the flu in your school was reported on " + getinfoflu.Name;

                }
                else
                {
                    MostRecentFluDate.SetValue(IsVisibleProperty, false);
                }



            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Oops!", "Token Expired", "OK");
            }
        }

        void BackToDashboard_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());

        }
    }
}

