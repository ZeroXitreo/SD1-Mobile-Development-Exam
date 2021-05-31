using System;
using Exam.Models;
using Exam.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Exam.Views.Modals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserModal : ContentPage
    {
        public string ButtonText { get; set; }
        private ApiConnectionService<User> UserService { get; } = new ApiConnectionService<User>("app-users");
        public User User { get; set; }

        public UserModal() : this(new User()
        {
            DateOfBirth = DateTime.Now.AddYears(-18),
        })
        {
        }

        public UserModal(User user)
        {
            if (user.id == 0)
            {
                Title = "Opret bruger";
                ButtonText = Title;
            }
            else
            {
                Title = "Opdatér bruger";
                ButtonText = Title;

                ToolbarItem item = new ToolbarItem()
                {
                    Text = "Log ud",
                    IconImageSource = (ImageSource)new ImageResource()
                    {
                        Source = "Exam.Images.outline_logout_white_48dp.png"
                    }.ProvideValue(null),
                };
                item.Clicked += Logout;
                ToolbarItems.Add(item);

                /*item = new ToolbarItem()
                {
                    Text = "Slet bruger",
                    IconImageSource = (ImageSource)new ImageResource()
                    {
                        Source = "Exam.Images.baseline_delete_white_48dp.png"
                    }.ProvideValue(null),
                };
                item.Clicked += Delete;
                ToolbarItems.Add(item);*/
            }
            User = user;
            InitializeComponent();
        }

        private async void Create(object sender, EventArgs e)
        {
            if (User.published_at == null)
            {
                User.published_at = DateTime.UtcNow;
            }

            try
            {
                if (User.id == 0)
                {
                    User = await UserService.Create(User);
                    await Navigation.PopAsync();
                    await Navigation.PushAsync(new UserView(User));
                }
                else
                {
                    User = await UserService.Update(User);
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
            }
        }

        private async void Delete(object sender, EventArgs e)
        {
            bool accept = await DisplayAlert("Slet bruger", "Er du sikker på du vil slette brugeren '" + User.Name + "'?", "Slet", "Annuller");
            if (accept)
            {
                try
                {
                    await UserService.Delete(User);
                    await Navigation.PopToRootAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
                }
            }
        }

        private async void Logout(object sender, EventArgs e)
        {
            bool accept = await DisplayAlert("Log ud", "Er du sikker på du vil logge ud?", "Log ud", "Annuller");
            if (accept)
            {
                try
                {
                    SecureStorage.Remove("userid");
                    Application.Current.MainPage = new NavigationPage(new LoginView());
                }
                catch (Exception ex)
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
                }
            }
        }
    }
}