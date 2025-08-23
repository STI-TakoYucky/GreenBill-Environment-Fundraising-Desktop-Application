using GreenBill.Core;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using GreenBill.MVVM.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MongoDB.Driver;

namespace GreenBill.MVVM.ViewModel
{
    public class CategoryItem : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class FundraisingStepsViewModel : Core.ViewModel
    {
        public ObservableCollection<CategoryItem> CategoryItems { get; set; }
        private INavigationService _navigationService;
        private Campaign _currentCampaign;
        private int _currentStep = 1;


        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                _currentStep = value;
                OnPropertyChanged();
            }
        }

        public Campaign CurrentCampaign
        {
            get => _currentCampaign;
            set
            {
                _currentCampaign = value;
                OnPropertyChanged();
            }
        }

        public string SelectedCountry
        {
            get => CurrentCampaign?.Country;
            set
            {
                if (CurrentCampaign != null)
                {
                    CurrentCampaign.Country = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ZipCode
        {
            get => CurrentCampaign?.ZipCode;
            set
            {
                if (CurrentCampaign != null)
                {
                    CurrentCampaign.ZipCode = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedCategory
        {
            get => CurrentCampaign?.Category;
            set
            {
                if (CurrentCampaign != null)
                {
                    CurrentCampaign.Category = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal DonationGoal
        {
            get => CurrentCampaign.DonationGoal;
            set
            {
                if (CurrentCampaign != null)
                {
                    CurrentCampaign.DonationGoal = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => CurrentCampaign.Title;
            set
            {
                if (CurrentCampaign != null)
                {
                    CurrentCampaign.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => CurrentCampaign.Description;
            set
            {
                if (CurrentCampaign != null)
                {
                    CurrentCampaign.Description = value;
                    OnPropertyChanged();
                }
            }
        }


        public byte[] Image
        {
            get => CurrentCampaign?.Image;
            set
            {
                if (CurrentCampaign != null)
                {
                    CurrentCampaign.Image = value;
                    OnPropertyChanged();
                }
            }
        }



        public ICommand GoToStep2 { get; set; }
        public ICommand GoToStep3 { get; set; }
        public ICommand GoToStep4 { get; set; }
        public ICommand GoToStep5 { get; set; }
        public ICommand GoToPreviousStep { get; set; }
        public ICommand GoToHome { get; set; }
        public ICommand SelectCategoryCommand { get; set; }
        public ICommand SaveCampaign {  get; set; }


        public ObservableCollection<string> Countries { get; set; }
        public ObservableCollection<string> Categories { get; set; }

        public FundraisingStepsViewModel()
        {
            InitializeCampaign();
            InitializeCollections();
            InitializeCommands();
        }



        public FundraisingStepsViewModel(INavigationService navService)
        {
            Navigation = navService;
            InitializeCampaign();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeCampaign()
        {
            CurrentCampaign = new Campaign();
            // Set default values
            CurrentCampaign.Country = "United States";
        }

        private void InitializeCollections()
        {
            Countries = new ObservableCollection<string>
            {
                "United States",
                "Canada",
                "United Kingdom",
                "Australia",
                "Germany",
                "France",
                "Other"
            };

            Categories = new ObservableCollection<string>
            {
                "Happening worldwide",
                "Local projects",
                "Emergency relief",
                "Medical fundraisers",
                "Environmental causes"
            };

            CategoryItems = new ObservableCollection<CategoryItem>
    {
        new CategoryItem { Name = "Happening worldwide" },
        new CategoryItem { Name = "Local projects" },
        new CategoryItem { Name = "Emergency relief" },
        new CategoryItem { Name = "Medical fundraisers" },
        new CategoryItem { Name = "Environmental causes" }
    };
        }

        private void InitializeCommands()
        {
            GoToStep2 = new RelayCommand(o =>
            {
                if (ValidateStep1())
                {
                    CurrentStep = 2;
                }
            });

            GoToStep3 = new RelayCommand(o => CurrentStep = 3);
            GoToStep4 = new RelayCommand(o => CurrentStep = 4);
            GoToStep5 = new RelayCommand(o => CurrentStep = 5);
            SaveCampaign = new RelayCommand(async o =>
            {
                await SaveCampaignAsync();
            }, o => CurrentCampaign != null);
            GoToHome = new RelayCommand(o =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                {
                    mainVM.ShowNavigation = true;
                }
                Navigation.NavigateTo<HomePageViewModel>();
            });

            GoToPreviousStep = new RelayCommand(o => { if (CurrentStep > 1) CurrentStep--; });

            SelectCategoryCommand = new RelayCommand(parameter =>
            {
                if (parameter is string categoryName)
                {
                    foreach (var item in CategoryItems)
                    {
                        item.IsSelected = false;
                    }

                    var selectedItem = CategoryItems.FirstOrDefault(c => c.Name == categoryName);
                    if (selectedItem != null)
                    {
                        selectedItem.IsSelected = true;
                        SelectedCategory = categoryName;
                    }
                }
            });
        }

        private bool ValidateStep1()
        {
            if (string.IsNullOrWhiteSpace(SelectedCountry))
            {
                MessageBox.Show("Please select a country.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(ZipCode))
            {
                MessageBox.Show("Please enter a zip code.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                MessageBox.Show("Please select a fundraising category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public Campaign GetCompletedCampaign()
        {
            return CurrentCampaign;
        }


        public async Task SaveCampaignAsync()
        {
            try
            {
                // Validate campaign data
                if (CurrentCampaign == null)
                {
                    MessageBox.Show("No campaign data to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate required fields
                if (!ValidateCampaignForSaving())
                {
                    return; // Validation messages are shown in the method
                }

                // Get connection string from config (you should move this to a service or config)
                string connectionString = "mongodb://localhost:27017"; // Consider moving to app.config

                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("GreenBill");
                var collection = database.GetCollection<Campaign>("Campaigns");

                // Set creation date if not already set
                if (CurrentCampaign.CreatedAt == default(DateTime))
                {
                    CurrentCampaign.CreatedAt = DateTime.UtcNow;
                }

                await collection.InsertOneAsync(CurrentCampaign);

                MessageBox.Show("Campaign saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Optionally navigate back to home or campaigns list
                Navigation?.NavigateTo<HomePageViewModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateCampaignForSaving()
        {
            if (string.IsNullOrWhiteSpace(CurrentCampaign.Title))
            {
                MessageBox.Show("Please enter a campaign title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(CurrentCampaign.Description))
            {
                MessageBox.Show("Please enter a campaign description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (CurrentCampaign.DonationGoal <= 0)
            {
                MessageBox.Show("Please enter a valid donation goal.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(CurrentCampaign.Country))
            {
                MessageBox.Show("Please select a country.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(CurrentCampaign.Category))
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Optional: Validate that an image is uploaded
            if (CurrentCampaign.Image == null || CurrentCampaign.Image.Length == 0)
            {
                MessageBox.Show("Please upload an image for your campaign.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}