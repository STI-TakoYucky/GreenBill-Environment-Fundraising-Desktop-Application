using GreenBill.Core;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class FundraisingStepsViewModel : Core.ViewModel
    {
        private INavigationService _navigationService;
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

        public ICommand GoToStep2 { get; set; }
        public ICommand GoToStep3 { get; set; }
        public ICommand GoToStep4 { get; set; }
        public ICommand GoToStep5 { get; set; }
        public ICommand GoToPreviousStep { get; set; }

        public FundraisingStepsViewModel()
        {
            InitializeCommands();
        }

        public FundraisingStepsViewModel(INavigationService navService)
        {
            Navigation = navService;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            GoToStep2 = new RelayCommand(o => CurrentStep = 2);
            GoToStep3 = new RelayCommand(o => CurrentStep = 3);
            GoToStep4 = new RelayCommand(o => CurrentStep = 4);
            GoToStep5 = new RelayCommand(o => CurrentStep = 5);
            GoToPreviousStep = new RelayCommand(o => { if (CurrentStep > 1) CurrentStep--; });
        }
    }
}