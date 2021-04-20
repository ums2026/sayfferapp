using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Sayffer.Helper;
using Xamarin.Forms;
using Sayffer.Model;

namespace Sayffer
{
    public partial class SendCustomMessage : ContentPage
    {
        public SendCustomMessage()
        {
            InitializeComponent();
            GetProfilInformationAndRefreshToken();

        }
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        async void GetProfilInformationAndRefreshToken()
        {
            try
            {

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));

                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                string UsersEmailToDisplay = savedfirebaseauth.User.Email;
                Person person = await FirebaseHelper.GetPerson();
                string cityschool = (person.City.ToLower() + person.School.ToLower()).Replace(" ", "");
                var listofclasscodes = new List<string>()
            {
                "All people in school",
                "All teachers",
                "All parents",
                "All administators"
            };
                if(person.Role != "Parent")
                {
                    if (person.Role == "Teacher")
                    {
                        var personclasscode = await FirebaseHelper.GetPersonsClassCodes();
                        ClassCodeSend.ItemsSource = personclasscode;
                    }
                    else
                    {
                        ClassCodeSend.IsVisible = false;
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
                                    if (classcodeforuse != "emptyatpresent" && classcodeforuse != "EmptyAtPresent")
                                    {
                                        listofclasscodes.Add(classcodeforuse.ToLower());

                                    }
                                }

                            }


                        }
                    }


                    if (person.Role != "Nurse" && person.Role != "Admin")
                    {
                        SendTo.SetValue(IsVisibleProperty, false);

                    }
                    else
                    {
                        SendTo.ItemsSource = listofclasscodes;

                    }
                }
                else {                    App.Current.MainPage = new NavigationPage(new DashboardPage());

                    await App.Current.MainPage.DisplayAlert("Oops!", "Error", "OK");
                }
            }
            catch (Exception x)
            { 
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Oops!", "Token Expired", "OK");
            }
        }

        async void Send_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {

                if ((MessageText.Text).Length >= 101)
                {
                    await App.Current.MainPage.DisplayAlert("Oops!", "Message exceeds character limit", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Done!", "Message sent", "OK");
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));

                    var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                    var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                    Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                    string UsersEmailToDisplay = savedfirebaseauth.User.Email;
                    if (SendTo.IsVisible)
                    {
                        if (SendTo.SelectedItem.ToString() == "All people in school" || SendTo.SelectedItem.ToString() == "All teachers" || SendTo.SelectedItem.ToString() == "All parents" || SendTo.SelectedItem.ToString() == "All administators")
                        {
                            if (SendTo.SelectedItem.ToString() == "All people in school")
                            {
                                string CitySchool = (await FirebaseHelper.GetPersonCity() + await FirebaseHelper.GetPersonSchool()).Replace(" ", "").ToLower();
                                await FirebaseHelper.SendMessageToSchool(await FirebaseHelper.GetPersonSchool(), await FirebaseHelper.GetPersonCity(), MessageText.Text, await FirebaseHelper.GetPersonName(CitySchool));     
                            }
                            if (SendTo.SelectedItem.ToString() == "All teachers")
                            {
                                string CitySchool = (await FirebaseHelper.GetPersonCity() + await FirebaseHelper.GetPersonSchool()).Replace(" ", "").ToLower();
                                await FirebaseHelper.SendMessageToTeachers(await FirebaseHelper.GetPersonSchool(), await FirebaseHelper.GetPersonCity(), MessageText.Text, await FirebaseHelper.GetPersonName(CitySchool));
                            }

                        }
                        else
                        {
                            string school = await FirebaseHelper.GetPersonSchool();
                            string city = await FirebaseHelper.GetPersonCity();
                            await FirebaseHelper.SendMessageToClass(school, city, MessageText.Text, ClassCodeSend.SelectedItem.ToString(), await FirebaseHelper.GetPersonName((city + school).ToLower().Replace(" ", "")), ClassCodeSend.SelectedItem.ToString());
                        }
                        

                    }
                    else
                    {
                        string school = await FirebaseHelper.GetPersonSchool();
                        string city = await FirebaseHelper.GetPersonCity();
                        await FirebaseHelper.SendMessageToClass(school, city, MessageText.Text, SendTo.SelectedItem.ToString(), await FirebaseHelper.GetPersonName((city + school).ToLower().Replace(" ", "")), SendTo.SelectedItem.ToString());
                    }

                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Oops!", "Token Expired", "OK");
            }

        }
        async void BackToDashboard_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());
        }
    }
}

