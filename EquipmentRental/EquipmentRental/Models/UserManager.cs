// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/EquipmentRental/blob/master/LICENSE for license details.

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
    // class derived from TableManager with generic type parameter - User
    public partial class UserManager : TableManager<User>
    {
        // static property used for creating new object of type UserManager
        public static UserManager DefaultManager { get; private set; } = new UserManager();

        // property used for storing current user data
        public static User CurrentUser { get; set;}
        
        // constructor which derives from TableManager constructor
        private UserManager() : base()
        {

        }

        // method used for saving user data to table
        public async Task<bool> SaveUserAsync(User user, Page currentPage)
        {
            IEnumerable<User> checkIfUsernameTaken;
            IEnumerable<User> checkIfEmailTaken;
            // query table to check if username and email are already a part of database
            checkIfUsernameTaken = await table.Where(User => User.Username == user.Username).ToEnumerableAsync();
            checkIfEmailTaken = await table.Where(User => User.Email == user.Email).ToEnumerableAsync();
            if (checkIfUsernameTaken.FirstOrDefault() != null || checkIfEmailTaken.FirstOrDefault() != null)
            {
                return false;
            }
            else
            {
                // save data to table if user's username and email are not a part of database
                await SaveTableItemAsync(user, currentPage);
                return true;
            }

        }

        // method used for approving users in the database
        public async Task ApproveUserAsync(User user, Page currentPage)
        {
            user.IsConfirmed = true;
            await UpdateTableItemAsync(user, currentPage);
        }

        // method used to search for the user with specific name and password that are passed as parameters
        public async Task<User> FindUserAsync(string username, string password, Page currentPage)
        {
            IEnumerable<User> currentUser;
            try
            {
                // query used for checking if passed login credentials are a part of database table
                currentUser = await table.Where(User => User.Username == username && User.Password == password)
                    .ToEnumerableAsync();
                CurrentUser = currentUser.FirstOrDefault();
                return currentUser.FirstOrDefault();
            }
            catch (Exception e)
            {
                await currentPage.DisplayAlert("Query error: ", e.Message, "OK");
                return null;
            }
        }
    }
}
