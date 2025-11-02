using GreenBill.Core;
using GreenBill.IServices;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GreenBill.Services {
    public interface ITabNavigationService {
        ViewModel CurrentTabView { get; }

        void NavigateToTab<T>() where T : ViewModel;
        void NavigateToTab<T>(object parameter) where T : ViewModel;
    }

    public class TabNavigationService : ITabNavigationService, INotifyPropertyChanged {
        private readonly Func<Type, ViewModel> _viewModelFactory;

        public TabNavigationService(Func<Type, ViewModel> viewModelFactory) {
            _viewModelFactory = viewModelFactory;
        }

        private ViewModel _currentTabView;
        public ViewModel CurrentTabView {
            get => _currentTabView;
            private set {
                if (!Equals(_currentTabView, value)) {
                    _currentTabView = value;
                    OnPropertyChanged();
                }
            }
        }

        public void NavigateToTab<T>() where T : ViewModel {
            NavigateToTab<T>(null);
        }

        public void NavigateToTab<T>(object parameter) where T : ViewModel {
            // Resolve VM from DI (singleton/transient behavior is controlled by DI registration)
            var vm = (ViewModel)_viewModelFactory(typeof(T));
            if (vm == null) return;

            // If viewmodel supports navigation parameters, call it
            if (vm is INavigatableService navigatable) {
                try { navigatable.ApplyNavigationParameter(parameter); } catch { /* swallow */ }
            }

            // If the VM exposes a public Refresh() method, call it
            var refreshMethod = vm.GetType().GetMethod("Refresh", BindingFlags.Public | BindingFlags.Instance);
            try { refreshMethod?.Invoke(vm, null); } catch { /* swallow */ }

            // Set CurrentTabView so UI bound to this property updates
            CurrentTabView = vm;
        }

        // Simple INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
