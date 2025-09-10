using SalesAnalysis.Models;
using System.Collections.ObjectModel;

namespace SalesAnalysis.Data
{
    public interface IOperationsDb
    {
        /// <summary>
        /// Проверка соединения с БД
        /// </summary>
        /// <returns>true - если есть подключение</returns>
        bool CheckConnect();

        /// <summary>
        /// Получение списка лет, за которые продавались модели из БД
        /// </summary>
        List<int>? GetListYearsFromBd();

        /// <summary>
        /// Получение списка моделей из БД
        /// </summary>
        List<Model> GetModelsFromBd(Model? selectedModel);

        /// <summary>
        /// Получение дат продаж моделей из БД за выбранный год
        /// </summary>
        List<DateSalesModel> GetDatesSalesModelsFromDb(Model? selectedModel, int selectedYear);
    }
}
