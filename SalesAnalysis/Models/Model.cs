using System.ComponentModel.DataAnnotations;

namespace SalesAnalysis.Models
{
    /// <summary>
    /// Описание модели
    /// </summary>
    public class Model
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Id модели
        /// </summary>
        [Key]
        public int IdModel { get; set; }

        /// <summary>
        /// Название модели
        /// </summary>
        public string NameModel { get; set; } = string.Empty;

        /// <summary>
        /// Стоимость модели
        /// </summary>
        public double PriceModel { get; set; }

        #endregion


        #region КОНСТРУКТОР

        /// <summary>
        /// Нужен для дата контекста из БД
        /// </summary>
        public Model()
        {

        }

        public Model(string newNameModel,
                     double newPriceModel)
        {
            NameModel = newNameModel;
            PriceModel = newPriceModel;
        }

        public Model(int newIdModel,
                     string newNameModel,
                     double newPriceModel)
        {
            IdModel = newIdModel;
            NameModel = newNameModel;
            PriceModel = newPriceModel;
        }

        #endregion

    }
}
