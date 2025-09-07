using SalesAnalysis.ViewModels;
using System.Windows;

namespace SalesAnalysis.Views
{
    /// <summary>
    /// Окно для Добавления новой продажи
    /// </summary>
    public partial class WindowForAddingDateSaleView : Window
    {

        #region КОНСТУРКТОР

        public WindowForAddingDateSaleView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is WindowForAddingDateSaleViewModel viewModel)
                {
                    viewModel.RequestClose += () => this.Close();
                }
            };
        }

        #endregion
    }
}
