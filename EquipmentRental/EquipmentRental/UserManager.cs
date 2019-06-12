using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace EquipmentRental
{
    public partial class UserManager
    {
        static UserManager defaultInstance = new UserManager();
        MobileServiceClient client;

        IMobileServiceTable<User> userTable;

        const string offlineDbPath = @"localstore.db";
        private UserManager()
        {
            client = new MobileServiceClient(Constants.ApplicationURL);
            userTable = client.GetTable<User>();
        }

        public static UserManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        public async Task<ObservableCollection<User>> GetUsersAsync(bool syncItems = false)
        {
            try
            {
                IEnumerable<User> items = await userTable
                    //.Where(User => !User.Done)
                    .ToEnumerableAsync();

                return new ObservableCollection<User>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return null;
        }

        public async Task SaveUserAsync(User item)
        {
            try
            {
                if (item.Id == null)
                {
                    await userTable.InsertAsync(item);
                }
                else
                {
                    await userTable.UpdateAsync(item);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }
    }
}
