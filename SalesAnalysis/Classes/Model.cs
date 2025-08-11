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
    public class Model : INotifyPropertyChanged
    {
        #region ПОЛЯ И СВОЙСТВА

        [Key]
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


        public double PriceModel
        {
            get => _PriceModel;
            set
            {
                _PriceModel = value;
                OnPropertyChanged();
            }
        }
        private double _PriceModel;

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
