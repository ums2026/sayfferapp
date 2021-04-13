using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Sayffer.Helper;
using Sayffer.Model;

namespace Sayffer
{
    public partial class AddInfoAboutVirusCases : ContentPage
    {
        public AddInfoAboutVirusCases()
        {
            InitializeComponent();
            var viruscaseslist = new List<string>()
                    {
                        "COVID19",
                        "Viral Flu",
                        "Stomach Flu",
                        "Head Lice",
                        "Other"

                    };
            viruscasespicker.ItemsSource = viruscaseslist;
            GetInfo();
        }

        async void GetInfo()
        {
            Person person = await FirebaseHelper.GetPerson();
            if (person.Role == "Teacher")
            {
                studentname.SetValue(IsVisibleProperty, false);
            }
            if (person.Role == "Parent")
            {
                studentname.ItemsSource = await FirebaseHelper.GetPersonStudentName();


            }
        }
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";


        async void Enter_Clicked(System.Object sender, System.EventArgs e)
        {
            if (viruscasespicker.SelectedItem.ToString() == null)
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Please fill in all fields", "Ok");
            }
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            try
            {
                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                string UsersEmailToDisplay = savedfirebaseauth.User.Email;
                string selectedinfo = viruscasespicker.SelectedItem.ToString();
                if (selectedinfo == "Other")
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
                    Person person = await FirebaseHelper.GetPerson();
                    if (person.ClassCode == "EmptyAtPresent")
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Oops! Please join a class first", "Ok");

                    }
                    else
                    {
                        if (viruscasespicker.SelectedItem.ToString() == "COVID19")
                        {

                            //Used to have the Device.RuntimePlatform, send COVID Info

                        }

                    }
                    string city = await FirebaseHelper.GetPersonCity();
                    string school = await FirebaseHelper.GetPersonSchool();
                    string cityschool = (city + school).Replace(" ", "").ToLower();
                    if (person.Role.ToLower() == "parent")
                    {

                        foreach (var classcode in await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()))
                        {

                            await FirebaseHelper.AddVirusCaseInfo("Student", studentname.SelectedItem.ToString(), person.School, person.City, savedfirebaseauth.User.Email, viruscasespicker.SelectedItem.ToString(), classcode.ToLower());

                        }
                        if (viruscasespicker.SelectedItem.ToString() == "COVID19")
                        {
                            await FirebaseHelper.GetPeopleInSchoolAndAddNotifsCOVID(person.School, person.City, viruscasespicker.SelectedItem.ToString(), studentname.SelectedItem.ToString());
                        }
                        await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");
                        if (viruscasespicker.SelectedItem.ToString() == "Head Lice")
                        {
                            foreach (var classcode in await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()))
                            {
                                
                                await FirebaseHelper.AddVirusCaseInfo("Student", studentname.SelectedItem.ToString(), person.School, person.City, savedfirebaseauth.User.Email, viruscasespicker.SelectedItem.ToString(), classcode.ToLower());

                                await FirebaseHelper.GetPeopleInClassAndAddNotifsforHeadLiceCases(person.School, person.City, viruscasespicker.SelectedItem.ToString(), classcode.ToLower());
                            }
                        }


                    }
                    else
                    {
                        foreach (var classcode in await FirebaseHelper.GetPersonsClassCodes())
                        {
                            await FirebaseHelper.AddVirusCaseInfo(person.Role, await FirebaseHelper.GetPersonName((person.City + person.School).ToLower().Replace(" ", "")), person.School, person.City, await FirebaseHelper.GetPersonEmail(), viruscasespicker.SelectedItem.ToString(), classcode.ToLower());
                        }

                        if (viruscasespicker.SelectedItem.ToString() == "COVID19")
                        {
                            await FirebaseHelper.GetPeopleInSchoolAndAddNotifsCOVID(person.School, person.City, viruscasespicker.SelectedItem.ToString(), await FirebaseHelper.GetPersonName((person.City + person.School).ToLower().Replace(" ", "")));
                        }
                        await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");


                    }


                }


            }

            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Alert", "Oh no! Token expired!", "Ok");
            }
            /*public async Task<bool> SendNotification(FCMBody fcmBody)
            {
                try
                {
                    var httpContent = JsonConvert.SerializeObject(fcmBody);
                    var client = new HttpClient();
                    var authorization = string.Format("key={0}", serverkey);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
                    var stringContent = new StringContent(httpContent);
                    stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    string uri = "https://fcm.googleapis.com/fcm/send";
                    var response = await client.PostAsync(uri, stringContent).ConfigureAwait(false);
                    var result = response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (TaskCanceledException ex)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            */
        }
        async void EnterOther_Clicked(System.Object sender, System.EventArgs e)
        {
            if (other.Text.ToString() == null)
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Please fill in all fields", "Ok");
            }

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
                    string personrole = "Student";
                    foreach (var code in await FirebaseHelper.GetAllClassCodes((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")))
                    {
                        await FirebaseHelper.AddVirusCaseInfo(personrole, studentname.SelectedItem.ToString(), person.School, person.City, savedfirebaseauth.User.Email, other.Text, code);
                    }

                    if (viruscasespicker.SelectedItem.ToString() == "COVID19")
                    {
                        await FirebaseHelper.GetPeopleInSchoolAndAddNotifsCOVID(person.School, person.City, viruscasespicker.SelectedItem.ToString(), studentname.SelectedItem.ToString());
                    }
                    await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");


                }
                else
                {

                    await FirebaseHelper.AddVirusCaseInfo(person.Role, await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")), person.School, person.City, person.NewUserEmail, other.Text, person.ClassCode);
                    if (viruscasespicker.SelectedItem.ToString() == "COVID19")
                    {
                        await FirebaseHelper.GetPeopleInSchoolAndAddNotifsCOVID(person.School, person.City, viruscasespicker.SelectedItem.ToString(), "Teacher");
                    }
                    await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");

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

