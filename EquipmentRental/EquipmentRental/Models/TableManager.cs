using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace EquipmentRental
{
    public partial class TableManager<T>
    {
        //public static TableManager<T> DefaultManager { get; private set; } = new TableManager<T>();
        public MobileServiceClient CurrentClient { get; }
        protected readonly IMobileServiceTable<T> table;
        protected IEnumerable<T> tableItems;
        protected TableManager()
        {
            CurrentClient = new MobileServiceClient(Constants.ApplicationURL);
            table = CurrentClient.GetTable<T>();
        }


        public virtual async Task TableToEnumerableAsync()
        {
            tableItems = await table.ToEnumerableAsync();
        }


        public async Task<ObservableCollection<T>> GetTableAsync(Page currentPage)
        {
            try
            {
                await TableToEnumerableAsync();
                return new ObservableCollection<T>(tableItems);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                await currentPage.DisplayAlert("Invalid sync operation: ", msioe.Message, "OK");
            }
            catch (Exception e)
            {
                await currentPage.DisplayAlert("Sync error: ", e.Message, "OK");
            }
            return null;
        }


        public async Task DeleteTableItemAsync(T tableItem, Page currentPage)
        {
            try
            {
                await table.DeleteAsync(tableItem);
            }
            catch (Exception e)
            {
                await currentPage.DisplayAlert("Delete error: ", e.Message, "OK");
            }
        }


        public async Task SaveTableItemAsync(T tableItem, Page currentPage)
        {
            try
            {
                await table.InsertAsync(tableItem);
            }
            catch (Exception e)
            {
                await currentPage.DisplayAlert("Save error: ", e.Message, "OK");
            }
        }

        public async Task UpdateTableItemAsync(T tableItem, Page currentPage)
        {
            try
            {
                await table.UpdateAsync(tableItem);
            }
            catch (Exception e)
            {
                await currentPage.DisplayAlert("Update error: ", e.Message, "OK");
            }
        }

    }
}
