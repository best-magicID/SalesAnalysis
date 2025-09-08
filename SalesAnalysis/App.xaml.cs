using SalesAnalysis.ViewModels;
using System.Windows;
using SalesAnalysis.Views;
using Microsoft.Extensions.DependencyInjection;
using SalesAnalysis.Services;

namespace SalesAnalysis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;


        public App()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IWorkingWithExcel, WorkingWithExcel>();
            services.AddSingleton<MainWindowView>();
            services.AddSingleton<MainWindowViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //MainWindowView mainWindowView = new MainWindowView()
            //{
            //    DataContext = new MainWindowViewModel()
            //};

            MainWindowView mainWindowView = _serviceProvider.GetRequiredService<MainWindowView>();
            mainWindowView.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();

            mainWindowView.Show();
        }

    }
}
