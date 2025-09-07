using SalesAnalysis.ViewModels;
using System.Windows;
using SalesAnalysis.Views;

namespace SalesAnalysis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindowView mainWindowView = new MainWindowView()
            {
                DataContext = new MainWindowViewModel()
            };

            mainWindowView.Show();
        }

    }
}
