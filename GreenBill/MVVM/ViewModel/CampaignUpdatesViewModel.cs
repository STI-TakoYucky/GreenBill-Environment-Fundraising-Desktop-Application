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
        private readonly ICampaignUpdateService _campaignUpdateService;
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
            _campaignUpdateService = new CampaignUpdateService();
            InitializeData();
            InitializeCommands();

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
            EditCommand = new RelayCommand(o => StartEditing((ObjectId)o));
            DeleteCommand = new RelayCommand(async o => await DeleteUpdate((ObjectId)o));
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

                EditingUpdate.Title = Title?.Trim();
                EditingUpdate.Description = Description?.Trim();
                EditingUpdate.Category = SelectedCategory;
                EditingUpdate.UpdatedAt = DateTime.Now;

                OnSuccessMessage?.Invoke("Update modified successfully!");
            }
            else
            {
                var newUpdate = new CampaignUpdate
                {
                    Title = Title?.Trim(),
                    CampaignId = this.CampaignId,
                    Description = Description?.Trim(),
                    Category = SelectedCategory,
                    CreatedAt = DateTime.Now
                };

                await _campaignUpdateService.Create(newUpdate);
                Updates.Add(newUpdate);
                ApplySearch();
                OnSuccessMessage?.Invoke("Update added successfully!");
            }

            ResetForm();
        }

        public async Task LoadUpdatesAsync()
        {
            var updates = await _campaignUpdateService.GetByCampaignIdAsync(CampaignId);
            foreach (var update in updates)
            {
                Updates.Add(update);
                FilteredUpdates.Add(update);
            }
        }

        private void StartEditing(ObjectId updateId)
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

        private async Task DeleteUpdate(ObjectId updateId)
        {
            Debug.WriteLine("TEST 1");
            var confirmResult = await OnConfirmDelete?.Invoke("Are you sure you want to delete this update? This action cannot be undone.");
            if (confirmResult != true) return;
            Debug.WriteLine("TEST 2");

            var update = Updates.FirstOrDefault(u => u.Id == updateId);
            if (update != null)
            {
                Updates.Remove(update);
                ApplySearch();
                await _campaignUpdateService.DeleteAsync(updateId);
                OnSuccessMessage?.Invoke("Update deleted successfully!");

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
                    u.Category.ToLower().Contains(searchTerm)); 

            foreach (var item in filteredItems.OrderByDescending(u => u.CreatedAt))
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
            await LoadUpdatesAsync();
        }
    }
}