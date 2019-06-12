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
                var currentUser = await manager.FindUserAsync(user.Username, user.Password);
                if (currentUser == null)
                {
                    return false;
                }
                return user.Username == currentUser.Username && user.Password == currentUser.Password;
            }
        }

        //private class ActivityIndicatorScope : IDisposable
        //{
        //    private bool showIndicator;
        //    private ActivityIndicator indicator;
        //    private Task indicatorDelay;

        //    public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
        //    {
        //        this.indicator = indicator;
        //        this.showIndicator = showIndicator;

        //        if (showIndicator)
        //        {
        //            indicatorDelay = Task.Delay(2000);
        //            SetIndicatorActivity(true);
        //        }
        //        else
        //        {
        //            indicatorDelay = Task.FromResult(0);
        //        }
        //    }

        //    private void SetIndicatorActivity(bool isActive)
        //    {
        //        this.indicator.IsVisible = isActive;
        //        this.indicator.IsRunning = isActive;
        //    }

        //    public void Dispose()
        //    {
        //        if (showIndicator)
        //        {
        //            indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
        //        }
        //    }
        //}
    }
}