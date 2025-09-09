using SalesAnalysis.Models;
using System.Collections.ObjectModel;

namespace SalesAnalysis.Services
{
    /// <summary>
    /// Интерфейс работы с Excel
    /// </summary>
    public interface IWorkingWithExcel
    {
        /// <summary>
        /// Сохранение списка в Excel
        /// </summary>
        /// <param name="listSalesModels"></param>
        Task SaveToExcel(ObservableCollection<SalesByYear> listSalesModels, string pathFile);
    }
}
