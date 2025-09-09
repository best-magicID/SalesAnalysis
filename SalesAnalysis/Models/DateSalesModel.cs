namespace SalesAnalysis.Models
{
    /// <summary>
    /// Дата продажи модели (наследуется от Model)
    /// </summary>
    public class DateSalesModel : Model
    {
        /// <summary>
        /// Id даты продажи
        /// </summary>
        public int IdDateSale { get; set; }

        /// <summary>
        /// Дата продажи модели 
        /// </summary>
        public DateTime DateSaleModel { get; set; }

        /// <summary>
        /// Количество проданных моделей 
        /// </summary>
        public int CountSoldModels { get; set; }

        /// <summary>
        /// Стоимость всех проданных моделей 
        /// </summary>
        public double CostAllModelsSold { get; set; }


        public DateSalesModel(int newIdModel,
                              string newNameModel,
                              double newPriceModel,
                              int newIdDateSale,
                              DateTime newDateSaleModel,
                              int newCountSoldModels) 
            : base(newIdModel, newNameModel, newPriceModel) 
        {
            IdDateSale = newIdDateSale;
            DateSaleModel = newDateSaleModel;
            CountSoldModels = newCountSoldModels;
            CostAllModelsSold = base.PriceModel * CountSoldModels;
        }
    }
}
