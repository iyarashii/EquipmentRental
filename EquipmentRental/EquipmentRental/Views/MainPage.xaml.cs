// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/EquipmentRental/blob/master/LICENSE for license details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EquipmentRental.Models;
using EquipmentRental.Models.DTOs;

namespace EquipmentRental.Views
{
    public partial class MainPage : ContentPage
    {
        // manager field used for storing EquipmentManager object
        EquipmentManager manager;

        // property that stores whether logged in user is an admin
        public bool IsAdmin { get; set; }

        // property that stores the value whether data setting layout should be displayed
        public bool SettingDate { get; set; }

        // property that stores Equipment item from selected view cell
        public Equipment SelectedEquipment { get; set; }

        // property that stores minimum date for start datepicker element
        public DateTime MinStartDate { get; set; }

        // property that stores minimum date for end datepicker element
        public DateTime MinEndDate { get; set; }

        // constructor
        public MainPage()
        {
            InitializeComponent();
            
            // assigning properties to their default values on page creation
            IsAdmin = App.IsLoggedInUserAnAdmin;
            MinStartDate = DateTime.Now.Date;           // today
            MinEndDate = DateTime.Now.Date.AddDays(1);  // tomorrow
            SettingDate = false;

            // setting datepickers date properties to minimum date
            endDate.Date = MinEndDate;
            startDate.Date = MinStartDate;

            // change buttons grid layout when logged in as admin
            if (IsAdmin)
            {
                buttonsGrid.HorizontalOptions = LayoutOptions.End;
            }
            else
            {
                // when logged in as normal user remove users button form toolbar
                ToolbarItems.RemoveAt(1);
            }

            // set binding context to this page to update bindings with new proprty values
            BindingContext = this;

            // assign new EquipmentManager object to manager field
            manager = EquipmentManager.DefaultManager;

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

        // approve selected equipment table item, update table and replace current listview with updated table items
        async Task ApproveItemRental(Equipment item)
        {
            await manager.ApproveRentalAsync(item, this);
            equipmentList.ItemsSource = await manager.GetTableAsync(this);
        }

        // delete selected equipment table item from the table and replace current listview with updated table items
        async Task DeleteItem(Equipment item)
        {
            await manager.DeleteTableItemAsync(item, this);
            equipmentList.ItemsSource = await manager.GetTableAsync(this);
        }

        // add new equipment item to the table and replace current listview with updated table items
        async Task AddItem(Equipment item)
        {
            await manager.SaveTableItemAsync(item, this);
            equipmentList.ItemsSource = await manager.GetTableAsync(this);
        }

        // update selected equipment table item permission property and replace current listview with updated table items
        async Task AskToRentItem(Equipment item)
        {
            await manager.AskToRentAsync(item, this);
            equipmentList.ItemsSource = await manager.GetTableAsync(this);
        }

        // update selected equipment table item properties to default values 
        // and replace current listview with updated table items
        async Task MarkItemAsReturned(Equipment item)
        {
            await manager.MarkItemAsReturnedAsync(item, this);
            equipmentList.ItemsSource = await manager.GetTableAsync(this);
        }
        
        // refresh listview table items
        private async Task RefreshItems(bool showActivityIndicator)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                equipmentList.ItemsSource = await manager.GetTableAsync(this);
            }
        }


        // -------------------     Event handlers     -------------------
        
