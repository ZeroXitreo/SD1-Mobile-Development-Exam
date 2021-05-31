using System;
using System.Collections.Generic;
using Exam.Models;
using Exam.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Exam.Views.Modals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseModal : ContentPage
    {
        public string ButtonText { get; set; }
        public User User { get; set; }
        private ApiConnectionService<Exercise> ExerciseService = new ApiConnectionService<Exercise>("exercises");
        public Exercise Exercise { get; set; }

        public ExerciseModal(User user) : this(user, new Exercise()
        {
        })
        {
        }

        public ExerciseModal(User user, Exercise exercise)
        {
            if (exercise.id == 0)
            {
                Title = "Opret øvelse";
                ButtonText = Title;
            }
            else
            {
                Title = "Opdatér øvelse";
                ButtonText = Title;
                ToolbarItem item = new ToolbarItem()
                {
                    Text = "Slet øvelse",
                    IconImageSource = (ImageSource)new ImageResource()
                    {
                        Source = "Exam.Images.baseline_delete_white_48dp.png"
                    }.ProvideValue(null),
                };
                item.Clicked += Delete;
                ToolbarItems.Add(item);
            }
            User = user;
            if (exercise.app_users == null)
            {
                exercise.app_users = new List<User>();
            }
            exercise.app_users.Add(user);
            Exercise = exercise;
            InitializeComponent();
        }

        private async void Create(object sender, EventArgs e)
        {
            if (Exercise.published_at == null)
            {
                Exercise.published_at = DateTime.UtcNow;
            }

            try
            {
                if (Exercise.id == 0)
                {
                    Exercise = await ExerciseService.Create(Exercise);
                    await Navigation.PopAsync();
                    await Navigation.PushAsync(new ExerciseView(User, Exercise));
                }
                else
                {
                    Exercise = await ExerciseService.Update(Exercise);
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
            bool accept = await DisplayAlert("Slet øvelse", "Er du sikker på du vil slette øvelsen '" + Exercise.Title + "'?", "Slet", "Annuller");
            if (accept)
            {
                try
                {
                    await ExerciseService.Delete(Exercise);
                    await Navigation.PopAsync();
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
                }
            }
        }
    }
}