using System;
using System.Collections.Generic;

namespace Exam.Models
{
    public class User : IIdentification
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? published_at { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public List<Exercise> exercises { get; set; }
    }
}
