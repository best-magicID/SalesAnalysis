using System.Collections.ObjectModel;

namespace SalesAnalysis.Classes
{
    /// <summary>
    /// Продажи конкретной модели за каждый месяц
    /// </summary>
    public class SalesModel : Model
    {
        /// <summary>
        /// Общее количество проданных моделей за Январь
        /// </summary>
        public int TotalAmountForJanuary { get; set; }
        /// <summary>
        /// Общая сумма за все проданные модели за Январь
        /// </summary>
        public double TotalCostForJanuary { get; set; }
        /// <summary>
        /// Лист с датами продаж за Январь
        /// </summary>
        public ObservableCollection<SalesByMonth> ListSaleForJanuary { get; set; } = [];

        public int TotalAmountForFebruary { get; set; }
        public double TotalCostForFebruary { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForFebruary { get; set; } = [];

        public int TotalAmountForMarch { get; set; }
        public double TotalCostForMarch { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForMarch { get; set; } = [];

        public int TotalAmountForApril { get; set; }
        public double TotalCostForApril { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForApril { get; set; } = [];

        public int TotalAmountForMay { get; set; }
        public double TotalCostForMay { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForMay { get; set; } = [];

        public int TotalAmountForJune { get; set; }
        public double TotalCostForJune { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForJune { get; set; } = [];

        public int TotalAmountForJuly { get; set; }
        public double TotalCostForJuly { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForJuly { get; set; } = [];

        public int TotalAmountForAugust { get; set; }
        public double TotalCostForAugust { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForAugust { get; set; } = [];

        public int TotalAmountForSeptember { get; set; }
        public double TotalCostForSeptember { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForSeptember { get; set; } = [];

        public int TotalAmountForOctober { get; set; }
        public double TotalCostForOctober { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForOctober { get; set; } = [];

        public int TotalAmountForNovember { get; set; }
        public double TotalCostForNovember { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForNovember { get; set; } = [];
        
        public int TotalAmountForDecember { get; set; }
        public double TotalCostForDecember { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForDecember { get; set; } = [];

        /// <summary>
        /// Общее стоимость продаж за год
        /// </summary>
        public double TotalCostForYear { get; set; }

        /// <summary>
        /// Общее кол-во продаж за год
        /// </summary>
        public int TotalAmountForYear { get; set; }


        public SalesModel(Model model)
            : base(model.IdModel, model.NameModel, model.PriceModel)
        {

        }

        //public static explicit operator SalesModel(Model model)
        //{
        //    return new SalesModel(newIdModel: model.IdModel,
        //                          newNameModel: model.NameModel,
        //                          newPriceModel: model.PriceModel)
        //    {

        //    };
        //}
    }
}
