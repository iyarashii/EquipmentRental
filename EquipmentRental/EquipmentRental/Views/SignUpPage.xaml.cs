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
	public partial class SignUpPage : ContentPage
	{
        UserManager manager;

        public SignUpPage ()
		{
			InitializeComponent ();
            manager = UserManager.DefaultManager;
        }
        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            var user = new User()
            {
                Username = usernameEntry.Text,
                Password = passwordEntry.Text,
                Email = emailEntry.Text
            };
            var checkIfUsersDatabaseIsEmpty =  await manager.GetUsersAsync();
            if (checkIfUsersDatabaseIsEmpty.Count == 0)
            {
                user.IsAdmin = true;
                user.IsConfirmed = true;
            }
            // Sign up logic goes here

            var signUpSucceeded = await AreDetailsValid(user);
            if (signUpSucceeded && user.IsAdmin)
            {
                var rootPage = Navigation.NavigationStack.FirstOrDefault();
                if (rootPage != null)
                {
                    App.IsUserLoggedIn = true;
                    App.IsLoggedInUserAnAdmin = true;
                    Navigation.InsertPageBefore(new MainPage(), Navigation.NavigationStack.First());
                    await Navigation.PopToRootAsync();
                }
            }
            else if(signUpSucceeded && !user.IsAdmin)
            {
                var rootPage = Navigation.NavigationStack.FirstOrDefault();
                if (rootPage != null)
                {   
                    await Navigation.PopToRootAsync();
                }
            }
            else
            {
                messageLabel.Text = "Sign up failed";
            }
        }

        async Task<bool> AreDetailsValid(User user)
        {
          bool showActivityIndicator = true;
          using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
          {
             if(!string.IsNullOrWhiteSpace(user.Username) && !string.IsNullOrWhiteSpace(user.Password) && !string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains("@"))
             {
                return await manager.SaveUserAsync(user);
             }
             return false;
          }
        }
    }
}