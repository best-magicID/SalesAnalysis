using SalesAnalysis.ViewModels;
using System.Windows;

namespace SalesAnalysis.Views
{
    /// <summary>
    /// Логика взаимодействия для WindowForAddNewModel.xaml
    /// </summary>
    public partial class WindowForAddNewModel_View : Window
    {
        #region Конструктор

        public WindowForAddNewModel_View()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is WindowForAddNewModel_ViewModel viewModel)
                {
                    viewModel.RequestClose += () => this.Close();
                }
            };
        }

        #endregion
    }
}
