using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using EquipmentRental.Models.DTOs;

namespace EquipmentRental.Models
{
    // class derived from TableManager with generic type parameter of Equipment
    public partial class EquipmentManager : TableManager<Equipment>
    {
        // static property used for creating new object of type EquipmentManager
        public static EquipmentManager DefaultManager { get; private set; } = new EquipmentManager();

        // constructor which derives from TableManager constructor
        private EquipmentManager() : base()
        {
        }

        // override of virtual method that gets elements of the table asynchronously
        public override async Task TableToEnumerableAsync()
        {
            // when user is not an admin only get table items concerning user or ones that are not rented 
            if (!App.IsLoggedInUserAnAdmin)
            {
                tableItems = await table
                   .Where(Equipment => !Equipment.IsRented && Equipment.Username == null ||
                   Equipment.Username == UserManager.CurrentUser.Username &&
                   Equipment.Email == UserManager.CurrentUser.Email)
                   .ToEnumerableAsync();
            }
            else
            {
                await base.TableToEnumerableAsync();
            }
        }

        // method that updates item properties concerning renting
        public async Task ApproveRentalAsync(Equipment item, Page currentPage)
        {
            item.IsWaitingForPermission = false;
            item.IsRented = true;
            await UpdateTableItemAsync(item, currentPage);
        }

        // method that sets waiting for permission item property flag to true and updates table
        public async Task AskToRentAsync(Equipment item, Page currentPage)
        {
            item.IsWaitingForPermission = true;
            await UpdateTableItemAsync(item, currentPage);
        }

        // method that sets table item properties to default ones, so that item is available to rent
        public async Task MarkItemAsReturnedAsync(Equipment item, Page currentPage)
        {
            item.IsWaitingForPermission = false;
            item.IsRented = false;
            item.Username = null;
            item.Email = null;
            item.StartDate = null;
            item.EndDate = null;
            await UpdateTableItemAsync(item, currentPage);
        }
    }
}
