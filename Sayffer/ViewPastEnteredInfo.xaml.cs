using System;
using System.Collections.Generic;
using Sayffer.Helper;
using Xamarin.Forms;

namespace Sayffer
{
    public partial class ViewPastEnteredInfo : ContentPage
    {
        public ViewPastEnteredInfo()
        {
            InitializeComponent();
            GetInfo();
        }
        async void GetInfo()
        {
            try
            {
                var list = await FirebaseHelper.GetPastEntered();
                InfoDisplay.ItemsSource = list;
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Oops!", "Token Expired", "OK");
            }

        }

        void Back_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());
        }
    }
}
