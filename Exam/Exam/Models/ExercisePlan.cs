using System;
using System.Collections.Generic;

namespace Exam.Models
{
    public class ExercisePlan : IIdentification
    {
        public int id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? published_at { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public List<Exercise> exercises { get; set; }
    }
}
