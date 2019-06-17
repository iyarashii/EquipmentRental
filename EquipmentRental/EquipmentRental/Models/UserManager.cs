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
    public partial class UserManager : TableManager<User>
    {
        public static UserManager DefaultManager { get; private set; } = new UserManager();

        public static User CurrentUser { get; set;}
        
        private UserManager() : base()
        {

        }

        public async Task<bool> SaveUserAsync(User user, Page currentPage)
        {
            IEnumerable<User> checkIfUsernameTaken;
            IEnumerable<User> checkIfEmailTaken;
            checkIfUsernameTaken = await table.Where(User => User.Username == user.Username).ToEnumerableAsync();
            checkIfEmailTaken = await table.Where(User => User.Email == user.Email).ToEnumerableAsync();
            if (checkIfUsernameTaken.FirstOrDefault() != null || checkIfEmailTaken.FirstOrDefault() != null)
            {
                return false;
            }
            else
            {
                await SaveTableItemAsync(user, currentPage);
                return true;
            }

        }

        public async Task ApproveUserAsync(User user, Page currentPage)
        {
            user.IsConfirmed = true;
            await UpdateTableItemAsync(user, currentPage);
        }

        public async Task<User> FindUserAsync(string username, string password)
        {
            IEnumerable<User> currentUser;
            try
            {
                currentUser = await table.Where(User => User.Username == username && User.Password == password)
                    .ToEnumerableAsync();
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
