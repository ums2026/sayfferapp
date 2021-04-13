using System;
using System.Collections.Generic;
using Sayffer.Helper;
using Xamarin.Forms;

namespace Sayffer
{
    public partial class PeopleEnrolledInClass : ContentPage
    {
        public PeopleEnrolledInClass()
        {
            InitializeComponent();
            GetPeople();
        }
        public async void GetPeople()
        {
            try
            {
                var person = await FirebaseHelper.GetPerson();
                if(person.Role == "Teacher")
                {
                    var list = await FirebaseHelper.GetAllPeopleFromClassCode(person.School, person.City);
                    PeopleInClass.ItemsSource = list;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Oops!", "Error", "OK");
                    App.Current.MainPage = new NavigationPage(new DashboardPage());
                }
            }
            catch(Exception x)
            {
                await App.Current.MainPage.DisplayAlert("Oops!","Error","OK");
                App.Current.MainPage = new NavigationPage(new DashboardPage());
            }
        }
    }
}
