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
using GreenBill.Validators;
using System.Diagnostics;
using System.Collections;
using GreenBill.IServices;

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

    public class FundraisingStepsViewModel : Core.ViewModel, INavigationAware
    {
        public bool ShowNavigation => false;
        private ICampaignService _campaignService;
        private Dictionary<String, String> _errorsList; 

        public string ZipCodeError
        {
            get => _errorsList?.ContainsKey("ZipCode") == true ? _errorsList["ZipCode"] : null;
        }

        public string CategoryError
        {
            get => _errorsList?.ContainsKey("Category") == true ? _errorsList["Category"] : null;
        }

        public string DonationGoalError
        {
            get => _errorsList?.ContainsKey("DonationGoal") == true ? _errorsList["DonationGoal"] : null;
        }

        public string ImageError
        {
            get => _errorsList?.ContainsKey("Image") == true ? _errorsList["Image"] : null;
        }

        public string TitleError
        {
            get => _errorsList?.ContainsKey("Title") == true ? _errorsList["Title"] : null;
        }

        public string DescriptionError
        {
            get => _errorsList?.ContainsKey("Description") == true ? _errorsList["Description"] : null;
        }
        private INavigationService _navigationService;
        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        private int _currentStep = 1;
        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                _currentStep = value;
                OnPropertyChanged();
            }
        }

        private Campaign _currentCampaign;
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

                    ValidateField(nameof(ZipCodeError));
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
                    ValidateField(nameof(CategoryError));
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

                    ValidateField(nameof(DonationGoalError));
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
                    ValidateField(nameof(TitleError));
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
                    ValidateField(nameof(DescriptionError));
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
                    ValidateField(nameof(ImageError));
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
        public ICommand SaveCampaign { get; set; }

        public ObservableCollection<CategoryItem> CategoryItems { get; set; }
        public ObservableCollection<string> Countries { get; set; }
        public ObservableCollection<string> Categories { get; set; }

        public FundraisingStepsViewModel(INavigationService navService, ICampaignService campaignService)
        {
            Navigation = navService;
            _campaignService = campaignService;
            InitializeCampaign();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeCampaign()
        {
            CurrentCampaign = new Campaign();
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

            _errorsList = new Dictionary<string, string>();
        }

        private void InitializeCommands()
        {
            GoToStep2 = new RelayCommand(o =>
            {
                if (ValidateStep1()) CurrentStep = 2;
            });

            GoToStep3 = new RelayCommand(o =>
            {
                if (ValidateStep2()) CurrentStep = 3;
            });

            GoToStep4 = new RelayCommand(o =>
            {
                if(ValidateStep3()) CurrentStep = 4;
            });
            GoToStep5 = new RelayCommand(o =>
            {
                if(ValidateStep4()) CurrentStep = 5;
            });

            SaveCampaign = new RelayCommand(o => SaveCampaignAsync());
            GoToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());
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

  

        private void ValidateField(string errorPropertyName)
        {
            _errorsList.Clear();
            var validationErrors = CampaignValidator.Validate(CurrentCampaign);

            foreach (var error in validationErrors)
            {
                _errorsList[error.Key] = error.Value;
            }

            OnPropertyChanged(errorPropertyName);
        }


        private bool ValidateStep1()
        {
            _errorsList.Clear();
            _errorsList = CampaignValidator.Validate(CurrentCampaign, new ArrayList { "Category", "ZipCode" });

            OnPropertyChanged(nameof(ZipCodeError));
            OnPropertyChanged(nameof(CategoryError));

            return !_errorsList.ToList().Any();
        }

        private bool ValidateStep2()
        {
            _errorsList.Clear();
            _errorsList = CampaignValidator.Validate(CurrentCampaign, new ArrayList { "DonationGoal" });

            OnPropertyChanged(nameof(DonationGoalError));

            return !_errorsList.ToList().Any();
        }

        private bool ValidateStep3()
        {
            _errorsList.Clear();
            _errorsList = CampaignValidator.Validate(CurrentCampaign, new ArrayList { "Image" });

            OnPropertyChanged(nameof(ImageError));

            return !_errorsList.ToList().Any();
        }

        private bool ValidateStep4()
        {
            _errorsList.Clear();
            _errorsList = CampaignValidator.Validate(CurrentCampaign, new ArrayList { "Title", "Description" });

            OnPropertyChanged(nameof(TitleError));
            OnPropertyChanged(nameof(DescriptionError));

            return !_errorsList.ToList().Any();
        }

        public Campaign GetCompletedCampaign()
        {
            return CurrentCampaign;
        }

        public void SaveCampaignAsync()
        {
            try
            {
                if (ValidateCampaignForSaving()) return;
                _campaignService.Create(CurrentCampaign);
                MessageBox.Show("Campaign saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Navigation.NavigateTo<HomePageViewModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateCampaignForSaving()
        {
            _errorsList.Clear();
            _errorsList = CampaignValidator.Validate(CurrentCampaign);
            return _errorsList.ToList().Any();
        }
    }
}