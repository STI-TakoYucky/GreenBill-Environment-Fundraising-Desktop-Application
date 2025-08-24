using GreenBill.Core;
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

namespace GreenBill
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
           private readonly IServiceProvider serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            });

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SigninViewModel>();
            services.AddTransient<HomePageViewModel>();
            services.AddSingleton<SignupViewModel>();
            services.AddSingleton<FundraisingDetailsViewModel>();
            services.AddSingleton<FundraisingStepsViewModel>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ICampaignService, CampaignService>();
            services.AddSingleton<IUserService, UserService>();

            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

            serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}
