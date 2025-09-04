using GreenBill.Core;
using GreenBill.IServices;

namespace GreenBill.Services
{
    public interface ITabNavigationService
    {
        ViewModel CurrentTabView { get; }
        void NavigateToTab<T>() where T : ViewModel, new();
        void NavigateToTab<T>(object parameter) where T : ViewModel, new();
    }

    public class TabNavigationService : ObservableObject, ITabNavigationService
    {
        private ViewModel _currentTabView;

        public ViewModel CurrentTabView
        {
            get => _currentTabView;
            set
            {
                _currentTabView = value;
                OnPropertyChanged();
            }
        }

        public void NavigateToTab<TViewModel>() where TViewModel : ViewModel, new()
        {
            ViewModel viewModel = new TViewModel();
            CurrentTabView = viewModel;
        }

        public void NavigateToTab<TViewModel>(object parameter) where TViewModel : ViewModel, new()
        {
            ViewModel viewModel = new TViewModel();
            if (viewModel is INavigatableService navigatableVm)
            {
                navigatableVm.ApplyNavigationParameter(parameter);
            }
            CurrentTabView = viewModel;
        }
    }
}