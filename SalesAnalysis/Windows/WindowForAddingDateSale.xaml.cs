using SalesAnalysis.Classes;
using System.Collections.ObjectModel;
using System.Windows;

namespace SalesAnalysis.Windows
{
    /// <summary>
    /// Окно для Добавления новой продажи
    /// </summary>
    public partial class WindowForAddingDateSale : Window
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Выбранная модель
        /// </summary>
        public Model? SelectedModel { get; set; }

        /// <summary>
        /// Новая дата продажи
        /// </summary>
        public DateSale NewDateSale { get; set; } = new DateSale();

        /// <summary>
        /// Список всех моделей
        /// </summary>
        public ObservableCollection<Model> ListModels { get; set; } = new ObservableCollection<Model>();

        /// <summary>
        /// Количество проданных моделей (Не стал ставить условие от 0, вдруг надо будет делать возврат
        /// </summary>
        public int CountModels { get; set; }

        /// <summary>
        /// Новая дата продажи
        /// </summary>
        public DateTime NewDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Флаг, Сохранить 
        /// </summary>
        public bool IsSave { get; set; } = false;

        #endregion

        #region КОНСТУРКТОР

        public WindowForAddingDateSale(List<Model> newListModels)
        {
            InitializeComponent();

            foreach (var model in newListModels)
            {
                ListModels.Add(model);
            }

            DataContext = this;
        }

        #endregion

        #region МЕТОДЫ

        /// <summary>
        /// Запись новых данных о продаже за день
        /// </summary>
        public bool SetDateSale()
        {
            if (SelectedModel != null)
            {
                NewDateSale.IdModel = SelectedModel.IdModel;
                NewDateSale.DateSaleModel = NewDate;
                NewDateSale.CountSoldModels = CountModels;

                IsSave = true;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Кнопка сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonForSaveDateSale_Click(object sender, RoutedEventArgs e)
        {
            if(SetDateSale())
            {
                Close();
            }
            else
            {
                GeneralMethods.ShowNotification("Выберите модель");
            }

        }

        #endregion
    }
}
