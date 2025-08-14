using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesAnalysis.Classes
{
    /// <summary>
    /// Дата продажи модели
    /// </summary>
    public class DateSalesModel : Model
    {
        /// <summary>
        /// Номер по порядку
        /// </summary>
        public int IndexNumber
        {
            get => _IndexNumber;
            set
            {
                _IndexNumber = value;
                //OnPropertyChanged();
            }
        }
        private int _IndexNumber;

        /// <summary>
        /// Id даты продажи
        /// </summary>
        public int IdDateSale
        {
            get => _IdDateSale;
            set
            {
                _IdDateSale = value;
                //OnPropertyChanged();
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
                //OnPropertyChanged();
            }
        }
        private DateTime _DateSaleModel;

        /// <summary>
        /// Количество проданных моделей 
        /// </summary>
        public int CountSoldModels
        {
            get => _CountSoldModels;
            set
            {
                _CountSoldModels = value;
                //OnPropertyChanged();
            }
        }
        private int _CountSoldModels;

        /// <summary>
        /// Стоимость всех проданных моделей 
        /// </summary>
        public double CostAllModelsSold
        {
            get => _CostAllModelsSold;
            set
            {
                _CostAllModelsSold = value;
                //OnPropertyChanged();
            }
        }
        private double _CostAllModelsSold;


        public DateSalesModel(int newIndexNumber, 
                               int newIdModel, 
                               string newNameModel,
                               double newPriceModel,
                               int newIdDateSale,
                               DateTime newDateSaleModel,
                               int newCountSoldModels) 
            : base(newIdModel, newNameModel, newPriceModel) 
        {
            IndexNumber = newIdModel; //

            IdDateSale = newIdDateSale;
            DateSaleModel = newDateSaleModel;
            CountSoldModels = newCountSoldModels;
            CostAllModelsSold = base.PriceModel * CountSoldModels;
        }
    }
}
