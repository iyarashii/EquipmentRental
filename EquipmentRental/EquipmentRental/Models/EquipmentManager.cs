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
        //public MobileServiceClient CurrentClient { get; }

        //readonly IMobileServiceTable<Equipment> equipmentTable;

        private EquipmentManager() : base()
        {
            //CurrentClient = new MobileServiceClient(Constants.ApplicationURL);
            //equipmentTable = CurrentClient.GetTable<Equipment>();
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

            
        //public async Task<ObservableCollection<Equipment>> GetItemsAsync(bool syncUsers = false)
        //{
        //    try
        //    {
        //        IEnumerable<Equipment> items;
        //        if (App.IsLoggedInUserAnAdmin)
        //        {
        //            items = await equipmentTable
        //            .ToEnumerableAsync();
        //        }
        //        else
        //        {
        //            items = await equipmentTable
        //            .Where(Equipment => !Equipment.IsRented && Equipment.Username == null || Equipment.Username == UserManager.CurrentUser.Username && Equipment.Email == UserManager.CurrentUser.Email)
        //            .ToEnumerableAsync();
        //        }
        //        return new ObservableCollection<Equipment>(items);
        //    }
        //    catch (MobileServiceInvalidOperationException msioe)
        //    {
        //        Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("Sync error: {0}", new[] { e.Message });
        //    }
        //    return null;
        //}
        //public async Task SaveItemAsync(Equipment item)
        //{
        //    try
        //    {
        //         await equipmentTable.InsertAsync(item);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("Save error: {0}", new[] { e.Message });
        //    }
        //}

        //public async Task DeleteItemAsync(Equipment item)
        //{
        //    try
        //    {
        //        await equipmentTable.DeleteAsync(item);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("Delete error: {0}", new[] { e.Message });
        //    }
        //}

        public async Task ApproveRentalAsync(Equipment item, Page currentPage)
        {
            item.IsWaitingForPermission = false;
            item.IsRented = true;
            await UpdateTableItemAsync(item, currentPage);
            //try
            //{
            //    await equipmentTable.UpdateAsync(item);
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine("Update error: {0}", new[] { e.Message });
            //}
        }

        public async Task AskToRentAsync(Equipment item, Page currentPage)
        {
            item.IsWaitingForPermission = true;
            await UpdateTableItemAsync(item, currentPage);
            //try
            //{
            //    await equipmentTable.UpdateAsync(item);
            //}
            //catch (Exception e)
            //{
            //    await page.DisplayAlert("Update error:", e.Message, "OK");
            //}
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
            //try
            //{
            //    await equipmentTable.UpdateAsync(item);
            //}
            //catch (Exception e)
            //{
            //    await page.DisplayAlert("Update error:", e.Message, "OK");
            //}
        }

    }
}
