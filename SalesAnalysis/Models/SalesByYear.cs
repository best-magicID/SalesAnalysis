using System.Collections.ObjectModel;

namespace SalesAnalysis.Models
{
    /// <summary>
    /// Продажи конкретной модели за каждый месяц
    /// </summary>
    public class SalesByYear : Model
    {
        /// <summary>
        /// Массив из 12 листов с продажами за каждый месяц
        /// </summary>
        public ObservableCollection<SalesByMonth>[] ListAllSalesByMonths { get; set; } = new ObservableCollection<SalesByMonth>[12];

        /// <summary>
        /// Массив из 12 общих стоимостей продаж за каждый месяц
        /// </summary>
        public double[] ArrAllTotalCosts { get; set; } = new double[12];

        /// <summary>
        /// Массив из 12 общих кол-в продаж за каждый месяц
        /// </summary>
        public int[] ArrAllTotalAmounts { get; set; } = new int[12];

        /// <summary>
        /// Общая стоимость продаж за год
        /// </summary>
        public double TotalCostForYear { get; set; }

        /// <summary>
        /// Общее кол-во продаж за год
        /// </summary>
        public int TotalAmountForYear { get; set; }


        public SalesByYear(Model model)
            : base(model.IdModel, model.NameModel, model.PriceModel)
        {
            for (int i = 0; i < 12; i++)
            {
                ListAllSalesByMonths[i] = new ObservableCollection<SalesByMonth>();
            }
        }

    }
}
