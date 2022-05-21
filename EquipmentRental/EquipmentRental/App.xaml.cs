// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/EquipmentRental/blob/master/LICENSE for license details.

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EquipmentRental.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace EquipmentRental
{
    public partial class App : Application
    {
        public static bool IsUserLoggedIn { get; set; }
        public static bool IsLoggedInUserAnAdmin { get; set; }

        public App()
        {
            InitializeComponent();

            if (!IsUserLoggedIn)
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                MainPage = new NavigationPage(new MainPage());
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
