using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SalesAnalysis.Classes
{
    internal class DateSale : INotifyPropertyChanged
    {
        #region ПОЛЯ И СВОЙСТВА

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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
