using SalesAnalysis.Helpers;
using SalesAnalysis.Models;
using System.Collections.ObjectModel;

namespace SalesAnalysis.ViewModels
{
    public class WindowForAddingDateSaleViewModel : BaseViewModel
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

        public event Action? RequestClose;

        public RaiseCommand? SaveDateSaleCommand { get; set; }

        #endregion


        #region КОНСТУРКТОР

        public WindowForAddingDateSaleViewModel()
        {

        }

        public WindowForAddingDateSaleViewModel(List<Model> newListModels)
        {
            newListModels.ForEach(x => ListModels.Add(x));

            SaveDateSaleCommand = new RaiseCommand(SaveDateSaleCommand_Execute);
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
        /// Выполнить команду, "Сохранить дату продажи"
        /// </summary>
        private void SaveDateSaleCommand_Execute(object parameter)
        {
            if (SetDateSale())
            {
                OnClose();
                return;
            }
            else
            {
                GeneralMethods.ShowNotification("Выберите модель");
            }
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        public void OnClose()
        {
            RequestClose?.Invoke();
        }

        #endregion
    }
}
