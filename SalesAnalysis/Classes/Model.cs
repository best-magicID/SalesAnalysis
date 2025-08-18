using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace SalesAnalysis.Classes
{
    /// <summary>
    /// Описание модели
    /// </summary>
    public class Model : INotifyPropertyChanged
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Id модели
        /// </summary>
        [Key]
        public int IdModel
        {
            get => _IdModel;
            set
            {
                _IdModel = value;
                //OnPropertyChanged();
            }
        }
        private int _IdModel;

        /// <summary>
        /// Название модели
        /// </summary>
        public string NameModel
        {
            get => _NameModel;
            set
            {
                _NameModel = value;
                //OnPropertyChanged();
            }
        }
        private string _NameModel = string.Empty;

        /// <summary>
        /// Стоимость модели
        /// </summary>
        public double PriceModel
        {
            get => _PriceModel;
            set
            {
                _PriceModel = value;
                //OnPropertyChanged();
            }
        }
        private double _PriceModel;

        #endregion

        /// <summary>
        /// Нужен для дата контекста из БД
        /// </summary>
        public Model()
        {

        }

        public Model(string newNameModel,
                     double newPriceModel)
        {
            NameModel = newNameModel;
            PriceModel = newPriceModel;
        }

        public Model(int newIdModel,
                     string newNameModel,
                     double newPriceModel)
        {
            IdModel = newIdModel;
            NameModel = newNameModel;
            PriceModel = newPriceModel;
        }

        #region ОБНОВЛЕНИЕ UI

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
