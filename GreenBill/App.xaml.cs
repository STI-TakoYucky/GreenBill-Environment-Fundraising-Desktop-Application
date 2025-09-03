using GreenBill.Core;
using GreenBill.MVVM.Model;
using GreenBill.MVVM.ViewModel;
using GreenBill.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GreenBill.IServices;

namespace GreenBill
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;
        private IUserSessionService _sessionService;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            // Register UI components
            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            });

            // Register ViewModels
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SigninViewModel>();
            services.AddTransient<HomePageViewModel>();
            services.AddSingleton<SignupViewModel>();
            services.AddSingleton<FundraisingDetailsViewModel>();
            services.AddSingleton<FundraisingStepsViewModel>();

            // Register Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ICampaignService, CampaignService>();
            services.AddSingleton<IUserService, UserService>();

            // Register Session Service
            services.AddSingleton<IUserSessionService>(UserSessionService.Instance);

            // Register ViewModel factory
            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

            serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize session service
            _sessionService = serviceProvider.GetRequiredService<IUserSessionService>();

            // Subscribe to session events
            _sessionService.UserLoggedIn += OnUserLoggedIn;
            _sessionService.UserLoggedOut += OnUserLoggedOut;

            // Check for saved session on startup (optional)
            CheckSavedSession();

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Clean up session on app exit
            _sessionService?.ClearSession();

            // Unsubscribe from events
            if (_sessionService != null)
            {
                _sessionService.UserLoggedIn -= OnUserLoggedIn;
                _sessionService.UserLoggedOut -= OnUserLoggedOut;
            }

            // Dispose service provider
            if (serviceProvider is IDisposable disposableServiceProvider)
            {
                disposableServiceProvider.Dispose();
            }

            base.OnExit(e);
        }

        private void OnUserLoggedIn(object sender, User user)
        {
            // Handle user login at application level
            // You can update global UI state, start background services, etc.
            Current.Properties["CurrentUserId"] = user.Id;
            Current.Properties["CurrentUserEmail"] = user.Email;

            // Optional: Log the login event
            System.Diagnostics.Debug.WriteLine($"User logged in: {user.Email}");
        }

        private void OnUserLoggedOut(object sender, EventArgs e)
        {
            // Handle user logout at application level
            Current.Properties.Remove("CurrentUserId");
            Current.Properties.Remove("CurrentUserEmail");

            // Navigate back to signin
            var navigationService = serviceProvider.GetRequiredService<INavigationService>();
            navigationService.NavigateTo<SigninViewModel>();

            // Update main window state
            var mainWindow = Current.MainWindow;
            if (mainWindow?.DataContext is MainWindowViewModel mainVM)
            {
                mainVM.ShowNavigation = false;
                mainVM.IsUserLoggedIn = false;
            }

            // Optional: Log the logout event
            System.Diagnostics.Debug.WriteLine("User logged out");
        }

        private void CheckSavedSession()
        {
            // Implement logic to restore session from persistent storage if needed
            // For example, if user chose "Remember me" option
            // This is where you'd check for saved session tokens or user preferences
        }
    }
}