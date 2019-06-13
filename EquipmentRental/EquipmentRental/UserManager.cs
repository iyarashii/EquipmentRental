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
        public static UserManager DefaultManager { get; private set; } = new UserManager();
        public MobileServiceClient CurrentClient { get; }

        private User CurrentUser { get; set;}

        IMobileServiceTable<User> userTable;

        const string offlineDbPath = @"localstore.db";

        private UserManager()
        {
            CurrentClient = new MobileServiceClient(Constants.ApplicationURL);
            userTable = CurrentClient.GetTable<User>();
        }

        public async Task<ObservableCollection<User>> GetUsersAsync(bool syncUsers = false)
        {
            try
            {
                IEnumerable<User> users = await userTable
                    .ToEnumerableAsync();

                return new ObservableCollection<User>(users);
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

        public async Task<bool> SaveUserAsync(User user)
        {
            IEnumerable<User> checkIfUsernameTaken;
            IEnumerable<User> checkIfEmailTaken;
            try
            {
                checkIfUsernameTaken = await userTable.Where(User => User.Username == user.Username).ToEnumerableAsync();
                checkIfEmailTaken = await userTable.Where(User => User.Email == user.Email).ToEnumerableAsync();
                if(checkIfUsernameTaken.FirstOrDefault() != null || checkIfEmailTaken.FirstOrDefault() != null)
                {
                    return false;
                }
                if (user.Id == null)
                {
                    await userTable.InsertAsync(user);
                }
                else
                {
                    await userTable.UpdateAsync(user);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
                return false;
            }
        }
        public async Task DeleteUserAsync(User user)
        {
            try
            {
                await userTable.DeleteAsync(user);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Delete error: {0}", new[] { e.Message });
            }
        }

        public async Task ApproveUserAsync(User user)
        {
            user.IsConfirmed = true;
            try
            {
                await userTable.UpdateAsync(user);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Update error: {0}", new[] { e.Message });
            }
        }

        public async Task<User> FindUserAsync(string username, string password)
        {
            IEnumerable<User> currentUser;
            try
            {
                currentUser = await userTable.Where(User => User.Username == username)
                    .Where(User => User.Password == password).ToEnumerableAsync();
                CurrentUser = currentUser.FirstOrDefault();
                return currentUser.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Query error: {0}", new[] { e.Message });
                return null;
            }
        }
    }
}
