using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Sayffer.Helper;
using Sayffer.Model;

namespace Sayffer
{
    public partial class ViewInfoPage : ContentPage
    {
        public ViewInfoPage()
        {
            InitializeComponent();
            GetProfilInformationAndRefreshToken();
            var options = new List<string>()
                    {
                        "Food Allergies",
                        "Medical-related allergies",
                        "Virus Cases"

                    };
            Optionsforfiltering.ItemsSource = options;

        }
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        async void GetProfilInformationAndRefreshToken()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            try
            {
                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));

                FirebaseHelper firebaseHelper = new FirebaseHelper();
                string usableEmail = savedfirebaseauth.User.Email.Replace(".", ",");
                Person person = await FirebaseHelper.GetPerson();

                string cityschool = (person.City.ToLower() + person.School.ToLower()).Replace(" ", "");


                var listofclasscodes = new List<string>();

                var allclasscodes = await FirebaseHelper.GetAllClassCodes(cityschool);
                foreach (var classcode in allclasscodes)
                {
                    string classcodeforuse = classcode.ToString();
                    if (listofclasscodes.Contains(classcodeforuse.ToLower()))
                    {
                        listofclasscodes = listofclasscodes;
                    }
                    else
                    {
                        if (classcodeforuse.ToString() == "emptyatpresent")
                        {
                            listofclasscodes = listofclasscodes;
                        }
                        else
                        {
                            listofclasscodes.Add(classcodeforuse.ToLower());
                        }

                    }



                }
                ClassCodes.ItemsSource = listofclasscodes;

            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Alert", "Oh no! Token expired!", "Ok");
            }


        }


        async void NextButton_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));

            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            if (Optionsforfiltering.SelectedItem.ToString() == null || ClassCodes.SelectedItem.ToString() == null)
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Please fill all required fields", "Ok");
            }
            else
            {
                FirebaseHelper firebaseHelper = new FirebaseHelper();
                string usableEmail = savedfirebaseauth.User.Email.Replace(".", ",");
                Person person = await FirebaseHelper.GetPerson();

                if (Optionsforfiltering.SelectedItem.ToString() == "Food Allergies")
                {
                    var classinfofoodallergies = await FirebaseHelper.GetClassAllergiesInfo(person.City, person.School, ClassCodes.SelectedItem.ToString());
                    if (classinfofoodallergies.Count != 0)
                    {
                        InfoListNursesFoodAllergies.ItemsSource = classinfofoodallergies;
                        InfoListNursesFoodAllergies.SetValue(IsVisibleProperty, true);
                        InfoListNursesMedicalAllergiesTeachers.SetValue(IsVisibleProperty, false);
                        InfoListNursesMedicalAllergiesStudents.SetValue(IsVisibleProperty, false);

                    }

                    else
                    {
                        await App.Current.MainPage.DisplayAlert("No Allergies in this Class", "No food allergies entered for this class", "Ok");
                        Alertfoodallergies.Text = "No food allergies in this class have been entered";
                    }

                }
                else
                {
                    InfoListNursesFoodAllergies.SetValue(IsVisibleProperty, false);
                    Alertfoodallergies.SetValue(IsVisibleProperty, false);


                }
                if (Optionsforfiltering.SelectedItem.ToString() == "Medical-related allergies")
                {
                    var classinfomedicalallergiesteacher = new List<Person>(await FirebaseHelper.GetMedicalAllergyInfoByClassTeachers(person.City, person.School, ClassCodes.SelectedItem.ToString()));
                    if (classinfomedicalallergiesteacher.Count != 0)
                    {
                        InfoListNursesMedicalAllergiesTeachers.ItemsSource = classinfomedicalallergiesteacher;
                        InfoListNursesMedicalAllergiesTeachers.SetValue(IsVisibleProperty, true);
                    }
                    else
                    {
                        Alertmedicalallergiesteacher.Text = "No food allergies in this class have been entered for teachers";
                    }
                    var classinfomedicalallergiesstudent = await FirebaseHelper.GetMedicalAllergyInfoByClassStudents(person.City, person.School, ClassCodes.SelectedItem.ToString());
                    if (classinfomedicalallergiesstudent.Count != 0)
                    {
                        InfoListNursesMedicalAllergiesStudents.ItemsSource = classinfomedicalallergiesstudent;
                        InfoListNursesMedicalAllergiesStudents.SetValue(IsVisibleProperty, true);
                    }
                    else
                    {
                        Alertmedicalallergiesstudent.Text = "No food allergies in this class have been entered for students";
                    }
                }
                else
                {

                    InfoListNursesMedicalAllergiesStudents.SetValue(IsVisibleProperty, false);
                    InfoListNursesMedicalAllergiesTeachers.SetValue(IsVisibleProperty, false);
                    Alertmedicalallergiesstudent.SetValue(IsVisibleProperty, false);
                    Alertmedicalallergiesteacher.SetValue(IsVisibleProperty, false);
                }

                if (Optionsforfiltering.SelectedItem.ToString() == "Virus Cases")
                {
                    var classinfoviruscase = new List<Person>(await FirebaseHelper.GetVirusCaseInfoByClass(person.City, person.School, ClassCodes.SelectedItem.ToString()));
                    if (classinfoviruscase.Count != 0)
                    {
                        InfoListNursesVirusCasesInfo.ItemsSource = classinfoviruscase;
                        InfoListNursesVirusCasesInfo.SetValue(IsVisibleProperty, true);
                    }
                    else
                    {
                        Alertviruscases.Text = "No virus cases entered for this class!";
                    }

                }
                else
                {

                    InfoListNursesVirusCasesInfo.SetValue(IsVisibleProperty, false);
                    Alertviruscases.SetValue(IsVisibleProperty, false);
                }
            }


        }
        void BackButton_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());

        }

    }
}
