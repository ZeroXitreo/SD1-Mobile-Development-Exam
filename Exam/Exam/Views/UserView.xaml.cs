using Exam.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Exam.Views.Modals;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Exam.Services;

namespace Exam.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserView : ContentPage
    {
        public User User { get; set; }
        private ApiConnectionService<Exercise> ExerciseService { get; } = new ApiConnectionService<Exercise>("exercises");
        public ObservableCollection<Exercise> Exercises { get; set; } = new ObservableCollection<Exercise>();
        public ObservableCollection<Exercise> CheckedExercises { get; set; } = new ObservableCollection<Exercise>();

        public UserView()
        {
            User = new User();
            InitializeComponent();
        }

        public UserView(User user)
        {
            User = user;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            CheckedExercises.Clear();
            List<Exercise> exercises = await ExerciseService.Get();
            Exercises.Clear();
            foreach (Exercise exercise in exercises)
            {
                if (exercise.app_users.Any(user => user.id == User.id))
                {
                    Exercises.Add(exercise);
                }
            }
        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (listView.SelectedItem is Exercise exercise)
                {
                    await Navigation.PushAsync(new ExerciseView(User, exercise));
                }
                listView.SelectedItem = null;
            }
        }

        private async void CreateExercise(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExerciseModal(User));
        }

        private async void UpdateUser(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserModal(User));
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.BindingContext is Exercise exercise)
                {
                    if (checkBox.IsChecked)
                    {
                        CheckedExercises.Add(exercise);
                    }
                    else
                    {
                        CheckedExercises.Remove(exercise);
                    }
                }
            }
        }

        private async void CreateExercisePlan(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExercisePlanModal(CheckedExercises));
            //if (Exercise.app_users.Count > 0)
            //{
            //    try
            //    {
            //        var message = new EmailMessage
            //        {
            //            Subject = Exercise.Title,
            //            Body = Exercise.Description,
            //            To = new List<string>() {
            //                Exercise.app_users[0].email
            //            }
            //        };
            //        await Email.ComposeAsync(message);
            //    }
            //    catch (FeatureNotSupportedException fbsEx)
            //    {
            //        // Email is not supported on this device
            //    }
            //    catch (Exception ex)
            //    {
            //        // Some other exception occurred
            //    }
            //}
        }
    }
}