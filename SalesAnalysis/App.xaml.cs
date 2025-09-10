using Microsoft.Extensions.DependencyInjection;
using SalesAnalysis.Data;
using SalesAnalysis.Services;
using SalesAnalysis.ViewModels;
using SalesAnalysis.Views;
using System.Windows;

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

            // DbContext и сервисы
            services.AddDbContext<MyDbContext>();
            services.AddScoped<IOperationsDb, OperationsDb>();

            services.AddSingleton<IWorkingWithExcel, WorkingWithExcel>();

            // View и ViewModel
            services.AddTransient<MainWindowView>();
            services.AddTransient<MainWindowViewModel>();

            // Фабрика окон
            services.AddSingleton<IWindowFactory, WindowFactory>();

            _serviceProvider = services.BuildServiceProvider();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var windowFactory = _serviceProvider.GetRequiredService<IWindowFactory>();

            MainWindowView mainWindowView = windowFactory.CreateWindow<MainWindowView, MainWindowViewModel>();

            mainWindowView.Show();
        }

    }
}
