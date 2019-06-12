using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquipmentRental
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserList : ContentPage
    {
        UserManager manager;
        public UserList()
        {
            InitializeComponent();

            manager = UserManager.DefaultManager;
            if (Device.RuntimePlatform == Device.UWP)
            {
                var refreshButton = new Button
                {
                    Text = "Refresh",
                    HeightRequest = 30
                };
                //refreshButton.Clicked += OnRefreshItems;
                //buttonsPanel.Children.Add(refreshButton);
            }
        }
        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();

        //    // Set syncItems to true in order to synchronize the data on startup when running in offline mode
        //    //await RefreshItems(true, syncItems: true);
        //}
    }
}