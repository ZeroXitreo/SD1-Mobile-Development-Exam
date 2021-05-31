using Exam.Services;
using System;

namespace Exam.Models
{
    public class Image : IIdentification
    {
        public int id { get; set; }
        public string name { get; set; }
        public string alternativeText { get; set; }
        public string caption { get; set; }
        public string url { get; set; }

        public string path => new Uri(ApiConnectionService<Image>.Uri, url).AbsoluteUri;
    }
}
