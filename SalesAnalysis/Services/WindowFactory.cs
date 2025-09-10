using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace SalesAnalysis.Services
{
    public class WindowFactory : IWindowFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public WindowFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TWindow CreateWindow<TWindow, TViewModel>()
            where TWindow : Window
            where TViewModel : class
        {
            var window = _serviceProvider.GetRequiredService<TWindow>();
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            window.DataContext = viewModel;
            return window;
        }

        public TWindow CreateWindow<TWindow, TViewModel>(params object[] parameters)
            where TWindow : Window
            where TViewModel : class
        {
            var window = _serviceProvider.GetRequiredService<TWindow>();

            // создаём VM вручную, передаём параметры
            var viewModel = ActivatorUtilities.CreateInstance<TViewModel>(_serviceProvider, parameters);

            window.DataContext = viewModel;
            return window;
        }
    }
}
