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
	public partial class LoginPage : ContentPage
	{
        UserManager manager;
        public bool IsTypedUserConfirmed { get; set; }
        public LoginPage ()
		{
			InitializeComponent ();

            manager = UserManager.DefaultManager;
        }
        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var user = new User
            {
                Username = usernameEntry.Text,
                Password = passwordEntry.Text
            };

            var isValid = await AreCredentialsCorrect(user, true);
            if (isValid)
            {
                if (!IsTypedUserConfirmed)
                {
                    messageLabel.Text = "Account not approved! Try again later or contact admin.";
                    passwordEntry.Text = string.Empty;
                    return;
                }
                App.IsUserLoggedIn = true;
                Navigation.InsertPageBefore(new MainPage(), this);
                await Navigation.PopAsync();
            }
            else
            {
                messageLabel.Text = "Login failed";
                passwordEntry.Text = string.Empty;
            }
        }

        async Task<bool> AreCredentialsCorrect(User user, bool showActivityIndicator)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                var typedUser = await manager.FindUserAsync(user.Username, user.Password);
                if (typedUser == null)
                {
                    return false;
                }
                IsTypedUserConfirmed = typedUser.IsConfirmed;
                return user.Username == typedUser.Username && user.Password == typedUser.Password;
            }
        }
    }
}