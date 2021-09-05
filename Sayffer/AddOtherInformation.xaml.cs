using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Sayffer.Helper;
using Sayffer.Model;
using Microsoft.AppCenter.Crashes;

namespace Sayffer
{
    public partial class AddOtherInformation : ContentPage
    {
        public AddOtherInformation()
        {
            InitializeComponent();
            var othercaseslist = new List<string>()
                    {
                        "Viral Flu",
                         "Stomach Flu",
                        "Head Lice",
                        "Other"

                    };
            casespicker.ItemsSource = othercaseslist;
            GetInfo();
        }

        async void GetInfo()
        {
            Person person = await FirebaseHelper.GetPerson();

            if (person.Role == "Parent")
            {
                studentname.ItemsSource = await FirebaseHelper.GetPersonStudentName();
                studentname.SetValue(IsVisibleProperty, true);


            }
        }


        async void Enter_Clicked(System.Object sender, System.EventArgs e)
        {
            if (casespicker.SelectedItem.ToString() == null)
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Please fill in all fields", "Ok");
            }
            try
            {

                string UsersEmailToDisplay = (await FirebaseHelper.GetUser()).Email;
                string selectedinfo = casespicker.SelectedItem.ToString();
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
                    var personclasscodes = await FirebaseHelper.GetPersonsClassCodes();
                    if (personclasscodes == null)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Oops! Please join a class first", "Ok");

                    }
                    else
                    {
                        string city = await FirebaseHelper.GetPersonCity();
                        string school = await FirebaseHelper.GetPersonSchool();
                        string cityschool = (city + school).Replace(" ", "").ToLower();
                        if (person.Role.ToLower() == "parent")
                        {

                            foreach (var classcode in await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()))
                            {

                                await FirebaseHelper.AddVirusCaseInfo("Student", studentname.SelectedItem.ToString(), person.School, person.City, UsersEmailToDisplay, casespicker.SelectedItem.ToString(), classcode.ToLower());

                            }
                            await FirebaseHelper.AddEnteredInfoForVirusCases(casespicker.SelectedItem.ToString());

                            await FirebaseHelper.SendNurseNotificationsForVirusCase(city, school, personclasscodes[0]);

                            await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");
                            if (casespicker.SelectedItem.ToString() == "Head Lice")
                            {
                                foreach (var classcode in await FirebaseHelper.GetPersonsClassCodesFromName(studentname.SelectedItem.ToString()))
                                {

                                    await FirebaseHelper.AddVirusCaseInfo("Student", studentname.SelectedItem.ToString(), person.School, person.City, UsersEmailToDisplay, casespicker.SelectedItem.ToString(), classcode.ToLower());

                                    await FirebaseHelper.GetPeopleInClassAndAddNotifsforHeadLiceCases(person.School, person.City, classcode.ToLower());
                                }
                            }

                            if (casespicker.SelectedItem.ToString() == "Viral Flu")
                            {
                                await FirebaseHelper.AddViralFluNumbers(person.School, person.City);
                            }

                        }
                        else
                        {
                            await FirebaseHelper.AddEnteredInfoForVirusCases(casespicker.SelectedItem.ToString());

                            foreach (var classcode in await FirebaseHelper.GetPersonsClassCodes())
                            {
                                await FirebaseHelper.AddVirusCaseInfo(person.Role, await FirebaseHelper.GetPersonName((person.City + person.School).ToLower().Replace(" ", "")), person.School, person.City, await FirebaseHelper.GetPersonEmail(), casespicker.SelectedItem.ToString(), classcode.ToLower());
                            }
                            await FirebaseHelper.SendNurseNotificationsForVirusCase(city, school, personclasscodes[0]);


                            await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");
                            if (casespicker.SelectedItem.ToString() == "Viral Flu")
                            {
                                await FirebaseHelper.AddViralFluNumbers(person.School, person.City);
                            }

                        }


                    }

                }


            }

            catch (Exception x)
            {
                Crashes.TrackError(x);
                await App.Current.MainPage.DisplayAlert("Alert", "Oops! Try logging in again.", "Ok");
            }

        }
        async void EnterOther_Clicked(System.Object sender, System.EventArgs e)
        {
            if (other.Text.ToString() == null)
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Please fill in all fields", "Ok");
            }

            try
            {

                string UsersEmailToDisplay = (await FirebaseHelper.GetUser()).Email;

                FirebaseHelper firebaseHelper = new FirebaseHelper();
                string usableEmail = UsersEmailToDisplay.Replace(".", ",");
                Person person = await FirebaseHelper.GetPerson();
                var personclasscodes = await FirebaseHelper.GetPersonsClassCodes();
                await FirebaseHelper.AddEnteredInfoForVirusCases(other.Text);

                if (person.Role.ToLower() == "parent")
                {
                    string personrole = "Student";
                    foreach (var code in await FirebaseHelper.GetAllClassCodes((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")))
                    {
                        await FirebaseHelper.AddVirusCaseInfo(personrole, studentname.SelectedItem.ToString(), person.School, person.City, UsersEmailToDisplay, other.Text, code);
                    }


                    await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");

                    await FirebaseHelper.SendNurseNotificationsForVirusCase(person.City, person.School, personclasscodes[0]);

                }
                else
                {
                    foreach (var code in await FirebaseHelper.GetAllClassCodes((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")))
                    {
                        await FirebaseHelper.AddVirusCaseInfo(person.Role, await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")), person.School, person.City, await FirebaseHelper.GetPersonEmail(), other.Text, code);

                    }


                    await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");
                    await FirebaseHelper.SendNurseNotificationsForVirusCase(person.City, person.School, personclasscodes[0]);

                }

                if (casespicker.SelectedItem.ToString() == "Viral Flu")
                {
                    await FirebaseHelper.AddViralFluNumbers(person.School, person.City);
                }
                if (other.Text.ToLower() == "covid-19" || other.Text.ToLower().Replace("-", "") == "covid19" || other.Text.ToLower() == "covid" || other.Text.ToLower().Replace(" ","") == "covid19")

                {
                    if(person.Role == "Teacher")
                    {
                        await FirebaseHelper.GetPeopleInSchoolAndAddNotifsCOVID(person.School, person.City, "COVID-19", await FirebaseHelper.GetPersonName((person.City.ToLower() + person.School.ToLower()).Replace(" ", "")));

                    }
                    else
                    {
                        await FirebaseHelper.GetPeopleInSchoolAndAddNotifsCOVID(person.School, person.City, "COVID-19", studentname.SelectedItem.ToString());

                    }

                };

            }
            catch (Exception x)
            {
                Crashes.TrackError(x);

                await App.Current.MainPage.DisplayAlert("Alert", "Oops! Try logging in again.", "Ok");



            }
        }

        void BackButton_Clicked(System.Object sender, System.EventArgs e)

        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());

        }
    }
}

