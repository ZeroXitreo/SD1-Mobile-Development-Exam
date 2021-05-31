using System;
using System.Collections.Generic;

namespace Exam.Models
{
    public class Exercise : IIdentification
    {
        public int id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTime? published_at { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public List<Image> Image { get; set; }
        public List<User> app_users { get; set; }
        public List<ExercisePlan> exercise_plans { get; set; }
    }
}
