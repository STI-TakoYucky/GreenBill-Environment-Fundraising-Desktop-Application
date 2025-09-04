using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GreenBill.MVVM.ViewModel;
using System;

namespace GreenBill.MVVM.View.FundraisingSteps
{
    public partial class ThirdStep : UserControl
    {
        private byte[] _imageBytes;

        public ThirdStep()
        {
            InitializeComponent();
        }

        // Click → open file dialog
        private void UploadArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };
            if (dialog.ShowDialog() == true)
            {
                DisplayImage(dialog.FileName);
            }
        }

        // Drag enter → show copy effect
        private void UploadArea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        // Drop → display image
        private void UploadArea_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && File.Exists(files[0]))
                {
                    DisplayImage(files[0]);
                }
            }
        }

        // Helper method to load and show image
        private void DisplayImage(string filePath)
        {
            try
            {
                // Convert image file to byte array
                _imageBytes = File.ReadAllBytes(filePath);

                // Update the ViewModel with the image bytes
                if (DataContext is FundraisingStepsViewModel viewModel && viewModel.CurrentCampaign != null)
                {
                    viewModel.CurrentCampaign.Image = _imageBytes;
                }

                // Display the image in the UI
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new System.Uri(filePath);
                bitmap.EndInit();



                PreviewImage.Source = bitmap;
                PreviewImage.Visibility = Visibility.Visible;
                UploadPlaceholder.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Optional: Method to convert BitmapImage to byte array (if you need it)
        private byte[] BitmapImageToByteArray(BitmapImage bitmapImage)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        // Optional: Method to convert byte array back to BitmapImage (for display)
        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(byteArray))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}