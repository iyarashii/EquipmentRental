using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace EquipmentRental
{
    public partial class MainPage : ContentPage
    {
        EquipmentManager manager;
        public bool IsAdmin { get; set; }
        public bool SettingDate { get; set; }
        public Equipment SelectedEquipment { get; set; }
        public DateTime MinStartDate { get; set; }
        public DateTime MinEndDate { get; set; }

        public MainPage()
        {
            InitializeComponent();

            IsAdmin = App.IsLoggedInUserAnAdmin;
            
            MinStartDate = DateTime.Now.Date;
            MinEndDate = DateTime.Now.Date.AddDays(1);
            endDate.Date = MinEndDate;
            startDate.Date = MinStartDate;


            SettingDate = false;
            if (IsAdmin)
            {
                buttonsGrid.HorizontalOptions = LayoutOptions.End;
            }
            else
            {
                ToolbarItems.RemoveAt(1); // removes users button from toolbar
            }

            BindingContext = this;

            manager = EquipmentManager.DefaultManager;

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


        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            base.OnBindingContextChanged();

            if (BindingContext == null)
                return;

            ViewCell theViewCell = ((ViewCell)sender);
            var item = theViewCell.BindingContext as Equipment;

            if (item != null)
            {
                if (!IsAdmin)
                {
                    theViewCell.ContextActions.Clear();
                    if (item.IsWaitingForPermission == false && item.IsRented == false)
                    {
                        var menuRent = new MenuItem { Text = "Rent" };
                        menuRent.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                        menuRent.Clicked += OnRent;
                        theViewCell.ContextActions.Add(menuRent);
                    }
                }
                else
                {
                    if(item.IsRented == true)
                    {
                        theViewCell.ContextActions.RemoveAt(1); // remove Approve button
                    }
                    if (item.IsWaitingForPermission)
                    {
                        theViewCell.ContextActions.RemoveAt(2); // remove Returned button
                        var menuDeny = new MenuItem { Text = "Deny" };
                        menuDeny.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                        menuDeny.Clicked += OnReturned;
                        theViewCell.ContextActions.Add(menuDeny);
                    }
                    if(!item.IsRented && !item.IsWaitingForPermission)
                    {
                        theViewCell.ContextActions.RemoveAt(2); // remove Returned button
                    }
                }
            }
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
            if (IsAdmin)
            {
                await Navigation.PushAsync(new UserList());

            }
            else
            {
                await DisplayAlert("Access Denied", "Admin rights required.", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Set syncItems to true in order to synchronize the data on startup when running in offline mode
            await RefreshItems(true, syncItems: true);
        }
        // Data methods
        async Task ApproveItemRental(Equipment item)
        {
            await manager.ApproveRentalAsync(item);
            equipmentList.ItemsSource = await manager.GetItemsAsync();
        }

        async Task DeleteItem(Equipment item)
        {
            await manager.DeleteItemAsync(item);
            equipmentList.ItemsSource = await manager.GetItemsAsync();
        }

        async Task AddItem(Equipment item)
        {
            await manager.SaveItemAsync(item);
            equipmentList.ItemsSource = await manager.GetItemsAsync();
        }

        async Task AskToRentItem(Equipment item)
        {
            await manager.AskToRentAsync(item, this);
            equipmentList.ItemsSource = await manager.GetItemsAsync();
        }

        async Task MarkItemAsReturned(Equipment item)
        {
            await manager.MarkItemAsReturnedAsync(item, this);
            equipmentList.ItemsSource = await manager.GetItemsAsync();
        }



        // Event handlers
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as Equipment;
            if (item != null)
            {
                BindingContext = null;
                SettingDate = false;
                BindingContext = this;
                if (IsAdmin)
                {
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        await DisplayAlert("Item " + item.ItemName, "Press-and-hold to see item managment options", "Got it!");
                    }
                    else
                    {
                        string action;
                        // Windows, not all platforms support the Context Actions yet
                        if (item.IsRented == true)
                        {
                            action = await DisplayActionSheet("Item " + item.ItemName + " options:", "Cancel", "Delete", "Mark as returned");
                        }
                        else if(item.IsWaitingForPermission == true)
                        {
                            action = await DisplayActionSheet("Item " + item.ItemName + " options:", "Cancel", "Delete", "Approve", "Deny");
                        }
                        else
                        {
                            action = await DisplayActionSheet("Item " + item.ItemName + " options:", "Cancel", "Delete", "Approve");
                        }
                        switch (action)
                        {
                            case "Cancel":
                                break;
                            case "Delete":
                                await DeleteItem(item);
                                break;
                            case "Approve":
                                await DisplayDataSelection(item);
                                //await ApproveItemRental(item);
                                break;
                            case "Mark as returned":
                            case "Deny":
                                await MarkItemAsReturned(item);
                                break;
                        }
                    }
                }
                else
                {
                    if(item.IsRented == false && item.IsWaitingForPermission == false)
                    {
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            await DisplayAlert("Item " + item.ItemName, "Press-and-hold to ask for permission to rent " + item.ItemName, "Got it!");
                        }
                        else
                        {
                            // Windows, not all platforms support the Context Actions yet
                            string action = await DisplayActionSheet("Item " + item.ItemName + " options:", "Cancel", null, "Rent");
                            switch (action)
                            {
                                case "Cancel":
                                    break;
                                case "Rent":
                                    await DisplayDataSelection(item);
                                    BindingContext = this;
                                    break;
                            }
                        }
                    }
                }
                
            }

            // prevents background getting highlighted
            equipmentList.SelectedItem = null;
        }

        // event handler for "+" button
        public async void OnAdd(object sender, EventArgs e)
        {

            var item = new Equipment { ItemName = newItemName.Text, Email = "", Username = "" };
            await AddItem(item);

            newItemName.Text = string.Empty;
            newItemName.Unfocus();
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#context
        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var item = mi.CommandParameter as Equipment;
            await DeleteItem(item);
        }

        public async void OnApprove(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var item = mi.CommandParameter as Equipment;
            await DisplayDataSelection(item);
        }

        public async void OnRent(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var item = mi.CommandParameter as Equipment;
            await DisplayDataSelection(item);
        }
        public async void OnReturned(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var item = mi.CommandParameter as Equipment;
            await MarkItemAsReturned(item);
        }
        
        public async void OnAcceptDate(object sender, EventArgs e)
        {
            BindingContext = null;
            SelectedEquipment.StartDate = startDate.Date;
            SelectedEquipment.EndDate = endDate.Date;
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
        }

        public async Task DisplayDataSelection(Equipment item)
        {
            SelectedEquipment = item;
            BindingContext = null;
            if (IsAdmin)
            {
                startDate.Date = SelectedEquipment.StartDate ?? MinStartDate;
                endDate.Date = SelectedEquipment.EndDate ?? MinEndDate;
                SettingDate = true;
                BindingContext = this;
                await DisplayAlert("Check rental period for item " + item.ItemName + ".", "You can modify selected start date and end date. Click Accept to approve renting item for selected period of time.", "OK");
            }
            else
            {
                endDate.Date = MinEndDate;
                startDate.Date = MinStartDate;
                SettingDate = true;
                BindingContext = this;
                await DisplayAlert("To rent item " + item.ItemName + ":", "Select start and end date of rental period and click accept to send rent request.", "OK");
            }
            Debug.WriteLine(endDate.Date);
            Debug.WriteLine(startDate.Date);
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
                equipmentList.ItemsSource = await manager.GetItemsAsync(syncItems);
            }
        }
    }
}
