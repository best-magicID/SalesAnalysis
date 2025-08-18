using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace SalesAnalysis.Classes
{
    /// <summary>
    /// Описание класса Дата продаж
    /// </summary>
    internal class DateSale : INotifyPropertyChanged
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Id даты продажи
        /// </summary>
        [Key]
        public int IdDateSale
        {
            get => _IdDateSale;
            set
            {
                _IdDateSale = value;
                OnPropertyChanged();
            }
        }
        private int _IdDateSale;

        /// <summary>
        /// Дата продажи модели 
        /// </summary>
        public DateTime DateSaleModel
        {
            get => _DateSaleModel;
            set
            {
                _DateSaleModel = value;
                OnPropertyChanged();
            }
        }
        private DateTime _DateSaleModel;

        /// <summary>
        /// Id модели
        /// </summary>
        public int IdModel
        {
            get => _IdModel;
            set
            {
                _IdModel = value;
                OnPropertyChanged();
            }
        }
        private int _IdModel;

        /// <summary>
        /// Количество проданных моделей 
        /// </summary>
        public int CountSoldModels
        {
            get => _CountSoldModels;
            set
            {
                _CountSoldModels = value;
                OnPropertyChanged();
            }
        }
        private int _CountSoldModels;

        #endregion

        #region ОБНОВЛЕНИЕ UI

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
