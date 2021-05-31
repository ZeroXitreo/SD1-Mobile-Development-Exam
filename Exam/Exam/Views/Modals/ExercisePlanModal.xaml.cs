using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Exam.Models;
using Exam.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Exam.Views.Modals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExercisePlanModal : ContentPage
    {
        public ExercisePlan ExercisePlan { get; set; }
        public ObservableCollection<Exercise> Exercises { get; set; }

        public ExercisePlanModal(IEnumerable<Exercise> exercises) : this(exercises, new ExercisePlan()
        {
        })
        {
        }

        public ExercisePlanModal(IEnumerable<Exercise> exercises, ExercisePlan exercisePlan)
        {
            if (exercisePlan.exercises == null)
            {
                exercisePlan.exercises = new List<Exercise>();
            }
            foreach (Exercise exercise in exercises)
            {
                if (!exercisePlan.exercises.Contains(exercise))
                {
                    exercisePlan.exercises.Add(exercise);
                }
            }
            ExercisePlan = exercisePlan;
            Exercises = new ObservableCollection<Exercise>(exercisePlan.exercises);



            InitializeComponent();
        }

        private async void Create(object sender, EventArgs e)
        {
            if (ExercisePlan.published_at == null)
            {
                ExercisePlan.published_at = DateTime.UtcNow;
            }

            try
            {
                ApiConnectionService<ExercisePlan> exercisePlanService = new ApiConnectionService<ExercisePlan>("exercise-plans");
                ExercisePlan = await exercisePlanService.Create(ExercisePlan);
                await DisplayAlert("Oprettet", "Der er blevet oprettet en plan", "Ok");
                var message = new EmailMessage
                {
                    Subject = ExercisePlan.Title,
                    Body = ExercisePlan.Description,
                };
                await Email.ComposeAsync(message);
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
    }
}