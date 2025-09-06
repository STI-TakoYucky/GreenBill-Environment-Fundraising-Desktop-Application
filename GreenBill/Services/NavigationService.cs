using GreenBill.Core;
using GreenBill.MVVM.ViewModel;
using System;
using System.Windows;
using GreenBill.IServices;
using System.Collections.Generic;

namespace GreenBill.Services
{
    public interface INavigationAware
    {
        bool ShowNavigation { get; }
    }

    public interface INavigationService
    {
        ViewModel CurrentView { get; }
        bool CanNavigateBack { get; }
        void NavigateBack();
        void NavigateTo<T>() where T : ViewModel;
        void NavigateTo<T>(object parameter) where T : ViewModel;
        void ClearNavigationHistory();
    }

    public class NavigationService : ObservableObject, INavigationService
    {
        private ViewModel _currentView;
        private readonly Func<Type, ViewModel> viewModelFactory;
        private Stack<NavigationEntry> _navigationHistory = new Stack<NavigationEntry>();

        public bool CanNavigateBack => _navigationHistory.Count > 0;

        public ViewModel CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            this.viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModel
        {
            NavigateTo<TViewModel>(null);
        }

        public void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModel
        {
            // Store current view in history before navigating (if there is one)
            if (_currentView != null)
            {
                _navigationHistory.Push(new NavigationEntry
                {
                    ViewModelType = _currentView.GetType(),
                    Parameter = GetCurrentViewParameter()
                });
            }

            // Create and navigate to new view
            ViewModel viewModel = viewModelFactory.Invoke(typeof(TViewModel));

            if (viewModel is INavigatableService navigatableVm && parameter != null)
            {
                navigatableVm.ApplyNavigationParameter(parameter);
            }

            ApplyNavigationSettings(viewModel);
            CurrentView = viewModel;
        }

        public void NavigateBack()
        {
            if (!CanNavigateBack) return;

            var previousEntry = _navigationHistory.Pop();

            // Create the previous view model without adding to history
            ViewModel viewModel = viewModelFactory.Invoke(previousEntry.ViewModelType);

            if (viewModel is INavigatableService navigatableVm && previousEntry.Parameter != null)
            {
                navigatableVm.ApplyNavigationParameter(previousEntry.Parameter);
            }

            ApplyNavigationSettings(viewModel);
            CurrentView = viewModel;
        }

        public void ClearNavigationHistory()
        {
            _navigationHistory.Clear();
        }

        private void ApplyNavigationSettings(ViewModel viewModel)
        {
            if (Application.Current.MainWindow?.DataContext is MainWindowViewModel mainVM &&
                viewModel is INavigationAware navigationAware)
            {
                mainVM.ShowNavigation = navigationAware.ShowNavigation;
            }
        }

        private object GetCurrentViewParameter()
        {
            // This method can be extended to extract relevant parameters from current view
            // For now, we'll return null, but you can implement logic to preserve state
            // For example, if current view is FundraisingDetailsViewModel, return the CampaignId

            if (_currentView is FundraisingDetailsViewModel fundraisingVm)
            {
                return fundraisingVm.CampaignId;
            }

            // Add more cases as needed for other ViewModels that have parameters
            return null;
        }
    }

    // Helper class to store navigation history entries
    internal class NavigationEntry
    {
        public Type ViewModelType { get; set; }
        public object Parameter { get; set; }
    }
}