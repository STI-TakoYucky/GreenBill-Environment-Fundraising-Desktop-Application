using GreenBill.Core;
using GreenBill.IServices;
using System;

namespace GreenBill.Services {
    public interface ITabNavigationService {
        ViewModel CurrentTabView { get; }

        void NavigateToTab<T>() where T : ViewModel;
        void NavigateToTab<T>(object parameter) where T : ViewModel;
    }

    public class TabNavigationService : ObservableObject, ITabNavigationService {
        private readonly Func<Type, ViewModel> _factory;

        /// <summary>
        /// Pass a factory (e.g. from DI container) that knows how to create your ViewModels.
        /// </summary>
        public TabNavigationService(Func<Type, ViewModel> factory) {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        private ViewModel _currentTabView;
        public ViewModel CurrentTabView {
            get => _currentTabView;
            set {
                _currentTabView = value;
                OnPropertyChanged();
            }
        }

        public void NavigateToTab<T>() where T : ViewModel {
            var viewModel = (T)_factory(typeof(T));
            CurrentTabView = viewModel;
        }

        public void NavigateToTab<T>(object parameter) where T : ViewModel {
            var viewModel = (T)_factory(typeof(T));

            if (viewModel is INavigatableService navigatableVm) {
                navigatableVm.ApplyNavigationParameter(parameter);
            }

            CurrentTabView = viewModel;
        }
    }
}
