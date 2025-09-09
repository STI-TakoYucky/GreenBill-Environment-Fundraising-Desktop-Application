using GreenBill.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GreenBill.MVVM.View
{
    /// <summary>
    /// Interaction logic for SupportingDocumentsPage.xaml
    /// </summary>
    public partial class SupportingDocumentsPage : UserControl
    {
        public SupportingDocumentsPage()
        {
            InitializeComponent();

            // Set up event handlers for drag and drop
            UploadZone.Drop += UploadZone_Drop;
            UploadZone.DragOver += UploadZone_DragOver;
            UploadZone.DragEnter += UploadZone_DragEnter;
            UploadZone.DragLeave += UploadZone_DragLeave;
            UploadZone.MouseLeftButtonUp += UploadZone_MouseLeftButtonUp;

            // Enable drag and drop
            UploadZone.AllowDrop = true;

            // Bind document name textbox to update upload button state
            DocumentNameTextBox.TextChanged += DocumentNameTextBox_TextChanged;
        }

        private void DocumentNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is SupportingDocumentsPageViewModel viewModel)
            {
                viewModel.DocumentName = DocumentNameTextBox.Text;
            }
        }

        private void UploadZone_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SupportingDocumentsPageViewModel viewModel)
            {
                if (viewModel.BrowseFileCommand.CanExecute(null))
                {
                    viewModel.BrowseFileCommand.Execute(null);
                }
            }
        }

        private void UploadZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                UploadZone.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)); // Light blue
                UploadZone.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 151, 167)); // Darker cyan
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void UploadZone_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void UploadZone_DragLeave(object sender, DragEventArgs e)
        {
            // Reset upload zone appearance
            UploadZone.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250)); // Original background
            UploadZone.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 188, 212)); // Original border
        }

        private async void UploadZone_Drop(object sender, DragEventArgs e)
        {
            try
            {
                // Reset upload zone appearance
                UploadZone_DragLeave(sender, e);

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    if (files != null && files.Length > 0)
                    {
                        string filePath = files[0]; // Take the first file only

                        // Validate file extension
                        if (IsValidFileType(filePath))
                        {
                            // Validate file size
                            var fileInfo = new System.IO.FileInfo(filePath);
                            const long maxFileSize = 25 * 1024 * 1024; // 25MB

                            if (fileInfo.Length <= maxFileSize)
                            {
                                if (DataContext is SupportingDocumentsPageViewModel viewModel)
                                {
                                    viewModel.SelectedFilePath = filePath;

                                    // Auto-fill document name if empty
                                    if (string.IsNullOrWhiteSpace(viewModel.DocumentName))
                                    {
                                        viewModel.DocumentName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                                        DocumentNameTextBox.Text = viewModel.DocumentName;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("File size exceeds the maximum limit of 25MB. Please select a smaller file.",
                                              "File Too Large", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Unsupported file format. Please select a PDF, JPG, PNG, DOC, or DOCX file.",
                                          "Invalid File Type", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing dropped file: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidFileType(string filePath)
        {
            var extension = System.IO.Path.GetExtension(filePath).ToLower();
            var validExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            return validExtensions.Contains(extension);
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SupportingDocumentsPageViewModel viewModel)
            {
                if (viewModel.UploadDocumentCommand.CanExecute(null))
                {
                    viewModel.UploadDocumentCommand.Execute(null);
                }
            }
        }
    }
}