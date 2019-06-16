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
    
    // view of login screen page
	public partial class LoginPage : ContentPage
	{
        // local variable of type UserManager
        UserManager manager;

        // property of type User used to store typed in user information
        private User TypedUser { get; set; }

        // public constructor
        public LoginPage ()
		{
			InitializeComponent ();

            // instantiating UserManager class by using DefaultManager property
            manager = UserManager.DefaultManager;
        }

        // event handler for sing up toolbar button
        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            // push SingUp page onto the top of navigation stack
            await Navigation.PushAsync(new SignUpPage());
        }

        // event handler for login button
        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            // save typed login credentials to local variable user
            var user = new User
            {
                Username = usernameEntry.Text,
                Password = passwordEntry.Text
            };

            // check if credentials are vaild using AreCredentialsCorrect method and save bool returned from Task to local variable
            var isValid = await AreCredentialsCorrect(user, true);

            if (isValid)
            {
                // if credentials are valid check if the user is approved by an admin
                if (!TypedUser.IsConfirmed)
                {
                    // if the user is not confirmed change messageLabel text and end method execution
                    messageLabel.Text = "Account not approved! Try again later or contact admin.";
                    passwordEntry.Text = string.Empty;
                    return;
                }

                // set global properties concerning logged in user
                App.IsUserLoggedIn = true;
                if (TypedUser.IsAdmin)
                {
                    App.IsLoggedInUserAnAdmin = true;
                }

                // insert new MainPage before the current page on navigation stack
                Navigation.InsertPageBefore(new MainPage(), this);

                // pop current page from navigation stack
                await Navigation.PopAsync();
            }
            else
            {
                // if login credentials are not valid update text on the page
                messageLabel.Text = "Login failed";
                passwordEntry.Text = string.Empty;
            }
        }

        // method used for checking whether typed in login credentials match credentials in database
        async Task<bool> AreCredentialsCorrect(User user, bool showActivityIndicator)
        {
            // using statement sets activity indicator scope so that it is active during code execution inside using statement braces
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                // query database to check if typed in credentials are a part of user database
                TypedUser = await manager.FindUserAsync(user.Username, user.Password);
                if (TypedUser == null)
                {
                    return false;
                }
                // return true if password and username are matching a pair in database
                return user.Username == TypedUser.Username && user.Password == TypedUser.Password;
            }
        }
    }
}