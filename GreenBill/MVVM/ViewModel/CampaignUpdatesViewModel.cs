using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.Services;
using GreenBill.MVVM.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class CampaignUpdatesViewModel : Core.ViewModel, INavigationAware, INavigatableService
    {
        public bool ShowNavigation => false;
        private INavigationService _navigationService;
        private ObjectId CampaignId { get; set; }

        #region Properties
        private ObservableCollection<CampaignUpdate> _updates;
        public ObservableCollection<CampaignUpdate> Updates
        {
            get => _updates;
            set
            {
                _updates = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CampaignUpdate> _filteredUpdates;
        public ObservableCollection<CampaignUpdate> FilteredUpdates
        {
            get => _filteredUpdates;
            set
            {
                _filteredUpdates = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEmpty));
            }
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
                ApplySearch();
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormTitle));
                OnPropertyChanged(nameof(SaveButtonText));
                OnPropertyChanged(nameof(ShowCancelButton));
            }
        }

        private CampaignUpdate _editingUpdate;
        public CampaignUpdate EditingUpdate
        {
            get => _editingUpdate;
            set
            {
                _editingUpdate = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmpty => FilteredUpdates?.Count == 0;
        public string FormTitle => IsEditing ? "Edit Update" : "Add New Update";
        public string SaveButtonText => IsEditing ? "Save Changes" : "Add Update";
        public bool ShowCancelButton => IsEditing;

        public List<string> Categories { get; } = new List<string>
        {
            "General Update",
            "Milestone Reached",
            "Media Coverage",
            "Thank You",
            "Progress Report",
            "News & Announcements"
        };
        #endregion

        #region Commands
        public ICommand GoBackCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        #endregion

        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public CampaignUpdatesViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            InitializeData();
            InitializeCommands();
            LoadSampleData();
        }

        #region Initialization
        private void InitializeData()
        {
            Updates = new ObservableCollection<CampaignUpdate>();
            FilteredUpdates = new ObservableCollection<CampaignUpdate>();
            SelectedCategory = Categories.First();
        }

        private void InitializeCommands()
        {
            GoBackCommand = new RelayCommand(o =>
            {
                Navigation.NavigateBack();
                Debug.WriteLine("TEST BACK");
            }, o => Navigation.CanNavigateBack);

            SaveCommand = new RelayCommand(async o => await SaveUpdate(), o => CanSave());
            CancelCommand = new RelayCommand(o => ResetForm());
            EditCommand = new RelayCommand(o => StartEditing((Guid)o));
            DeleteCommand = new RelayCommand(async o => await DeleteUpdate((Guid)o));
        }

        private void LoadSampleData()
        {
            var sampleUpdates = new List<CampaignUpdate>
            {
                new CampaignUpdate
                {
                    Id = Guid.NewGuid(),
                    Title = "Campaign Launch Success!",
                    Description = "We're thrilled to announce the official launch of our green energy campaign! Thanks to early supporters, we've already reached 15% of our goal in the first week. Your contributions are making a real difference in promoting sustainable energy solutions.",
                    Category = "Milestone Reached",
                    Tags = "launch, milestone, thank you",
                    DateCreated = DateTime.Now.AddDays(-7)
                },
                new CampaignUpdate
                {
                    Id = Guid.NewGuid(),
                    Title = "Media Coverage Update",
                    Description = "Our campaign was featured in the local environmental magazine! This exposure has brought in new supporters and increased awareness about our cause. We're gaining momentum every day.",
                    Category = "Media Coverage",
                    Tags = "media, awareness, progress",
                    DateCreated = DateTime.Now.AddDays(-3)
                },
                new CampaignUpdate
                {
                    Id = Guid.NewGuid(),
                    Title = "50% Goal Reached!",
                    Description = "Incredible news! We've officially reached 50% of our funding goal! This milestone wouldn't have been possible without each and every one of our amazing supporters. We're halfway there!",
                    Category = "Milestone Reached",
                    Tags = "milestone, halfway, celebration",
                    DateCreated = DateTime.Now.AddDays(-1)
                }
            };

            foreach (var update in sampleUpdates)
            {
                Updates.Add(update);
                FilteredUpdates.Add(update);
            }
        }
        #endregion

        #region Command Implementations
        private async Task SaveUpdate()
        {
            var validationResult = ValidateForm();
            if (!validationResult.IsValid)
            {
                OnValidationFailed?.Invoke(validationResult.ErrorMessage);
                return;
            }

            if (IsEditing && EditingUpdate != null)
            {
                // Update existing
                EditingUpdate.Title = Title?.Trim();
                EditingUpdate.Description = Description?.Trim();
                EditingUpdate.Category = SelectedCategory;
                EditingUpdate.DateModified = DateTime.Now;

                OnSuccessMessage?.Invoke("Update modified successfully!");
            }
            else
            {
                // Create new
                var newUpdate = new CampaignUpdate
                {
                    Id = Guid.NewGuid(),
                    Title = Title?.Trim(),
                    Description = Description?.Trim(),
                    Category = SelectedCategory,
                    DateCreated = DateTime.Now
                };

                Updates.Insert(0, newUpdate);
                ApplySearch();
                OnSuccessMessage?.Invoke("Update added successfully!");
            }

            ResetForm();
        }

        private void StartEditing(Guid updateId)
        {
            var update = Updates.FirstOrDefault(u => u.Id == updateId);
            if (update != null)
            {
                IsEditing = true;
                EditingUpdate = update;

                Title = update.Title;
                Description = update.Description;
                SelectedCategory = update.Category;

                OnStartEditing?.Invoke();
            }
        }

        private async Task DeleteUpdate(Guid updateId)
        {
            var confirmResult = await OnConfirmDelete?.Invoke("Are you sure you want to delete this update? This action cannot be undone.");
            if (confirmResult != true) return;

            var update = Updates.FirstOrDefault(u => u.Id == updateId);
            if (update != null)
            {
                Updates.Remove(update);
                ApplySearch();
                OnSuccessMessage?.Invoke("Update deleted successfully!");

                // If we were editing this update, reset the form
                if (IsEditing && EditingUpdate?.Id == updateId)
                {
                    ResetForm();
                }
            }
        }

        private void ResetForm()
        {
            IsEditing = false;
            EditingUpdate = null;
            Title = string.Empty;
            Description = string.Empty;
            SelectedCategory = Categories.First();
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Description);
        }
        #endregion

        #region Validation and Search
        private (bool IsValid, string ErrorMessage) ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Title))
                errors.Add("Title is required.");

            if (string.IsNullOrWhiteSpace(Description))
                errors.Add("Description is required.");

            if (Title?.Trim().Length > 200)
                errors.Add("Title must be 200 characters or less.");

            if (Description?.Trim().Length > 2000)
                errors.Add("Description must be 2000 characters or less.");

            if (errors.Any())
                return (false, string.Join("\n", errors));

            return (true, string.Empty);
        }

        private void ApplySearch()
        {
            FilteredUpdates.Clear();

            var searchTerm = SearchTerm?.Trim().ToLower() ?? "";
            var filteredItems = string.IsNullOrEmpty(searchTerm)
                ? Updates
                : Updates.Where(u =>
                    u.Title.ToLower().Contains(searchTerm) ||
                    u.Description.ToLower().Contains(searchTerm) ||
                    u.Category.ToLower().Contains(searchTerm) ||
                    u.Tags.ToLower().Contains(searchTerm));

            foreach (var item in filteredItems.OrderByDescending(u => u.DateCreated))
            {
                FilteredUpdates.Add(item);
            }
        }
        #endregion

        #region Events for View Communication
        public event Action<string> OnSuccessMessage;
        public event Action<string> OnValidationFailed;
        public event Func<string, Task<bool>> OnConfirmDelete;
        public event Action OnStartEditing;
        #endregion

        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;
            CampaignId = (ObjectId)parameter;
        }
    }
}