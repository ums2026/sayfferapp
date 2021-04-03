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
    public partial class NavigateToAddInfoPages : ContentPage
    {
        public NavigateToAddInfoPages()
        {
            InitializeComponent();


        }
        void AddInfo_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AddInfoPage());

        }
        void AddMedicalInfo_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AddMedicalInfo());

        }
        void BackButton_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());

        }


        void addinfocaseofvirus_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AddInfoAboutVirusCases());

        }


    }
}
