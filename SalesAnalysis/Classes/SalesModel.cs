using System.Collections.ObjectModel;

namespace SalesAnalysis.Classes
{
    /// <summary>
    /// Продажи конкретной модели за каждый месяц
    /// </summary>
    public class SalesModel : Model
    {
        public double TotalCostForJanuary { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForJanuary { get; set; } = [];

        public double TotalCostForFebruary { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForFebruary { get; set; } = [];

        public double TotalCostForMarch { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForMarch { get; set; } = [];

        public double TotalCostForApril { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForApril { get; set; } = [];

        public double TotalCostForMay { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForMay { get; set; } = [];

        public double TotalCostForJune { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForJune { get; set; } = [];

        public double TotalCostForJuly { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForJuly { get; set; } = [];

        public double TotalCostForAugust { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForAugust { get; set; } = [];

        public double TotalCostForSeptember { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForSeptember { get; set; } = [];

        public double TotalCostForOctober { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForOctober { get; set; } = [];

        public double TotalCostForNovember { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForNovember { get; set; } = [];

        public double TotalCostForDecember { get; set; }
        public ObservableCollection<SalesByMonth> ListSaleForDecember { get; set; } = [];

        public double TotalCostForYear { get; set; }


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