        // event handler for clicked logout button
        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            App.IsUserLoggedIn = false;
            App.IsLoggedInUserAnAdmin = false;
            Navigation.InsertPageBefore(new LoginPage(), this);
            await Navigation.PopAsync();
        }

        // event handler for clicked users button
        async void OnUsersButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserList());
        }

        // event handler that is called when you click on list views viewcell
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // pattern matching
            if (e.SelectedItem is Equipment item)
            {
                string action = "";
                BindingContext = null;
                SettingDate = false;
                BindingContext = this;
                if (IsAdmin)
                {
                    // if user is admin call this method and receive string of clicked button
                    action = await OnSelectedAdminActions(item);
                }
                else
                {
                    // if user is not admin call this method and receive string of clicked button
                    action = await OnSelectedUserActions(item);
                }
                // do different things depending on which button was pressed
                switch (action)
                {
                    case "Cancel":
                        break;
                    case "Delete":
                        await DeleteItem(item);
                        break;
                    case "Approve":
                    case "Rent":
                        await DisplayDataSelection(item);
                        break;
                    case "Mark as returned":
                    case "Deny":
                        await MarkItemAsReturned(item);
                        break;
                    default:
                        break;
                }
            }
            // prevents background getting highlighted
            equipmentList.SelectedItem = null;
        }

        // event handler for Add button
        public async void OnAdd(object sender, EventArgs e)
        {
            var item = new Equipment { ItemName = newItemName.Text, Email = null, Username = null };
            await AddItem(item);
            newItemName.Text = string.Empty;
            newItemName.Unfocus();
        }

        // event handler for Delete context action button
        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var item = mi.CommandParameter as Equipment;
            await DeleteItem(item);
        }

        // event handler for Approve and Rent context action buttons
        public async void OnRentOrApprove(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var item = mi.CommandParameter as Equipment;
            await DisplayDataSelection(item);
        }

        // event handler for Mark as returned context action button
        public async void OnReturned(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var item = mi.CommandParameter as Equipment;
            await MarkItemAsReturned(item);
        }

        // event handler for Refresh button
        public async void OnRefreshItems(object sender, EventArgs e)
        {
            await RefreshItems(true);
        }

        // event handler for Accept button
        public async void OnAccept(object sender, EventArgs e)
        {
            BindingContext = null;
            SelectedEquipment.StartDate = startDate.Date;
            SelectedEquipment.EndDate = endDate.Date;
            SelectedEquipment.Username = usernameEntry.Text;
            SelectedEquipment.Email = emailEntry.Text;
            SettingDate = false;
            BindingContext = this;

            if (!IsAdmin)
            {
                SelectedEquipment.Username = UserManager.CurrentUser.Username;
                SelectedEquipment.Email = UserManager.CurrentUser.Email;
                await AskToRentItem(SelectedEquipment);
            }
            else
            {
                await ApproveItemRental(SelectedEquipment);
            }
            usernameEntry.Text = string.Empty;
            emailEntry.Text = string.Empty;
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

        // event handler that is called when view cell binding context changes 
        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            base.OnBindingContextChanged();

            if (BindingContext == null)
                return;

            ViewCell theViewCell = ((ViewCell)sender);

            // pattern matching
            if (theViewCell.BindingContext is Equipment item)
            {
                if (!IsAdmin)
                {
                    OnBindingContextChangedUserActions(theViewCell, item);
                }
                else
                {
                    OnBindingContextChangedAdminActions(theViewCell, item);
                }
            }
        }

        // method used for setting correct layout of date layout when approving or renting item
        public async Task DisplayDataSelection(Equipment item)
        {
            SelectedEquipment = item;
            // reset context to update bindings
            BindingContext = null;
            if (IsAdmin)
            {
                // when approving item set startDate picker and endDate picker to 
                // minimum if approving not rented item or to date set by user who asked for permission to rent
                startDate.Date = SelectedEquipment.StartDate ?? MinStartDate;
                endDate.Date = SelectedEquipment.EndDate ?? MinEndDate;

                // entries for username and email are set to the values of the selected view cell
                usernameEntry.Text = SelectedEquipment.Username;
                emailEntry.Text = SelectedEquipment.Email;
                
                // set binded property and update bindings
                SettingDate = true;
                BindingContext = this;

                // display popup with instructions
                await DisplayAlert("Renting " + item.ItemName + ":", "You can modify selected start date and end date. Username and email of the person who is renting are also adjustable. Click Accept to approve renting to selected person for a selected period of time.", "OK");
            }
            else
            {
                // when renting as user default values of date pickers are the minimum values
                endDate.Date = MinEndDate;
                startDate.Date = MinStartDate;
                
                // set binded property and update bindings
                SettingDate = true;
                BindingContext = this;

                // display popup with instructions
                await DisplayAlert("To rent item " + item.ItemName + ":", "Select start and end date of rental period and click accept to send rent request.", "OK");
            }
        }

        // method that returns selected button name from different action sheet popups depending on selected item properties
        public async Task<string> DisplayAdminActionSheet(Equipment selectedItem)
        {
            if (selectedItem.IsRented)
            {
                return await DisplayActionSheet(selectedItem.ItemName + " options:", "Cancel", "Delete", "Mark as returned");
            }
            else if (selectedItem.IsWaitingForPermission)
            {
                return await DisplayActionSheet(selectedItem.ItemName + " options:", "Cancel", "Delete", "Approve", "Deny");
            }
            else
            {
                return await DisplayActionSheet(selectedItem.ItemName + " options:", "Cancel", "Delete", "Approve");
            }
        }

        // method that is called by OnSelected event handler when user is an admin
        // returns string with clicked button name on Windows and displays popup with instructions on Android
        public async Task<string> OnSelectedAdminActions(Equipment selectedItem)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                await DisplayAlert(selectedItem.ItemName, "Press-and-hold to see item managment options.", "Got it!");
                return "";
            }
            return await DisplayAdminActionSheet(selectedItem);
        }

        // method that is called by OnSelected event handler when user is not an admin
        // returns string with clicked button name on Windows and displays popup with instructions on Android
        public async Task<string> OnSelectedUserActions(Equipment selectedItem)
        {
            if (!selectedItem.IsRented && !selectedItem.IsWaitingForPermission)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    await DisplayAlert(selectedItem.ItemName, "Press-and-hold to ask for permission to rent " + selectedItem.ItemName + ".", "Got it!");
                    return "";
                }
                return await DisplayActionSheet("Item " + selectedItem.ItemName + " options:", "Cancel", null, "Rent");
            }
            return "";
        }

        // method called by OnBindingContextChanged event handler when user is normal user, it modifies context menu of viewcell depending on item properties
        private void OnBindingContextChangedAdminActions(ViewCell eventSendingViewCell, Equipment viewCellsItem)
        {
            if (viewCellsItem.IsRented)
            {
                eventSendingViewCell.ContextActions.RemoveAt(1); // remove Approve button
            }
            if (viewCellsItem.IsWaitingForPermission)
            {
                eventSendingViewCell.ContextActions.RemoveAt(2); // remove Returned button
                // add Deny button to context menu of a viewcell
                var menuDeny = new MenuItem { Text = "Deny" };
                menuDeny.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                menuDeny.Clicked += OnReturned;
                eventSendingViewCell.ContextActions.Add(menuDeny);
            }
            if (!viewCellsItem.IsRented && !viewCellsItem.IsWaitingForPermission)
            {
                eventSendingViewCell.ContextActions.RemoveAt(2); // remove Returned button
            }
        }

        // method called by OnBindingContextChanged event handler when user is normal user, it modifies context menu of viewcell depending on item properties
        private void OnBindingContextChangedUserActions(ViewCell eventSendingViewCell, Equipment viewCellsItem)
        {
            // remove all buttons from view cell context menu
            eventSendingViewCell.ContextActions.Clear();
            if (!viewCellsItem.IsWaitingForPermission && !viewCellsItem.IsRented)
            {
                // add Rent button to context menu of the view cell
                var menuRent = new MenuItem { Text = "Rent" };
                menuRent.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                menuRent.Clicked += OnRentOrApprove;
                eventSendingViewCell.ContextActions.Add(menuRent);
            }
        }
    }
}
