using Exam.Models;
using System.Collections.ObjectModel;
using Exam.Views.Modals;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Exam.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace Exam.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseView : ContentPage
    {
        public User User { get; set; }
        public Exercise Exercise { get; set; }

        public ExerciseView(User user, Exercise exercise)
        {
            User = user;
            Exercise = exercise;
            InitializeComponent();
        }

        private async void CreateImage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ImageModal(Exercise));
        }

        private async void UpdateExercise(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExerciseModal(User, Exercise));
        }
    }
}