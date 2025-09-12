using SalesAnalysis.Helpers;
using SalesAnalysis.Models;

namespace SalesAnalysis.Data
{
    /// <summary>
    /// Операции с БД
    /// </summary>
    public class OperationsDb : IGetDataFromDb, IChangingDataInDb
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

        /// <summary>
        /// Добавление новой модели в БД
        /// </summary>
        public async Task AddModelInBd(Model model)
        {
            await Task.Run(() =>
            {
                _myDbContext.Models.Add(model);
                _myDbContext.SaveChanges();

                GeneralMethods.ShowNotification($"Модель '{model.NameModel}' успешно добавлена в БД.");
            });
        }

        /// <summary>
        /// Удаление модели из БД
        /// </summary>
        public async Task DeleteModelInBd(Model model)
        {
            await Task.Run(() =>
            {
                _myDbContext.Models.Remove(model);
                _myDbContext.SaveChanges();

                GeneralMethods.ShowNotification($"Модель '{model.NameModel}' успешно удалена из БД.");
            });
        }

        /// <summary>
        /// Добавление новой даты продажи в БД
        /// </summary>
        public async Task AddDatSaleInDb(DateSale dateSale)
        {
            await Task.Run(() =>
            {
                _myDbContext.DatesSale.Add(dateSale);
                _myDbContext.SaveChanges();

                GeneralMethods.ShowNotification($"Добавлена дата продажи модели '{dateSale.DateSaleModel.ToShortDateString()}' в БД.");
            });
        }

        /// <summary>
        /// Создание строк с датами продаж (Тестовые данные)
        /// </summary>
        public async Task CreateRecordsInBd()
        {
            await Task.Run(() =>
            {
                Random random = new();

                for (int i = 0; i < 50000; i++)
                {
                    _myDbContext.DatesSale.Add(new DateSale()
                    {
                        DateSaleModel = DateTime.Now.Date.AddDays(-i),
                        IdModel = random.Next(1, 8),
                        CountSoldModels = random.Next(0, 10)
                    });
                }
                _myDbContext.SaveChanges();
            });
        }

    }
}
