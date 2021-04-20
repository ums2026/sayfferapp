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
    public partial class AddInfoNurses : ContentPage
    {
        public AddInfoNurses()
        {
            InitializeComponent();
            GetProfilInformationAndRefreshToken();
            var options = new List<string>()
                    {
                        "Food Allergies",
                        "Medical-related Allergies",
                        "Virus Cases"

                    };
            AddInfoSpecifics.ItemsSource = options;

            var viruscaseslist = new List<string>()
                    {
                        "COVID19",
                        "Viral Flu",
                        "Stomach Flu",
                        "Head Lice",
                        "Other"

                    };
            viruscasespicker.ItemsSource = viruscaseslist;
            var commonmedicalallergies = new List<string>()
                    {
                        "Latex",
                        "Other"

                    };
            commonmedicalallergiespicker.ItemsSource = commonmedicalallergies;
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
            var severitylist = new List<string>()
            {
                "Very severe, can't be near it",
                "Severe, but can be near it",
                "Mild, just can't eat it",
            };
            allergydetails.ItemsSource = severitylist;
            var permission = new List<string>()
                    {
                        "No, do not show to other parents",
                        "Yes, we have permission for other parents to be notified/shown this information"

                    };
            privacydetails.ItemsSource = permission;
            Enter.SetValue(IsVisibleProperty, true);
            var roles = new List<string>()
                    {
                        "Teacher",
                        "Student",

                    };
            role.ItemsSource = roles;
        }
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        async void GetProfilInformationAndRefreshToken()
        {

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            /*try
            {*/
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));

            FirebaseHelper firebaseHelper = new FirebaseHelper();
            string usableEmail = savedfirebaseauth.User.Email.Replace(".", ",");
            Person person = await FirebaseHelper.GetPerson();
            if(person.Role != "Nurse")
            {                App.Current.MainPage = new NavigationPage(new DashboardPage());
                 
                await App.Current.MainPage.DisplayAlert("Oops!", "Error", "OK");
            }
            else
            {
                string cityschool = (person.City.ToString().ToLower() + person.School.ToString().ToLower()).Replace(" ", "");


                var listofclasscodes = new List<string>();

                var allclasscodes = await FirebaseHelper.GetAllClassCodes(cityschool);
                foreach (string classcode in allclasscodes)
                {
                    string classcodeforuse = classcode;
                    if (listofclasscodes.Contains(classcodeforuse.ToLower()))
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
                ClassCodes.ItemsSource = listofclasscodes;

                /*}
                catch (Exception x)
                {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Alert", "Oh no! Token expired!", "Ok");
                }*/
            }




        }
        async void Enter_Clicked(System.Object sender, System.EventArgs e)
        {
            if (AddInfoSpecifics.SelectedItem.ToString() == null)
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Please fill in all fields", "Ok");
            }
            if (AddInfoSpecifics.SelectedItem.ToString() == "Food Allergies")
            {
                commonallergiespicker.SetValue(IsVisibleProperty, true);
                allergydetails.SetValue(IsVisibleProperty, true);
                commonmedicalallergiespicker.SetValue(IsVisibleProperty, false);
                ClassCodes.SetValue(IsVisibleProperty, true);
                Emailenter.SetValue(IsVisibleProperty, true);
                privacydetails.SetValue(IsVisibleProperty, true);
                viruscasespicker.SetValue(IsVisibleProperty, false);

            }
            else
            {
                commonallergiespicker.SetValue(IsVisibleProperty, false);
                privacydetails.SetValue(IsVisibleProperty, false);
                allergydetails.SetValue(IsVisibleProperty, false);
            }
            if (AddInfoSpecifics.SelectedItem.ToString() == "Virus Cases")
            {
                commonmedicalallergiespicker.SetValue(IsVisibleProperty, false);
                viruscasespicker.SetValue(IsVisibleProperty, true);
                commonallergiespicker.SetValue(IsVisibleProperty, false);
                privacydetails.SetValue(IsVisibleProperty, false);
                allergydetails.SetValue(IsVisibleProperty, false);
                ClassCodes.SetValue(IsVisibleProperty, true);
                Emailenter.SetValue(IsVisibleProperty, true);


            }
            else
            {
                viruscasespicker.SetValue(IsVisibleProperty, false);

            }

            if (AddInfoSpecifics.SelectedItem.ToString() == "Medical-related Allergies")
            {
                commonmedicalallergiespicker.SetValue(IsVisibleProperty, true);
                commonallergiespicker.SetValue(IsVisibleProperty, false);
                allergydetails.SetValue(IsVisibleProperty, false);
                privacydetails.SetValue(IsVisibleProperty, false);
                ClassCodes.SetValue(IsVisibleProperty, true);
                Emailenter.SetValue(IsVisibleProperty, true);
                viruscasespicker.SetValue(IsVisibleProperty, false);



            }
            else
            {
                commonmedicalallergiespicker.SetValue(IsVisibleProperty, false);
            }
            Enter.SetValue(IsVisibleProperty, false);
            EnterInfo.SetValue(IsVisibleProperty, true);
        }

        void BackButton_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());

        }

        async void EnterInfo_Clicked(System.Object sender, System.EventArgs e)
        {
            if (ClassCodes.SelectedItem.ToString() == null)
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Please fill in all fields", "Ok");
            }

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            /*try
            {*/
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));

            FirebaseHelper firebaseHelper = new FirebaseHelper();
            string usableEmail = savedfirebaseauth.User.Email.Replace(".", ",");
            Person person = await FirebaseHelper.GetPerson();

            if (AddInfoSpecifics.SelectedItem.ToString() == "Food Allergies")
            {
                LabelText.SetValue(IsVisibleProperty, true);

                if (commonallergiespicker.SelectedItem.ToString() == "Other")
                {

                    otherFoodAllergy.SetValue(IsVisibleProperty, true);
                    EnterInfo.SetValue(IsVisibleProperty, false);
                    EnterOtherFoodAllergy.SetValue(IsVisibleProperty, true);
                    EnterOtherVirusCase.SetValue(IsVisibleProperty, false);
                    EnterOtherMedicalAllergy.SetValue(IsVisibleProperty, false);

                }
                else
                {
                    if (role.SelectedItem.ToString() == "Student")
                    {


                        await FirebaseHelper.AddAllergy(Name.Text, "Parent", person.School, person.City, ClassCodes.SelectedItem.ToString(), commonallergiespicker.SelectedItem.ToString(), Emailenter.Text, allergydetails.SelectedItem.ToString(), privacydetails.SelectedItem.ToString(), await FirebaseHelper.GetPersonName((person.City + person.School).Replace(" ", "").ToLower()));
                        await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");


                    }
                    else
                    {


                        await FirebaseHelper.AddAllergy(Name.Text, "Teacher", person.School, person.City, ClassCodes.SelectedItem.ToString(), commonallergiespicker.SelectedItem.ToString(), Emailenter.Text, allergydetails.SelectedItem.ToString(), privacydetails.SelectedItem.ToString(), "Teacher");

                        await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");

                    }
                }


            }


            if (AddInfoSpecifics.SelectedItem.ToString() == "Medical-related Allergies")
            {
                Person getpersoninfo = await FirebaseHelper.GetPerson();
                if (commonmedicalallergiespicker.SelectedItem.ToString() == "Other")
                {
                    otherMedicalAllergy.SetValue(IsVisibleProperty, true);
                    Enter.SetValue(IsVisibleProperty, false);
                    EnterOtherFoodAllergy.SetValue(IsVisibleProperty, false);
                    EnterOtherVirusCase.SetValue(IsVisibleProperty, false);
                    EnterOtherMedicalAllergy.SetValue(IsVisibleProperty, true);
                    EnterInfo.SetValue(IsVisibleProperty, false);
                }
                else
                {
                    if (getpersoninfo.Role.ToLower() == "parent")
                    {
                        string personrole = "Student";
                        await FirebaseHelper.AddMedicalAllergyStudent(ParentName.Text, personrole, Name.Text, person.School, person.City, ClassCodes.SelectedItem.ToString(), commonmedicalallergiespicker.SelectedItem.ToString(), Emailenter.Text);
                        await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
                    }
                    else
                    {
                        await FirebaseHelper.AddMedicalAllergyTeacher(Name.Text, "Teacher", person.School, person.City, ClassCodes.SelectedItem.ToString(), commonmedicalallergiespicker.SelectedItem.ToString(), Emailenter.Text);

                        await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
                    }

                }



            }

            if (AddInfoSpecifics.SelectedItem.ToString() == "Virus Cases")
            {
                Person getpersoninfo = await FirebaseHelper.GetPerson();
                if (viruscasespicker.SelectedItem.ToString() == "Other")
                {

                    otherVirusCase.SetValue(IsVisibleProperty, true);
                    Enter.SetValue(IsVisibleProperty, false);
                    EnterOtherVirusCase.SetValue(IsVisibleProperty, true);
                    EnterInfo.SetValue(IsVisibleProperty, false);
                    EnterOtherMedicalAllergy.SetValue(IsVisibleProperty, false);
                    EnterOtherFoodAllergy.SetValue(IsVisibleProperty, false);

                }
                else
                {
                    if (role.SelectedItem.ToString() == "Student")
                    {

                        await FirebaseHelper.AddVirusCaseInfo("Student", Name.Text, getpersoninfo.School, getpersoninfo.City, Emailenter.Text, viruscasespicker.SelectedItem.ToString(), ClassCodes.SelectedItem.ToString());
                        if (viruscasespicker.SelectedItem.ToString() == "COVID19")
                        {
                            await FirebaseHelper.GetPeopleInSchoolAndAddNotifsCOVID(getpersoninfo.School, getpersoninfo.City, viruscasespicker.SelectedItem.ToString(), Name.Text);
                            //Send to all people in the school, push notif.  Used to have the Device.RuntimePlatform method
                        }
                        await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");


                    }
                    else
                    {


                        await FirebaseHelper.AddVirusCaseInfo(getpersoninfo.Role, Name.Text, getpersoninfo.School, getpersoninfo.City, Emailenter.Text, viruscasespicker.SelectedItem.ToString(), ClassCodes.SelectedItem.ToString());
                        if (viruscasespicker.SelectedItem.ToString() == "COVID19")
                        {
                            await FirebaseHelper.GetPeopleInSchoolAndAddNotifsCOVID(getpersoninfo.School, getpersoninfo.City, viruscasespicker.SelectedItem.ToString(), Name.Text);
                        }
                        await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");


                    }



                }
            }
        }

        async void EnterOtherFoodAllergy_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));

            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));

            FirebaseHelper firebaseHelper = new FirebaseHelper();
            string usableEmail = savedfirebaseauth.User.Email.Replace(".", ",");
            Person person = await FirebaseHelper.GetPerson();

            if (role.SelectedItem.ToString() == "Student")
            {

                string personrole = "Parent";
                await FirebaseHelper.AddAllergy(Name.Text, personrole, person.School, person.City, ClassCodes.SelectedItem.ToString(), otherFoodAllergy.Text, Emailenter.Text, allergydetails.SelectedItem.ToString(), privacydetails.SelectedItem.ToString(), ParentName.Text);

                await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");

            }
            else
            {


                await FirebaseHelper.AddAllergy(Name.Text, "Teacher", person.School, person.City, ClassCodes.SelectedItem.ToString(), otherFoodAllergy.Text, Emailenter.Text, allergydetails.SelectedItem.ToString(), privacydetails.SelectedItem.ToString(), "Teacher");

                await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");

            }
        }
        async void EnterOtherVirusCase_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));

            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;

            FirebaseHelper firebaseHelper = new FirebaseHelper();
            string usableEmail = UsersEmailToDisplay.Replace(".", ",");
            Person person = await FirebaseHelper.GetPerson();
            if (role.SelectedItem.ToString() == "Teacher")
            {


                string personrole = "Teacher";
                await FirebaseHelper.AddVirusCaseInfo(personrole, Name.Text, person.School, person.City, Emailenter.Text, otherVirusCase.Text, ClassCodes.SelectedItem.ToString());
                await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");


            }
            else
            {


                await FirebaseHelper.AddVirusCaseInfo("Student", Name.Text, person.School, person.City, Emailenter.Text, otherVirusCase.Text, ClassCodes.SelectedItem.ToString());
                await App.Current.MainPage.DisplayAlert("Sucess", "Added, feel better soon!", "Ok");

            }





        }
        async void EnterOtherMedicalAllergy_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));

            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;

            FirebaseHelper firebaseHelper = new FirebaseHelper();
            string usableEmail = UsersEmailToDisplay.Replace(".", ",");
            Person person = await FirebaseHelper.GetPerson();
            if (role.SelectedItem.ToString() == "Student")
            {
                string personrole = "Student";
                await FirebaseHelper.AddMedicalAllergyStudent(ParentName.Text, personrole, Name.Text, person.School, person.City, ClassCodes.SelectedItem.ToString(), otherMedicalAllergy.Text, Emailenter.Text);
                await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
            }
            else
            {
                await FirebaseHelper.AddMedicalAllergyTeacher(Name.Text, role.SelectedItem.ToString(), person.School, person.City, ClassCodes.SelectedItem.ToString(), otherMedicalAllergy.Text, Emailenter.Text);
                await App.Current.MainPage.DisplayAlert("Sucess", "Done!", "Ok");
            }

        }

        void role_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (role.SelectedItem.ToString() == "Student")
            {
                ParentName.SetValue(IsVisibleProperty, true);
            }
            else
            {
                ParentName.SetValue(IsVisibleProperty, false);

            }
        }
    }
}
