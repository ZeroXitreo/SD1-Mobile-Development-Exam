using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Exam
{
    [ContentProperty(nameof(Source))]
    class ImageResource : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!string.IsNullOrEmpty(Source))
            {
                return ImageSource.FromResource(Source, typeof(ImageResource).GetTypeInfo().Assembly);
            }

            return null;
        }
    }
}
