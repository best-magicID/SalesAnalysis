using SalesAnalysis.Helpers;
using SalesAnalysis.Models;

namespace SalesAnalysis.Data
{
    /// <summary>
    /// Операции с БД
    /// </summary>
    public class OperationsDb : IOperationsDb
    {
        private readonly MyDbContext _myDbContext;

        public OperationsDb()
        {
            _myDbContext = new MyDbContext();
        }

        /// <summary>
        /// Проверка соединения с БД
        /// </summary>
        /// <returns></returns>
        public bool CheckConnect()
        {
            bool isConnect = false;
            try
            {
                isConnect = _myDbContext.Database.CanConnect();
            }
            catch (Exception ex)
            {
                GeneralMethods.ShowNotification("Нет доступа к БД.\r\n\r\nОшибка: " + ex.Message);
            }
            return isConnect;
        }

        /// <summary>
        /// Получить список лет, за которые продавались модели
        /// </summary>
        public List<int>? GetListYearsFromBd()
        {
            var years = _myDbContext.DatesSale.Select(x => x.DateSaleModel.Year)
                                              .Distinct()
                                              .Order()
                                              .Reverse()
                                              .ToList();
            return years;
        }

        /// <summary>
        /// Получение списка моделей из БД
        /// </summary>
        public List<Model> GetModelsFromBd(Model? selectedModel)
        {
            using MyDbContext db = new();

            List<Model> listModel = selectedModel == null ? _myDbContext.Models.ToList<Model>() : _myDbContext.Models.Where(x => x.IdModel == selectedModel.IdModel).ToList<Model>();

            return listModel;
        }

        /// <summary>
        /// Получение дат продаж моделей из БД за выбранный год
        /// </summary>
        public List<DateSalesModel> GetDatesSalesModelsFromDb(Model? selectedModel, int selectedYear)
        {
            var data = (from tModels in (selectedModel == null ? _myDbContext.Models : _myDbContext.Models.Where(x => x.IdModel == selectedModel.IdModel))
                        join tDatesSale in _myDbContext.DatesSale on tModels.IdModel equals tDatesSale.IdModel
                        where tDatesSale.DateSaleModel.Year == selectedYear
                        select new DateSalesModel(tModels.IdModel,
                                                  tModels.NameModel,
                                                  tModels.PriceModel,
                                                  tDatesSale.IdDateSale,
                                                  tDatesSale.DateSaleModel,
                                                  tDatesSale.CountSoldModels))
                                                  .ToList();
            return data;
        }
    }
}
