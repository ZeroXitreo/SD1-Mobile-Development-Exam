using System;
using Exam.Models;
using Exam.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Exam.Views.Modals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterModal : ContentPage
    {
        public User User { get; set; }
        private ApiConnectionService<User> UserService { get; } = new ApiConnectionService<User>("app-users");

        public string RepeatPassword { get; set; }

        public RegisterModal()
        {
            User = new User();
            InitializeComponent();
        }

        private async void Register(object sender, EventArgs e)
        {
            if (User.password == RepeatPassword)
            {
                User.published_at = DateTime.UtcNow;

                try
                {
                    User = await UserService.Create(User);
                    Application.Current.MainPage = new NavigationPage(new UserView(User));
                }
                catch (Exception ex)
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
                }
            }
            else
            {
                await DisplayAlert("Error", "Password doesn't match eachother", "Ok");
            }
        }
        private async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}