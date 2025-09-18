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
using System.Diagnostics;

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

            services.AddSingleton<AdminWindow>(provider => new AdminWindow
            {
                DataContext = provider.GetRequiredService<AdminWindowViewModel>()
            });

   

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SigninViewModel>();
            services.AddTransient<HomePageViewModel>();
            services.AddSingleton<SignupViewModel>();
            services.AddSingleton<FundraisingDetailsViewModel>();
            services.AddTransient<FundraisingStepsViewModel>();
            services.AddTransient<UserCampaignsViewModel>();
            services.AddSingleton<CampaignAnalyticsViewModel>();
            services.AddSingleton<CampaignDetailsViewModel>();
            services.AddSingleton<AdminDashboardViewModel>();
            services.AddSingleton<AdminWindowViewModel>();
            services.AddSingleton<CampaignsViewModel>();
            services.AddSingleton<UserAnalyticsViewModel>();
            services.AddSingleton<AdminCampaignAnalyticsViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddTransient<SupportingDocumentsPageViewModel>();
            services.AddTransient<DonationPageViewModel>();

            services.AddSingleton<ReviewCampaignViewModel>();

            services.AddTransient<CampaignUpdatesViewModel>();


            services.AddSingleton<INavigationService, NavigationService>();


            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ICampaignService, CampaignService>();
            services.AddSingleton<ISupportingDocumentService, SupportingDocumentService>();
            services.AddSingleton<ICampaignUpdateService, CampaignUpdateService>();

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

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
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