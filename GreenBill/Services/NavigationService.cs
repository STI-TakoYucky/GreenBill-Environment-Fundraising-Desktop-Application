using GreenBill.Core;
using GreenBill.MVVM.ViewModel;
using System;
using System.Windows;
using GreenBill.IServices;

namespace GreenBill.Services
{
    public interface INavigationAware
    {
        bool ShowNavigation { get; }
    }

    public interface INavigationService
    {
        ViewModel CurrentView { get; }

        void NavigateTo<T>() where T : ViewModel;
        void NavigateTo<T>(object parameter) where T : ViewModel;
    }

    public class NavigationService : ObservableObject, INavigationService
    {
        private ViewModel _currentView;
        private readonly Func<Type, ViewModel> viewModelFactory;

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
            ViewModel viewModel = viewModelFactory.Invoke(typeof(TViewModel));
            ApplyNavigationSettings(viewModel);
            CurrentView = viewModel;
        }

        public void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModel
        {
            ViewModel viewModel = viewModelFactory.Invoke(typeof(TViewModel));

            if (viewModel is INavigatableService navigatableVm)
            {
                navigatableVm.ApplyNavigationParameter(parameter);
            }

            ApplyNavigationSettings(viewModel);
            CurrentView = viewModel;
        }
        private void ApplyNavigationSettings(ViewModel viewModel)
        {
            if (Application.Current.MainWindow?.DataContext is MainWindowViewModel mainVM &&
                viewModel is INavigationAware navigationAware)
            {
                mainVM.ShowNavigation = navigationAware.ShowNavigation;
            }
        }
    }
}
