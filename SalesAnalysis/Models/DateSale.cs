using System.ComponentModel.DataAnnotations;

namespace SalesAnalysis.Models
{
    /// <summary>
    /// Описание класса Дата продаж модели за конкретный день (берется из БД)
    /// </summary>
    public class DateSale
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Id даты продажи
        /// </summary>
        [Key]
        public int IdDateSale { get; set; }

        /// <summary>
        /// Дата продажи модели 
        /// </summary>
        public DateTime DateSaleModel { get; set; }

        /// <summary>
        /// Id модели
        /// </summary>
        public int IdModel { get; set; }

        /// <summary>
        /// Количество проданных моделей 
        /// </summary>
        public int CountSoldModels { get; set; }

        #endregion

    }
}
