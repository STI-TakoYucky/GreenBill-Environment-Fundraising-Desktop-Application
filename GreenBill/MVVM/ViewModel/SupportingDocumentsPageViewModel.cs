using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.Services;
using GreenBill.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using System.Collections.ObjectModel;
using MongoDB.Bson;
using System.Diagnostics;
using MongoDB.Bson.IO;

namespace GreenBill.MVVM.ViewModel
{
    public class SupportingDocumentsPageViewModel : Core.ViewModel, INavigationAware, INavigatableService
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _showMessage;
        public bool ShowMessage
        {
            get => _showMessage;
            set
            {
                _showMessage = true;
                OnPropertyChanged();
            }
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                OnPropertyChanged();
            }
        }
        public bool ShowNavigation => false;

        private INavigationService _navigationService;
        private readonly ISupportingDocumentService _supportingDocumentService;
        private ObjectId CampaignId { get; set; }

        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        // Properties for document upload
        private string _selectedDocumentType = "Government ID";
        public string SelectedDocumentType
        {
            get => _selectedDocumentType;
            set
            {
                _selectedDocumentType = value;
                OnPropertyChanged();
                UpdateUploadButtonState();
            }
        }

        private string _documentName = string.Empty;
        public string DocumentName
        {
            get => _documentName;
            set
            {
                _documentName = value;
                OnPropertyChanged();
                UpdateUploadButtonState();
            }
        }

        private string _selectedFilePath = string.Empty;
        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedFile));
                OnPropertyChanged(nameof(SelectedFileName));
                UpdateUploadButtonState();
            }
        }

        public bool HasSelectedFile => !string.IsNullOrEmpty(_selectedFilePath);
        public string SelectedFileName => HasSelectedFile ? Path.GetFileName(_selectedFilePath) : string.Empty;

        private bool _canUpload = false;
        public bool CanUpload
        {
            get => _canUpload;
            set
            {
                _canUpload = value;
                OnPropertyChanged();
            }
        }

        private bool _isUploading = false;
        public bool IsUploading
        {
            get => _isUploading;
            set
            {
                _isUploading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUploadingVisibility));
            }
        }

        public Visibility IsUploadingVisibility => IsUploading ? Visibility.Visible : Visibility.Collapsed;

        private int _uploadProgress = 0;
        public int UploadProgress
        {
            get => _uploadProgress;
            set
            {
                _uploadProgress = value;
                OnPropertyChanged();
            }
        }

        // Documents collection for binding
        private ObservableCollection<SupportingDocument> _documents;
        public ObservableCollection<SupportingDocument> Documents
        {
            get => _documents;
            set
            {
                _documents = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DocumentsOverviewText));
            }
        }

        // Document types for ComboBox
        public ObservableCollection<string> DocumentTypes { get; set; }

        // Computed property for documents overview text
        public string DocumentsOverviewText
        {
            get
            {
                if (Documents == null || !Documents.Any())
                    return "No documents uploaded yet";

                var total = Documents.Count;
                var verified = Documents.Count(d => d.Status == "Verified" || d.Status == "Approved");
                var underReview = Documents.Count(d => d.Status == "Pending" || d.Status == "Under Review");
                var requiresAttention = Documents.Count(d => d.Status == "Rejected" || d.Status == "Requires Attention");

                return $"{total} documents uploaded • {verified} verified • {underReview} under review • {requiresAttention} requires attention";
            }
        }

        // Commands
        public ICommand GoBackCommand { get; set; }
        public ICommand BrowseFileCommand { get; set; }
        public ICommand UploadDocumentCommand { get; set; }
        public ICommand DeleteDocumentCommand { get; set; }
        public ICommand RefreshDocumentsCommand { get; set; }

        public SupportingDocumentsPageViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            _supportingDocumentService = new SupportingDocumentService();
            Documents = new ObservableCollection<SupportingDocument>();

            InitializeCommands();
            InitializeDocumentTypes();
        }

        private void InitializeCommands()
        {
            GoBackCommand = new RelayCommand(o => Navigation.NavigateBack(), o => Navigation.CanNavigateBack);
            BrowseFileCommand = new RelayCommand(async o => await BrowseFileAsync());
            UploadDocumentCommand = new RelayCommand(async o => await UploadDocumentAsync(), o => CanUpload && !IsUploading);
            DeleteDocumentCommand = new RelayCommand(async o => await DeleteDocumentAsync(o as SupportingDocument));
            RefreshDocumentsCommand = new RelayCommand(async o => await LoadDocumentsAsync());
        }

        private void InitializeDocumentTypes()
        {
            DocumentTypes = new ObservableCollection<string>
            {
                "Government ID",
                "Reference Letters",
                "Campaign Evidence",
                "Legal Documents",
                "Other"
            };
        }

        private async Task LoadDocumentsAsync()
        {
            try
            {
                var documents = await _supportingDocumentService.GetByCampaignIdAsync(CampaignId.ToString());

                Documents.Clear();
                foreach (var doc in documents)
                {
                    Documents.Add(doc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading documents: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteDocumentAsync(SupportingDocument document)
        {
            if (document == null) return;
            if(document.Status == "Approved")
            {
                MessageBox.Show("You cannot delete an approved document.");
                return;
            }
            var result = MessageBox.Show($"Are you sure you want to delete '{document.DocumentName}'?",
                                       "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);



            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true;
                try
                {
                    await _supportingDocumentService.DeleteAsync(document.Id);
                    Documents.Remove(document);

                    SuccessMessage = "Document Deleted Successfully!";
                    ShowMessage = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting document: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally 
                {
                    IsLoading = false;
                }
            }
        }

        private async Task BrowseFileAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select Document",
                    Filter = "All Supported Files|*.pdf;*.jpg;*.jpeg;*.png;*.doc;*.docx|" +
                            "PDF Files|*.pdf|" +
                            "Image Files|*.jpg;*.jpeg;*.png|" +
                            "Word Documents|*.doc;*.docx|" +
                            "All Files|*.*",
                    FilterIndex = 1,
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var fileInfo = new FileInfo(openFileDialog.FileName);

                    // Check file size (25MB limit)
                    const long maxFileSize = 25 * 1024 * 1024; // 25MB in bytes
                    if (fileInfo.Length > maxFileSize)
                    {
                        MessageBox.Show("File size exceeds the maximum limit of 25MB. Please select a smaller file.",
                                      "File Too Large", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    SelectedFilePath = openFileDialog.FileName;

                    // Auto-fill document name if empty
                    if (string.IsNullOrWhiteSpace(DocumentName))
                    {
                        DocumentName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting file: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UploadDocumentAsync()
        {
            if (!CanUpload || IsUploading)
                return;

            try
            {
                IsLoading = true;
                IsUploading = true;
                UploadProgress = 0;

                // Simulate upload progress
                for (int i = 0; i <= 30; i += 10)
                {
                    UploadProgress = i;
                    await Task.Delay(100);
                }

                // Read file data
                byte[] fileData = File.ReadAllBytes(SelectedFilePath);

                UploadProgress = 60;
                await Task.Delay(100);

                // Create document
                var document = new SupportingDocument
                {
                    FileName = Path.GetFileName(SelectedFilePath),
                    DocumentType = SelectedDocumentType,
                    DocumentName = DocumentName.Trim(),
                    FileData = fileData,
                    ContentType = GetContentType(SelectedFilePath),
                    FileSize = fileData.Length,
                    Status = "Pending",
                    CampaignId = this.CampaignId,
                    UploadDate = DateTime.UtcNow
                };

                UploadProgress = 80;
                await Task.Delay(100);

                // Save to database
                await _supportingDocumentService.Create(document);

                UploadProgress = 100;
                await Task.Delay(500);

                // Add to collection
                Documents.Add(document);

                // Reset form
                ResetUploadForm();

                SuccessMessage = "Document uploaded successfully! It will be reviewed by our team within 2-3 business days.";
                ShowMessage = true;
            }


            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading document: {ex.Message}", "Upload Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsUploading = false;
                IsLoading = false;
                UploadProgress = 0;
            }
        }

        private void ResetUploadForm()
        {
            SelectedFilePath = string.Empty;
            DocumentName = string.Empty;
            SelectedDocumentType = "Government ID";
        }

        private void UpdateUploadButtonState()
        {
            CanUpload = !string.IsNullOrWhiteSpace(DocumentName) &&
                       HasSelectedFile &&
                       !string.IsNullOrWhiteSpace(SelectedDocumentType);
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                default:
                    return "application/octet-stream";
            }
        }

        public async void ApplyNavigationParameter(object parameter)
        {
            Debug.WriteLine($"Here Parameter: ${parameter}");
            if (parameter == null) return;
            CampaignId = (ObjectId)parameter;
            Debug.WriteLine($"Campaign Id: {CampaignId.ToString()}");

            await LoadDocumentsAsync();
        }
    }
}