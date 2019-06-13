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
                    //HeightRequest = 30
                };
                refreshButton.Clicked += OnRefreshItems;
                buttonsPanel.Children.Add(refreshButton);
            }
        }

        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            App.IsUserLoggedIn = false;
            App.IsLoggedInUserAnAdmin = false;
            Navigation.InsertPageBefore(new LoginPage(), this);
            await Navigation.PopAsync();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Set syncItems to true in order to synchronize the data on startup when running in offline mode
            await RefreshItems(true, syncItems: true);
        }
        // Data methods
        async Task ApproveUser(User user)
        {
            await manager.ApproveUserAsync(user);
            userList.ItemsSource = await manager.GetUsersAsync();
        }

        async Task DeleteUser(User user)
        {
            await manager.DeleteUserAsync(user);
            userList.ItemsSource = await manager.GetUsersAsync();
        }

        // Event handlers
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var user = e.SelectedItem as User;
            if ( user != null)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                   await DisplayAlert("User " + user.Username, "Press-and-hold to see user managment options" + user.Username, "Got it!");
                   //string action = await DisplayActionSheet("User " + user.Username + " options:", "Cancel", "Delete",  "Approve");
                }
                else
                {
                    // Windows, not all platforms support the Context Actions yet
                    //if (await DisplayAlert("Mark completed?", "Do you wish to delete " + user.Username + "?", "Delete", "Cancel"))
                    string action = await DisplayActionSheet("User " + user.Username + " options:", "Cancel", "Delete", "Approve");
                    switch (action)
                    {
                        case "Cancel":
                            break;
                        case "Delete":
                            await DeleteUser(user);
                            break;
                        case "Approve":
                            await ApproveUser(user);
                            break;
                    }
                }
            }

            // prevents background getting highlighted
            userList.SelectedItem = null;
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#context
        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var user = mi.CommandParameter as User;
            await DeleteUser(user);
        }

        public async void OnApprove(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var user = mi.CommandParameter as User;
            await ApproveUser(user);
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#pulltorefresh
        public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }


        public async void OnRefreshItems(object sender, EventArgs e)
        {
            await RefreshItems(true, false);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                userList.ItemsSource = await manager.GetUsersAsync(syncItems);
            }
        }
    }
}