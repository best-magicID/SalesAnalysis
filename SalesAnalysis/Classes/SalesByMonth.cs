using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesAnalysis.Classes
{
    /// <summary>
    /// Продажи моделей за месяц
    /// </summary>
    public class SalesByMonth : Model
    {
        /// <summary>
        /// Месяц
        /// </summary>
        public Month Month { get; set; }

        /// <summary>
        /// Лист с датами продаж 
        /// </summary>
        public ObservableCollection<DateSalesModel> ListDateSalesModels { get; set; } = [];

        ///// <summary>
        ///// Сумма продаж за месяц
        ///// </summary>
        //public double MonthlyAmount { get; set; }

        public SalesByMonth(int newIdModel,
                            string newNameModel,
                            double newPriceModel,
                            Month newMonth,
                            DateSalesModel newDateSalesModels)
            : base(newIdModel, newNameModel, newPriceModel)
        {
            Month = newMonth;
            ListDateSalesModels.Add(newDateSalesModels);
        }

        public SalesByMonth(int newIdModel,
                            string newNameModel,
                            double newPriceModel,
                            Month newMonth,
                            ObservableCollection<DateSalesModel> newListDateSalesModels/*,double newMonthlyAmount*/)
            : base(newIdModel, newNameModel, newPriceModel)
        {
            Month = newMonth;
            ListDateSalesModels = newListDateSalesModels;
            //MonthlyAmount = newMonthlyAmount;
        }

        public SalesByMonth(DateSalesModel dateSalesModel,
                            Month newMonth,
                            ObservableCollection<DateSalesModel> newListDateSalesModels/*, double newMonthlyAmount*/)
            : base(dateSalesModel.IdModel, dateSalesModel.NameModel, dateSalesModel.PriceModel)
        {
            Month = newMonth;
            ListDateSalesModels = newListDateSalesModels;
            //MonthlyAmount = newMonthlyAmount;
        }
    }
}
