using SalesAnalysis.Models;

namespace SalesAnalysis.Data
{
    public interface IChangingDataInDb
    {
        /// <summary>
        /// Добавление новой модели в БД
        /// </summary>
        Task AddModelInBd(Model model);

        /// <summary>
        /// Удаление модели из БД
        /// </summary>
        Task DeleteModelInBd(Model model);

        /// <summary>
        /// Добавление новой даты продажи в БД
        /// </summary>
        Task AddDatSaleInDb(DateSale dateSale);
    }
}
