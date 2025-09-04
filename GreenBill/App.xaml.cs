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
using GreenBill.MVVM.View.Admin;
using GreenBill.MVVM.ViewModel.Admin;

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

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            });

            services.AddSingleton<AdminWindow>(provider => new AdminWindow {
                DataContext = provider.GetRequiredService<AdminWindowViewModel>()
            });

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SigninViewModel>();
            services.AddTransient<HomePageViewModel>();
            services.AddSingleton<SignupViewModel>();
            services.AddSingleton<FundraisingDetailsViewModel>();
            services.AddSingleton<FundraisingStepsViewModel>();
            services.AddSingleton<UserCampaignsViewModel>();
            services.AddSingleton<CampaignAnalyticsViewModel>();
            services.AddSingleton<CampaignDetailsViewModel>();
            services.AddSingleton<AdminDashboardViewModel>();
            services.AddSingleton<AdminWindowViewModel>();

            services.AddSingleton<INavigationService, NavigationService>();

 
            services.AddSingleton<ICampaignService, CampaignService>();
            services.AddSingleton<IUserService, UserService>();

            services.AddTransient<ITabNavigationService, TabNavigationService>();

            services.AddSingleton<IUserSessionService>(UserSessionService.Instance);
 
            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

            serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _sessionService = serviceProvider.GetRequiredService<IUserSessionService>();
            _sessionService.UserLoggedIn += OnUserLoggedIn;
            _sessionService.UserLoggedOut += OnUserLoggedOut;

            CheckSavedSession();

            var mainWindow = serviceProvider.GetRequiredService<AdminWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _sessionService?.ClearSession();

            if (_sessionService != null)
            {
                _sessionService.UserLoggedIn -= OnUserLoggedIn;
                _sessionService.UserLoggedOut -= OnUserLoggedOut;
            }

            if (serviceProvider is IDisposable disposableServiceProvider)
            {
                disposableServiceProvider.Dispose();
            }

            base.OnExit(e);
        }

        private void OnUserLoggedIn(object sender, User user)
        {
            Current.Properties["CurrentUserId"] = user.Id;
            Current.Properties["CurrentUserEmail"] = user.Email;

            System.Diagnostics.Debug.WriteLine($"User logged in: {user.Email}");
        }

        private void OnUserLoggedOut(object sender, EventArgs e)
        {
            Current.Properties.Remove("CurrentUserId");
            Current.Properties.Remove("CurrentUserEmail");

            var navigationService = serviceProvider.GetRequiredService<INavigationService>();
            navigationService.NavigateTo<SigninViewModel>();

            var mainWindow = Current.MainWindow;
            if (mainWindow?.DataContext is MainWindowViewModel mainVM)
            {
                mainVM.ShowNavigation = false;
                mainVM.IsUserLoggedIn = false;
            }

            System.Diagnostics.Debug.WriteLine("User logged out");
        }

        private void CheckSavedSession()
        {

        }
    }
}