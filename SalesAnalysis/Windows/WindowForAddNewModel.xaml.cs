using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SalesAnalysis.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowForAddNewModel.xaml
    /// </summary>
    public partial class WindowForAddNewModel : Window, INotifyPropertyChanged
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Название модели
        /// </summary>
        public string NameModel
        {
            get => _NameModel;
            set
            {
                _NameModel = value;
                OnPropertyChanged();
            }
        }
        private string _NameModel = string.Empty;

        /// <summary>
        /// Цена модели
        /// </summary>
        public string PriceModel 
        { 
            get => _PriceModel; 
            set
            {
                _PriceModel = value;
                OnPropertyChanged();
            }
        }
        public string _PriceModel = string.Empty;

        /// <summary>
        /// Флаг, сохранение данных
        /// </summary>
        public bool IsSave { get; set; } = false;

        #endregion

        #region Конструктор

        public WindowForAddNewModel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public WindowForAddNewModel(string nameModel, string priceModel)
        {
            InitializeComponent();

            this.NameModel = nameModel;
            this.PriceModel = priceModel;

            DataContext = this;
        }

        #endregion

        #region МЕТОДЫ

        private void ButtonForSaveModel_Click(object sender, RoutedEventArgs e)
        {
            IsSave = true;

            Close();
        }

        #region ОБНОВЛЕНИЕ UI

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }
}
