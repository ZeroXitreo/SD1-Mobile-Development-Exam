using Exam.Models;
using Exam.Services;
using Exam.Views.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Exam.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPage
    {
        public User User { get; set; }
        private ApiConnectionService<User> UserService { get; } = new ApiConnectionService<User>("app-users");

        public LoginView()
        {
            User = new User();
            InitializeComponent();
            Animation animation = new Animation(v => LoadingIcon.Rotation = v, 0, 360);
            animation.Commit(LoadingIcon, "LoadingIconAnimation", 16, 2000, Easing.Linear, (v, c) => LoadingIcon.Rotation = 0, () => true);
        }

        protected override async void OnAppearing()
        {
            IsBusy = true;
            try
            {
                string userid = await SecureStorage.GetAsync("userid");
                if (!string.IsNullOrEmpty(userid))
                {
                    User user = await UserService.Get(userid);
                    await RunNewStack(user);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
            }
            IsBusy = false;
        }

        private async void Login(object sender, EventArgs e)
        {
            IsBusy = true;
            List<User> users = await UserService.Get(new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("email", User.email),
                new KeyValuePair<string, string>("password", User.password),
            });

            if (users.Count == 1)
            {
                try
                {
                    await SecureStorage.SetAsync("userid", users[0].id.ToString());
                    await RunNewStack(users[0]);
                }
                catch (Exception ex)
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
                }
            }
            else
            {
                await DisplayAlert("Error", "Username or password was incorrect, please try again", "OK");
            }
            IsBusy = false;
        }

        private async void Register(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterModal());
        }

        private async Task RunNewStack(User user)
        {
            Application.Current.MainPage = new NavigationPage(new UserView(user));
        }
    }
}