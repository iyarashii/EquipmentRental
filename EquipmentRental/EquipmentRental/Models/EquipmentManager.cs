using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace EquipmentRental
{
    public partial class EquipmentManager : TableManager<Equipment>
    {
        public static EquipmentManager DefaultManager { get; private set; } = new EquipmentManager();

        private EquipmentManager() : base()
        {
        }
        public override async Task TableToEnumerableAsync()
        {
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

        public async Task ApproveRentalAsync(Equipment item, Page currentPage)
        {
            item.IsWaitingForPermission = false;
            item.IsRented = true;
            await UpdateTableItemAsync(item, currentPage);
        }

        public async Task AskToRentAsync(Equipment item, Page currentPage)
        {
            item.IsWaitingForPermission = true;
            await UpdateTableItemAsync(item, currentPage);
        }

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
