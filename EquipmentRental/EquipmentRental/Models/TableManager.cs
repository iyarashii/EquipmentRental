// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/EquipmentRental/blob/master/LICENSE for license details.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace EquipmentRental.Models
{
    // abstract base model manager class meant to be used as a template for other table managers
    // T is DTO class used for specific table
    public abstract partial class TableManager<T>
    {
        // mobile service client property with getter only
        public MobileServiceClient CurrentClient { get; }

        // field which provides operations on a table for a mobile service
        protected readonly IMobileServiceTable<T> table;

        // field used for storing table data as enumerable collection
        protected IEnumerable<T> tableItems;

        // constructor
        protected TableManager()
        {
            // initialize new instance of mobile service client with azure endpoint as app url
            CurrentClient = new MobileServiceClient(Constants.ApplicationURL);

            // initialize mobile service table for data operations 
            table = CurrentClient.GetTable<T>();
        }

        // virtual method used for getting elements of the table asynchronously
        public virtual async Task TableToEnumerableAsync()
        {
            tableItems = await table.ToEnumerableAsync();
        }

        /// <summary>
        /// Returns elements of table as observable collection.
        /// </summary>
        /// <param name="currentPage">Page on which alerts will be displayed.</param>
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

        // delete table item passed as argument from the managers table
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

        // save table item passed as parameter to the current managers table
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

        // update table item passed as parameter
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
