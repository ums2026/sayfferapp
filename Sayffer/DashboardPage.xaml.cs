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
using System.Threading.Tasks;
using System.Linq;

namespace Sayffer
{
    public partial class DashboardPage : ContentPage
    {
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        public DashboardPage()
        {

            InitializeComponent();



        }
        protected override void OnAppearing()
        {
            /* var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
             var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
             var RefreshedContent = authProvider.RefreshAuthAsync(savedfirebaseauth);
             Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
             string UsersEmailToDisplay = savedfirebaseauth.User.Email;
             string personcity = await FirebaseHelper.GetPersonCity(UsersEmailToDisplay);
             if(personcity != "false")
             {*/
            /*if (Device.RuntimePlatform == "Android")
            {
                MessagingCenter.Send(this, "OnDashboardPage");

            }*/
            //}
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

                
                
                if (UsersEmailToDisplay == null)
                {
                    await App.Current.MainPage.DisplayAlert("Oops!", "Token Expired - issues loading info", "Ok");
                }

                else
                {
                    Username.Text = UsersEmailToDisplay;

                    string usableEmail = UsersEmailToDisplay.Replace(".", ",");
                    Person person = await FirebaseHelper.GetPerson();

                    if (person == null)
                    {
                        await App.Current.MainPage.DisplayAlert("Oops!", "There was some error loading profile info", "Ok");

                    }
                    else
                    {
                        Notifications.Text = "Notifications" + " (" + (await FirebaseHelper.GetUnreadNotifications(person.City, person.School, usableEmail)).Count().ToString() + ")";

                        if (person.Role == "Admin")
                        {
                            CreateNurseCode.SetValue(IsVisibleProperty, true);
                        }
                        else
                        {
                            CreateNurseCode.SetValue(IsVisibleProperty, false);

                        }
                        if (await FirebaseHelper.GetPersonsClassCodes() == null)
                        {
                            ViewDataParentsAndTeachers.SetValue(IsVisibleProperty, false);
                        }
                        else
                        {
                            var classes = await FirebaseHelper.GetPersonsClassCodes();
                            string classcode = "";

                            foreach (var code in classes)
                            {
                                classcode = classcode + code + ", ";

                            }
                            DisplayClasses.Text = "Classes: " + classcode;
                        }

                        if (person.Role.ToLower() == "nurse")
                        {
                            InfoLstParents.SetValue(IsVisibleProperty, false);
                            InfoLstTeachers.SetValue(IsVisibleProperty, false);
                            DisplayClasses.SetValue(IsVisibleProperty, false);
                            ViewInfo.SetValue(IsVisibleProperty, true);
                            AddInfo.SetValue(IsVisibleProperty, false);
                            AddInfoForNurses.SetValue(IsVisibleProperty, true);

                        }
                        else
                        {
                            ViewInfo.SetValue(IsVisibleProperty, false);
                            AddInfoForNurses.SetValue(IsVisibleProperty, false);


                        }
                        DisplaySchool.Text = person.School;
                        DisplayCity.Text = person.City;

                        DisplayName.Text = await FirebaseHelper.GetPersonName((person.City + person.School).ToLower().Replace(" ", ""));
                        DisplayRole.Text = person.Role;
                        if (DisplayRole.Text.ToLower() == "teacher")
                        {
                            NewClassButton.SetValue(IsVisibleProperty, true);
                            ViewPeopleInClass.SetValue(IsVisibleProperty, true);
                        }
                        else
                        {
                            NewClassButton.SetValue(IsVisibleProperty, false);

                        }
                        if (DisplayRole.Text.ToLower() == "parent")

                        {
                            if (DisplayClasses.Text != "Classes: ")
                            {
                                var fulllist = new List<Person>();

                                foreach (var code in await FirebaseHelper.GetPersonsClassCodes())
                                {
                                    var list = await FirebaseHelper.GetClassAllergiesInfoParents(person.City, person.School, code);
                                    foreach (var item in list)
                                    {
                                        fulllist.Add(item);
                                    }
                                }
                                InfoLstParents.ItemsSource = fulllist;

                                labelheader.Text = "Food allergies in the class that you are enrolled in:";

                            }

                            SendCustomMessage.SetValue(IsVisibleProperty, false);
                        }
                        else
                        {
                            JoinClassButton.SetValue(IsVisibleProperty, false);

                        }

                        if (person.Role == "Teacher")
                        {
                            if (DisplayClasses.Text != "Classes: ")
                            {
                                var fulllist = new List<Person>();

                                foreach (var code in await FirebaseHelper.GetPersonsClassCodes())
                                {
                                    var list = await FirebaseHelper.GetClassAllergiesInfo(person.City, person.School, code);
                                    foreach (var item in list)
                                    {
                                        fulllist.Add(item);
                                    }
                                }
                                InfoLstTeachers.ItemsSource = fulllist;

                                labelheader.Text = "Food allergies in the class that you are enrolled in:";

                            }
                        }

                        if (person.Role == "Nurse" || DisplayClasses.Text == "Classes: ")
                        {
                            labelheader.SetValue(IsVisibleProperty, false);
                            DisplayClasses.SetValue(IsVisibleProperty, false);
                            ViewDataParentsAndTeachers.SetValue(IsVisibleProperty, false);
                        }

                        if (DisplayClasses.Text != "Classes: ")
                        {

                            labelheader.SetValue(IsVisibleProperty, true);
                            if (InfoLstParents.ItemsSource == null)
                            {
                                if (person.Role.ToString() == "Teacher")
                                {
                                    labelheader.Text = "Food allergies in the class that you are enrolled in:";

                                }
                                else
                                {
                                    labelheader.SetValue(IsVisibleProperty, false);
                                    InfoLstParents.SetValue(IsVisibleProperty, false);
                                }

                            }
                            else
                            {
                                labelheader.Text = "Food allergies in the class that you are enrolled in:";
                                if (InfoLstTeachers.ItemsSource == null)
                                {
                                    InfoLstTeachers.SetValue(IsVisibleProperty, false);
                                }
                                else
                                {
                                    labelheader.Text = "Food allergies in the class that you are enrolled in:";
                                }

                            }

                            if (DisplayRole.Text == "Parent")
                            {
                                InfoLstParents.SetValue(IsVisibleProperty, true);
                                var fulllist = new List<Person>();

                                foreach (var code in await FirebaseHelper.GetPersonsClassCodes())
                                {
                                    var list = await FirebaseHelper.GetClassAllergiesInfoParents(person.City, person.School, code);
                                    foreach (var item in list)
                                    {
                                        fulllist.Add(item);
                                    }
                                }
                                InfoLstParents.ItemsSource = fulllist;
                                InfoLstTeachers.SetValue(IsVisibleProperty, false);
                                DisplayClasses.SetValue(IsVisibleProperty, true);

                            }


                            else
                            {
                                InfoLstParents.SetValue(IsVisibleProperty, false);
                                InfoLstTeachers.SetValue(IsVisibleProperty, false);
                                if (person.Role.ToLower() == "teacher")
                                {
                                    InfoLstParents.SetValue(IsVisibleProperty, false);
                                    InfoLstTeachers.SetValue(IsVisibleProperty, true);
                                    DisplayClasses.SetValue(IsVisibleProperty, true);
                                }
                                else
                                {
                                    InfoLstTeachers.SetValue(IsVisibleProperty, false);
                                }

                            }
                            var COVIDcasesinschool = await FirebaseHelper.GetCovidNumbersSchool(person.City, person.School);
                            if (COVIDcasesinschool == null)
                            {
                                COVIDCasesInSchool.Text = "No COVID-19 Cases have been entered for this school";
                            }
                            else
                            {
                                if (COVIDcasesinschool.NumberOfCases == "1")
                                {
                                    COVIDCasesInSchool.Text = "There is " + COVIDcasesinschool.NumberOfCases + " current case of COVID-19 reported in your school.";

                                }
                                else
                                {
                                    COVIDCasesInSchool.Text = "There are " + COVIDcasesinschool.NumberOfCases + " current cases of COVID-19 reported in your school.";

                                }
                            }


                            if (COVIDcasesinschool != null)
                            {


                                MostRecentCOVIDDate.Text = "Most recent COVID-19 case in your school was reported on " + COVIDcasesinschool.Time;


                            }
                            else
                            {
                                MostRecentCOVIDDate.SetValue(IsVisibleProperty, false);
                            }
                        }


                    }
                    if (DisplayClasses.Text == "Classes: ")
                    {
                        AddInfo.SetValue(IsVisibleProperty, false);
                    }

                    if(person.Role == "Admin")
                    {
                        NursesSignedUp.SetValue(IsVisibleProperty, true);
                        NursesSignedUp.Text = "Number of Nurses in this school with Sayffer accounts: " + await FirebaseHelper.GetNumberOfPeopleInSchoolRegistered(person.School, person.City, "Nurse");
                        AdminSignedUp.SetValue(IsVisibleProperty, true);
                        AdminSignedUp.Text = "Number of Admin in this school with Sayffer accounts: " + await FirebaseHelper.GetNumberOfPeopleInSchoolRegistered(person.School, person.City, "Admin");
                        TeachersSignedUp.SetValue(IsVisibleProperty, true);
                        TeachersSignedUp.Text = "Number of Teachers in this school with Sayffer accounts: " + await FirebaseHelper.GetNumberOfPeopleInSchoolRegistered(person.School, person.City, "Teacher");
                        ParentsSignedUp.SetValue(IsVisibleProperty, true);
                        ParentsSignedUp.Text = "Number of Parents in this school with Sayffer accounts: " + await FirebaseHelper.GetNumberOfPeopleInSchoolRegistered(person.School, person.City, "Parent");
                        AdminLabel.SetValue(IsVisibleProperty, true);
                        AdminLabel.Text = "If you see that there are more people signed up than there should be, please contact us through our website at sayffer.com";
                        
                    }
                }

            }

            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Oops!", "Some error, please log back in", "Ok");
                App.Current.MainPage = new NavigationPage(new MainPage());
                Preferences.Remove("MyFirebaseRefreshToken");



            }

        }






        void LogOut_Clicked(System.Object sender, System.EventArgs e)
        {
            Preferences.Remove("MyFirebaseRefreshToken");
            App.Current.MainPage = new NavigationPage(new MainPage());

        }

        void NewClassButton_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new TeachersCreateClassCode());

        }
        void SendCustomMessage_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new SendCustomMessage());

        }

        void JoinClassButton_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new ParentsJoinClassPage());

        }
        async void AddInfo_Clicked(System.Object sender, System.EventArgs e)
        {
            if (DisplayClasses.Text == null)
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Please join/create a class first", "Ok");
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new NavigateToAddInfoPages());

            }

        }
        void AddInfoNurses_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AddInfoNurses());

        }
        void ViewInfo_Clicked(System.Object sender, System.EventArgs e)
        {

            App.Current.MainPage = new NavigationPage(new ViewInfoPage());

        }
        async void ViewDataParentsAndTeachers_Clicked(System.Object sender, System.EventArgs e)
        {
            if (DisplayClasses.Text != null)
            {
                App.Current.MainPage = new NavigationPage(new ViewInfoParentsAndTeachers());

            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Oops!", "Please join/create a class first", "Ok");

            }
        }

        void Notifications_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new Notifications());
        }

        void CreateNurseCode_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new CreateNurseVerificationCode());
        }

        void ViewPastEnteredInfo_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new ViewPastEnteredInfo());
        }
        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await Launcher.OpenAsync(new Uri("https://www.sayffer.com/"));

        }

        void ViewPeopleInClass_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new PeopleEnrolledInClass());
        }
    }
}
