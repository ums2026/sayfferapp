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
    public partial class AddInfoPage : ContentPage
    {
        public AddInfoPage()
        {
            InitializeComponent();
            var commonfoodallergies = new List<string>()
                    {
                        "Milk",
                        "Tree nuts",
                        "Peanuts",
                        "Eggs",
                        "Shellfish",
                        "Soy",
                        "Other"

                    };
            commonallergiespicker.ItemsSource = commonfoodallergies;
            GetInfo();
            var permission = new List<string>()
                    {
                        "No, do not show to other parents",
                        "Yes, I want other parents to be notified/shown this information"

                    };
            privacydetails.ItemsSource = permission;
            var severitylist = new List<string>()
            {
                "Very severe, can't be near it",
                "Severe, but can be near it",
                "Mild, just can't eat it",
            };
            allergydetails.ItemsSource = severitylist;

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
                string selectedallergy = commonallergiespicker.SelectedItem.ToString();
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


                        if (await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()) == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "Oops! Please join a class first", "Ok");

                        }
                        else
                        {
                            foreach (var classcode in await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()))
                            {
                                await FirebaseHelper.AddAllergy(studentname.SelectedItem.ToString(), person.Role, person.School, person.City, classcode, selectedallergy, await FirebaseHelper.GetEmail(), allergydetails.SelectedItem.ToString(), privacydetails.SelectedItem.ToString(), await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")));

                            }

                            /*if (privacydetails.SelectedItem.ToString() == "Yes, I want other parents to be notified/shown this information")
                            {
                                await FirebaseHelper.GetPeopleInClassAndAddNotifsforAllergy(person.School, person.City, selectedallergy, person.ClassCode, allergydetails.SelectedItem.ToString());

                                //Used to have the Device.RuntimePlatform, send to all people in class


                            }
                            else
                            {
                                //Used to have the Device.RuntimePlatform, send to teacher and nurse

                            }*/

                        }

                    }
                    else
                    {

                        if (await FirebaseHelper.GetPersonsClassCodes() == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "Oops! Please join a class first", "Ok");

                        }
                        else
                        {
                            foreach (var classcode in await FirebaseHelper.GetPersonsClassCodes())
                            {
                                await FirebaseHelper.AddAllergy(await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")), person.Role, person.School, person.City, classcode, selectedallergy, await FirebaseHelper.GetEmail(), allergydetails.SelectedItem.ToString(), privacydetails.SelectedItem.ToString(), "Teacher");

                            }
                            await App.Current.MainPage.DisplayAlert("Done!", "Added", "Ok");

                            /*if (privacydetails.SelectedItem.ToString() == "Yes, I want other parents to be notified/shown this information")
                            {
                                await FirebaseHelper.GetPeopleInClassAndAddNotifsforAllergy(person.School, person.City, selectedallergy, person.ClassCode, allergydetails.SelectedItem.ToString());
                                if(Device.RuntimePlatform == "Android")
                                {
                                    MessagingCenter.Send(this, "Entered food allergy");
                                }
                            }
                            await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
                            if (Device.RuntimePlatform == "Android")
                            {
                                MessagingCenter.Send(this, "Entered food allergy send to teacher");
                            }*/
                        }

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
                string usableEmail = UsersEmailToDisplay.Replace(".", ",").ToLower();
                Person person = await FirebaseHelper.GetPerson();

                if (person.Role.ToLower() == "parent")
                {

                    if (await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()) == null)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Oops! Please join a class first", "Ok");

                    }
                    else
                    {
                        string personrole = "Parent";
                        /*await FirebaseHelper.AddAllergy(person.StudentName, personrole, person.School, person.City, person.ClassCode, other.Text, person.NewUserEmail, allergydetails.SelectedItem.ToString(), privacydetails.SelectedItem.ToString());
                        if (privacydetails.SelectedItem.ToString() == "Yes, I want other parents to be notified/shown this information")
                        {
                            await FirebaseHelper.GetPeopleInClassAndAddNotifsforAllergy(person.School, person.City, other.Text, person.ClassCode, allergydetails.SelectedItem.ToString());
                            if (Device.RuntimePlatform == "Android")
                            {
                                MessagingCenter.Send(this, "Entered food allergy");
                            }
                        }
                        if (Device.RuntimePlatform == "Android")
                        {
                            MessagingCenter.Send(this, "Entered food allergy send to teacher");
                        }
                        await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");*/
                        foreach (var classcode in await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()))
                        {
                            await FirebaseHelper.AddAllergy(studentname.SelectedItem.ToString(), person.Role, person.School, person.City, classcode, other.Text, await FirebaseHelper.GetEmail(), allergydetails.SelectedItem.ToString(), privacydetails.SelectedItem.ToString(), await FirebaseHelper.GetPersonName((person.City + person.School).Replace(" ", "").ToLower()));
                            await App.Current.MainPage.DisplayAlert("Done!", "Added!", "OK");

                        }
                    }

                }
                else
                {

                    /*if (person.ClassCode == "EmptyAtPresent")
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Oops! Please join a class first", "Ok");

                    }
                    else
                    {*/
                    /*if (privacydetails.SelectedItem.ToString() == "Yes, I want other parents to be notified/shown this information")
                    {
                        await FirebaseHelper.GetPeopleInClassAndAddNotifsforAllergy(person.School, person.City, other.Text, person.ClassCode, allergydetails.SelectedItem.ToString());
                        //Used to have the Device.RuntimePlatform, send to all people in class

                    }
                    else
                    {
                        //Used to have the Device.RuntimePlatform, send to teacher and nurse

                    }*/
                    if ((await FirebaseHelper.GetPersonsClassCodesFromName(await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")))) == null)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Oops! Please join a class first", "Ok");
                    }
                    else
                    {
                        foreach (var classcode in await FirebaseHelper.GetPersonsClassCodes())
                        {
                            await FirebaseHelper.AddAllergy(await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")), person.Role, person.School, person.City, classcode, other.Text, await FirebaseHelper.GetEmail(), allergydetails.SelectedItem.ToString(), privacydetails.SelectedItem.ToString(), "Teacher");

                        }
                        await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
                    }

                    //}
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
