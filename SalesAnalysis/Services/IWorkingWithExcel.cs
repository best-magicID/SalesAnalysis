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
        void SaveToExcel(ObservableCollection<SalesModel> listSalesModels);
    }
}
