using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquipmentRental
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    // view of signup screen page
    public partial class SignUpPage : ContentPage
	{
        // local variable of type UserManager
        UserManager manager;

        // public constructor
        public SignUpPage ()
		{
			InitializeComponent ();

            // instantiating UserManager class by using DefaultManager property
            manager = UserManager.DefaultManager;
        }

        // event handler for sing up button
        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            // save typed login credentials and email to local variable user
            var user = new User()
            {
                Username = usernameEntry.Text,
                Password = passwordEntry.Text,
                Email = emailEntry.Text
            };
            
            // save user table from database to local variable as observable collection
            var checkIfUsersDatabaseIsEmpty =  await manager.GetUsersAsync();
            // if saved table is empty
            if (checkIfUsersDatabaseIsEmpty.Count == 0)
            {
                // set typed user as confirmed admin
                user.IsAdmin = true;
                user.IsConfirmed = true;
            }
            
            // call method that checks if typed in details are valid and save result to local variable
            var signUpSucceeded = await AreDetailsValid(user, true);

            if (signUpSucceeded && user.IsAdmin)
            {
                // save first element of navigation stack to local variable
                var rootPage = Navigation.NavigationStack.FirstOrDefault();
                if (rootPage != null)
                {
                    // set global properties
                    App.IsUserLoggedIn = true;
                    App.IsLoggedInUserAnAdmin = true;

                    // insert new MainPage before current page on navigation stack
                    Navigation.InsertPageBefore(new MainPage(), Navigation.NavigationStack.First());
                    
                    // pop all but the root page off the navigation stack
                    await Navigation.PopToRootAsync();
                }
            }
            else if(signUpSucceeded && !user.IsAdmin)
            {
                // save first element of navigation stack to local variable
                var rootPage = Navigation.NavigationStack.FirstOrDefault();
                if (rootPage != null)
                {
                    // pop all but the root page off the navigation stack
                    await Navigation.PopToRootAsync();
                }
            }
            else
            {
                messageLabel.Text = "Sign up failed";
            }
        }

        // method used for checking whether typed in sign up details are valid
        async Task<bool> AreDetailsValid(User user, bool showActivityIndicator)
        {
          // using statement sets activity indicator scope so that it is active during code execution inside using statement braces
          using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
          {
             if(!string.IsNullOrWhiteSpace(user.Username) && !string.IsNullOrWhiteSpace(user.Password) && !string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains("@"))
             {
                // return true if saving typed in user details to database succeeded
                return await manager.SaveUserAsync(user);
             }
             return false;
          }
        }
    }
}