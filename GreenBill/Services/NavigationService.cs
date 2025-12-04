using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.ViewModel;
using GreenBill.MVVM.ViewModel.Admin;
using System;
using System.Collections.Generic;
using System.Windows;

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

            if (_currentView != null)
            {
                _navigationHistory.Push(new NavigationEntry
                {
                    ViewModelType = _currentView.GetType(),
                    Parameter = GetCurrentViewParameter()
                });
            }

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

        private void ApplyNavigationSettings(ViewModel viewModel) {
            if (Application.Current.Windows.Count > 0) {
                foreach (Window window in Application.Current.Windows) {
                    if (window.DataContext is AdminWindowViewModel adminVM &&
                        viewModel is INavigationAware navigationAwareAdmin) {
                        adminVM.ShowNavigation = navigationAwareAdmin.ShowNavigation;
                        return;
                    }
                }
            }

            if (Application.Current.MainWindow?.DataContext is MainWindowViewModel mainVM &&
                viewModel is INavigationAware navigationAware) {
                mainVM.ShowNavigation = navigationAware.ShowNavigation;
            }
        }


        private object GetCurrentViewParameter()
        {

            if (_currentView is FundraisingDetailsViewModel fundraisingVm)
            {
                return fundraisingVm.CampaignId;
            }

            return null;
        }
    }
    internal class NavigationEntry
    {
        public Type ViewModelType { get; set; }
        public object Parameter { get; set; }
    }
}