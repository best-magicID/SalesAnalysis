using System.Collections.ObjectModel;

namespace SalesAnalysis.Models
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

    }
}
