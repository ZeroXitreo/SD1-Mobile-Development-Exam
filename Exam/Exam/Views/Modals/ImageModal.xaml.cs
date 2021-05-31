using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Exam.Models;
using Exam.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Exam.Views.Modals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageModal : ContentPage
    {
        public Exercise Exercise { get; set; }
        private ApiConnectionService<Models.Image> ImageService = new ApiConnectionService<Models.Image>("upload");

        public ImageModal(Exercise exercise)
        {
            Exercise = exercise;
            InitializeComponent();
        }

        private async void UploadImage(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PhotoPath))
            {
                try
                {
                    await ImageService.LoginWrapper<List<Models.Image>>(async () =>
                    {
                        var boundary = Guid.NewGuid().ToString();
                        StreamContent streamContent = new StreamContent(File.OpenRead(PhotoPath));
                        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "\"files\"",
                            FileName = "\"" + Guid.NewGuid() + Path.GetExtension(PhotoPath) + "\"",
                        };
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                        MultipartFormDataContent form = new MultipartFormDataContent(boundary);
                        form.Add(streamContent, "files", Guid.NewGuid() + Path.GetExtension(PhotoPath));
                        form.Add(new StringContent("exercises"), "ref");
                        form.Add(new StringContent("Image"), "field");
                        form.Add(new StringContent(Exercise.id.ToString()), "refId");

                        return await (await ApiConnectionService<Models.Image>.Client).PostAsync(new Uri(ApiConnectionService<Models.Image>.Uri, ImageService.path), form);
                    });

                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
                }
            }
        }


        public string PhotoPath { get; private set; }

        private async void ImageFromCamera(object sender, EventArgs e)
        {
            try
            {
                FileResult photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo);
                ImagePreview.Source = ImageSource.FromFile(PhotoPath);
            }
            catch (Exception ex)
            {
                await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
            }
        }

        private async void ImageFromStorage(object sender, EventArgs e)
        {
            try
            {
                FileResult photo = await MediaPicker.PickPhotoAsync();
                await LoadPhotoAsync(photo);
                ImagePreview.Source = ImageSource.FromFile(PhotoPath);
            }
            catch (Exception ex)
            {
                await DisplayAlert(ex.GetType().Name, ex.Message, "Ok");
            }
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }
            // save the file into local storage
            string newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;
        }
    }
}