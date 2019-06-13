using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EquipmentRental
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            App.IsUserLoggedIn = false;
            App.IsLoggedInUserAnAdmin = false;
            Navigation.InsertPageBefore(new LoginPage(), this);
            await Navigation.PopAsync();
        }

        async void OnUsersButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserList());
        }
    }
}
