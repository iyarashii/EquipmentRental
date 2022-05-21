// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/EquipmentRental/blob/master/LICENSE for license details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EquipmentRental.Models;
using EquipmentRental.Models.DTOs;

namespace EquipmentRental.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserList : ContentPage
    {
        // manager field used for storing UserManager object
        UserManager manager;

        // constructor
        public UserList()
        {
            InitializeComponent();

            // assign new UserManager object to manager field
            manager = UserManager.DefaultManager;

            // on Windows add refresh button to buttons panel
            if (Device.RuntimePlatform == Device.UWP)
            {
                var refreshButton = new Button
                {
                    Text = "Refresh",
                };
                refreshButton.Clicked += OnRefreshItems;
                buttonsPanel.Children.Add(refreshButton);
            }
        }

        // override OnAppearing method to refresh list view items before this page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // refresh listview items
            await RefreshItems(true);
        }

        // -------------------     Data methods    -------------------

        // approve selected user table item, update table and replace current listview with updated table items
        async Task ApproveUser(User user)
        {
            await manager.ApproveUserAsync(user, this);
            userList.ItemsSource = await manager.GetTableAsync(this);
        }

        // delete selected user table item from the table and replace current listview with updated table items
        async Task DeleteUser(User user)
        {
            await manager.DeleteTableItemAsync(user, this);
            userList.ItemsSource = await manager.GetTableAsync(this);
        }

        // refresh listview table items
        private async Task RefreshItems(bool showActivityIndicator)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                userList.ItemsSource = await manager.GetTableAsync(this);
            }
        }

        // -------------------     Event handlers     -------------------

        // event handler that is called when you click on list views viewcell
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // pattern matching
            if (e.SelectedItem is User user)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    // on android display this popup message
                    await DisplayAlert(user.Username, "Press-and-hold to see user "+ user.Username +  " managment options.", "Got it!");
                }
                else
                {
                    // on windows display action sheet popup with 3 buttons
                    string action = await DisplayActionSheet("User " + user.Username + " options:", "Cancel", "Delete", "Approve");
                    
                    // do different things depending on which button was pressed
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

        // event handler for clicked logout button
        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            // logs user out and changes page to login page
            App.IsUserLoggedIn = false;
            App.IsLoggedInUserAnAdmin = false;
            Application.Current.MainPage = new NavigationPage(new LoginPage());
            await Navigation.PopToRootAsync();
        }

        // event handler for Delete context action button
        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var user = mi.CommandParameter as User;
            await DeleteUser(user);
        }

        // event handler for Approve context action button
        public async void OnApprove(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var user = mi.CommandParameter as User;
            await ApproveUser(user);
        }

        // event handler for Refresh button
        public async void OnRefreshItems(object sender, EventArgs e)
        {
            await RefreshItems(true);
        }

        // event handler that is called when listview is being refreshed
        public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false);
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
    }
}