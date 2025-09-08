using DocumentFormat.OpenXml;
using SalesAnalysis.Data;
using SalesAnalysis.Helpers;
using SalesAnalysis.Models;
using SalesAnalysis.Services;
using SalesAnalysis.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SalesAnalysis.ViewModels
{
    public delegate void LoadDataFromBD();

    /// <summary>
    /// ViewModel для главного окна
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Лист моделей полученных из БД
        /// </summary>
        public ObservableCollection<Model> ListModels { get; set; } = [];

        /// <summary>
        /// Лист всех дат полученный из БД
        /// </summary>
        public ObservableCollection<DateSalesModel> ListAllDatesSalesModels { get; set; } = [];

        /// <summary>
        /// Лист месяцев полученный из БД
        /// </summary>
        public List<Month> ListMonths { get; set; } = [];

        /// <summary>
        /// Лист проданных моделей, на основе его отображается основная таблица с моделями
        /// </summary>
        public ObservableCollection<SalesModel> ListSalesModels { get; set; } = [];

        /// <summary>
        /// Список лет за которые продавались модели
        /// </summary>
        public ObservableCollection<int> ListYears { get; set; } = [];

        /// <summary>
        /// Выбранный год за который показать список моделей
        /// </summary>
        public int SelectedYear
        {
            get => _SelectedYear;
            set
            {
                _SelectedYear = value;
                OnPropertyChanged();

                if (SelectedModel == null)
                {
                    UpdateDataInTableForAllModels?.Invoke();
                }
                else
                {
                    UpdateDataInTableForSelectedModel?.Invoke();
                }
            }
        }
        private int _SelectedYear { get; set; } = DateTime.Now.Year;

        /// <summary>
        /// Загрузка данных из БД для всех моделей 
        /// </summary>
        public LoadDataFromBD? UpdateDataInTableForAllModels;

        /// <summary>
        /// Загрузка данных из БД для конкретной модели
        /// </summary>
        public LoadDataFromBD? UpdateDataInTableForSelectedModel;

        /// <summary>
        /// Выбранная модель
        /// </summary>
        public Model? SelectedModel
        {
            get => _SelectedModel;
            set
            {
                _SelectedModel = value;

                if (SelectedModel != null)
                {
                    UpdateDataInTableForSelectedModel?.Invoke();
                }
            }
        }
        private Model? _SelectedModel;

        /// <summary>
        /// Работа с Excel
        /// </summary>
        private readonly IWorkingWithExcel? _iWorkingWithExcel;

        #region КОМАНДЫ

        public RaiseCommand? AddModelCommand { get; set; }
        public RaiseCommand? DeleteModelCommand { get; set; }

        public RaiseCommand? ShowAllModelsCommand { get; set; }
        public RaiseCommand? SaveToExcelCommand { get; set; }
        public RaiseCommand? AddDateSaleCommand { get; set; }

        #endregion

        #endregion


        #region КОНСТРУКТОР

        public MainWindowViewModel()
        {

        }

        public MainWindowViewModel(IWorkingWithExcel newIWorkingWithExcel)
        {
            if (CheckConnect())
            {
                LoadCommands();
                _iWorkingWithExcel = newIWorkingWithExcel;

                GetMonthsFromBd();

                UpdateDataInTableForAllModels += GetModelsFromBd;
                GetListYearsFromBd();

                UpdateDataInTableForAllModels += GetDatesSalesModelsFromDb;

                UpdateDataInTableForAllModels += ConvertDataFromBD;

                UpdateDataInTableForAllModels.Invoke();

                UpdateDataInTableForSelectedModel += GetDatesSalesModelsFromDb;
                UpdateDataInTableForSelectedModel += ConvertDataFromBD;
            }
        }

        #endregion


        #region МЕТОДЫ

        #region ОБНОВЛЕНИЕ UI

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Загрузка команд
        /// </summary>
        public void LoadCommands()
        {
            AddModelCommand = new RaiseCommand(AddModelCommand_Execute);
            DeleteModelCommand = new RaiseCommand(DeleteModelCommand_Execute);

            ShowAllModelsCommand = new RaiseCommand(ShowAllModelsCommand_Execute);

            SaveToExcelCommand = new RaiseCommand(SaveToExcelCommand_Execute);

            AddDateSaleCommand = new RaiseCommand(AddDateSaleCommand_Execute);
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
                using MyDbContext db = new();

                isConnect = db.Database.CanConnect();
            }
            catch (Exception ex)
            {
                GeneralMethods.ShowNotification("Нет доступа к БД.\r\n\r\nОшибка: " + ex.Message);
            }
            return isConnect;
        }

        /// <summary>
        /// Добавление новой модели в БД
        /// </summary>
        /// <param name="name"></param>
        /// <param name="price"></param>
        public void AddModelInBd(string? name = "", string? price = "")
        {
            WindowForAddNewModel_ViewModel windowForAddNewModel_ViewModel = new ();
            WindowForAddNewModel_View windowForAddNewModel = new()
            {
                DataContext = windowForAddNewModel_ViewModel
            };
            windowForAddNewModel.ShowDialog();

            if (windowForAddNewModel_ViewModel.IsSave)
            {
                using MyDbContext db = new MyDbContext();

                db.Models.Add(new Model(newNameModel: windowForAddNewModel_ViewModel.NameModel,
                                        newPriceModel: (double.TryParse(windowForAddNewModel_ViewModel.PriceModel, out double priceModel) ? priceModel : 0)));
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Удалить модель из БД
        /// </summary>
        /// <param name="text"></param>
        /// <param name="idModel"></param>
        public void DeleteModelInBd(string text, Model idModel)
        {
            if (GeneralMethods.ShowSelectionWindow(text) == MessageBoxResult.Yes)
            {
                using MyDbContext db = new MyDbContext();

                db.Models.Remove(idModel);
                db.SaveChanges();

                SelectedModel = null;
                UpdateDataInTableForAllModels?.Invoke();
            }
        }

        /// <summary>
        /// Получение списка моделей из БД
        /// </summary>
        public void GetModelsFromBd()
        {
            using (MyDbContext db = new())
            {
                ListModels.Clear();

                List<Model> listModel = [];
                if (SelectedModel != null)
                {
                    listModel = db.Models.Where(x => x.IdModel == SelectedModel.IdModel).ToList();
                }
                else
                {
                    listModel = db.Models.ToList();
                }

                foreach (Model model in listModel)
                {
                    ListModels.Add(new Model(newIdModel: model.IdModel,
                                             newNameModel: model.NameModel,
                                             newPriceModel: model.PriceModel));
                }
            }
        }

        /// <summary>
        /// Получение списка месяцев из БД
        /// </summary>
        public void GetMonthsFromBd()
        {
            using (MyDbContext db = new MyDbContext())
            {
                ListMonths.Clear();

                var months = db.Months.ToList();

                foreach (Month month in months)
                {
                    ListMonths.Add(new Month()
                    {
                        IdMonth = month.IdMonth,
                        NameMonth = month.NameMonth
                    });
                }
            }
        }

        /// <summary>
        /// Получить список лет, за которые продавались модели
        /// </summary>
        public void GetListYearsFromBd()
        {
            using (MyDbContext db = new())
            {
                ListYears.Clear();

                var years = db.DatesSale.Select(x => x.DateSaleModel).ToList(); ;

                foreach (var year in years)
                {
                    if (!ListYears.Any(x => x == year.Year))
                    {
                        ListYears.Add(year.Year);
                    }
                }
                if (ListYears.Count > 0)
                {
                    SelectedYear = ListYears.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Кнопка добавления новой модели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddModelCommand_Execute(object parameter)
        {
            AddModelInBd();
            GetModelsFromBd();
        }

        /// <summary>
        /// Получение дат продаж моделей из БД
        /// </summary>
        public void GetDatesSalesModelsFromDb()
        {
            using MyDbContext db = new MyDbContext();

            var data = (from tModels in (SelectedModel != null ? db.Models.Where(x => x.IdModel == SelectedModel.IdModel) : db.Models)
                        join tDatesSale in db.DatesSale on tModels.IdModel equals tDatesSale.IdModel
                        where tDatesSale.DateSaleModel.Year == SelectedYear
                        select new
                        {
                            tModels.IdModel,
                            tModels.NameModel,
                            tModels.PriceModel,
                            tDatesSale.IdDateSale,
                            tDatesSale.DateSaleModel,
                            tDatesSale.CountSoldModels
                        }).ToList();

            ListAllDatesSalesModels.Clear();
            int i = 1;
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    ListAllDatesSalesModels.Add(new(newIndexNumber: i++,
                                                    newIdModel: item.IdModel,
                                                    newNameModel: item.NameModel,
                                                    newPriceModel: item.PriceModel,
                                                    newIdDateSale: item.IdDateSale,
                                                    newDateSaleModel: item.DateSaleModel,
                                                    newCountSoldModels: item.CountSoldModels));
                }
            }
        }

        /// <summary>
        /// Конвертация данных полученных из БД в объекты
        /// </summary>
        public void ConvertDataFromBD()
        {
            ListSalesModels.Clear();

            foreach (Model model in ListModels)
            {
                if (SelectedModel == null) // Добавление всех моделей в список
                {
                    ListSalesModels.Add(new SalesModel(model));
                }
                else
                {
                    if (SelectedModel.IdModel == model.IdModel)
                    {
                        ListSalesModels.Add(new SalesModel(model));
                    }
                }
            }

            foreach (SalesModel salesModel in ListSalesModels)
            {
                foreach (DateSalesModel dateSalesModel in ListAllDatesSalesModels)
                {
                    if (salesModel.IdModel == dateSalesModel.IdModel)
                    {
                        var numberMonth = dateSalesModel.DateSaleModel.Month;

                        SalesByMonth salesByMonth = new SalesByMonth(newIdModel: dateSalesModel.IdModel,
                                                                     newNameModel: dateSalesModel.NameModel,
                                                                     newPriceModel: dateSalesModel.PriceModel,
                                                                     newMonth: ListMonths.Single(month => month.IdMonth == numberMonth),
                                                                     newDateSalesModels: dateSalesModel);

                        AddInListDateSaleAndCalculatingSums(numberMonth, salesModel, salesByMonth, dateSalesModel);
                    }
                }
            }
        }

        /// <summary>
        /// Добавление в лист дат продаж и подсчет годовой суммы
        /// </summary>
        /// <param name="numberMonth"></param>
        /// <param name="salesModel"></param>
        /// <param name="salesByMonth"></param>
        /// <param name="dateSalesModel"></param>
        public void AddInListDateSaleAndCalculatingSums(int numberMonth, SalesModel salesModel, SalesByMonth salesByMonth, DateSalesModel dateSalesModel)
        {
            switch (numberMonth)
            {
                case 1:
                    salesModel.ListSaleForJanuary.Add(salesByMonth);
                    salesModel.TotalCostForJanuary = salesModel.TotalCostForJanuary + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForJanuary = salesModel.TotalAmountForJanuary + dateSalesModel.CountSoldModels;
                    break;
                case 2:
                    salesModel.ListSaleForFebruary.Add(salesByMonth);
                    salesModel.TotalCostForFebruary = salesModel.TotalCostForFebruary + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForFebruary = salesModel.TotalAmountForFebruary + dateSalesModel.CountSoldModels;
                    break;
                case 3:
                    salesModel.ListSaleForMarch.Add(salesByMonth);
                    salesModel.TotalCostForMarch = salesModel.TotalCostForMarch + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForMarch = salesModel.TotalAmountForMarch + dateSalesModel.CountSoldModels;
                    break;
                case 4:
                    salesModel.ListSaleForApril.Add(salesByMonth);
                    salesModel.TotalCostForApril = salesModel.TotalCostForApril + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForApril = salesModel.TotalAmountForApril + dateSalesModel.CountSoldModels;
                    break;
                case 5:
                    salesModel.ListSaleForMay.Add(salesByMonth);
                    salesModel.TotalCostForMay = salesModel.TotalCostForMay + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForMay = salesModel.TotalAmountForMay + dateSalesModel.CountSoldModels;
                    break;
                case 6:
                    salesModel.ListSaleForJune.Add(salesByMonth);
                    salesModel.TotalCostForJune = salesModel.TotalCostForJune + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForJune = salesModel.TotalAmountForJune + dateSalesModel.CountSoldModels;
                    break;
                case 7:
                    salesModel.ListSaleForJuly.Add(salesByMonth);
                    salesModel.TotalCostForJuly = salesModel.TotalCostForJuly + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForJuly = salesModel.TotalAmountForJuly + dateSalesModel.CountSoldModels;
                    break;
                case 8:
                    salesModel.ListSaleForAugust.Add(salesByMonth);
                    salesModel.TotalCostForAugust = salesModel.TotalCostForAugust + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForAugust = salesModel.TotalAmountForAugust + dateSalesModel.CountSoldModels;
                    break;
                case 9:
                    salesModel.ListSaleForSeptember.Add(salesByMonth);
                    salesModel.TotalCostForSeptember = salesModel.TotalCostForSeptember + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForSeptember = salesModel.TotalAmountForSeptember + dateSalesModel.CountSoldModels;
                    break;
                case 10:
                    salesModel.ListSaleForOctober.Add(salesByMonth);
                    salesModel.TotalCostForOctober = salesModel.TotalCostForOctober + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForOctober = salesModel.TotalAmountForOctober + dateSalesModel.CountSoldModels;
                    break;
                case 11:
                    salesModel.ListSaleForNovember.Add(salesByMonth);
                    salesModel.TotalCostForNovember = salesModel.TotalCostForNovember + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForNovember = salesModel.TotalAmountForNovember + dateSalesModel.CountSoldModels;
                    break;
                case 12:
                    salesModel.ListSaleForDecember.Add(salesByMonth);
                    salesModel.TotalCostForDecember = salesModel.TotalCostForDecember + dateSalesModel.CostAllModelsSold;
                    salesModel.TotalAmountForDecember = salesModel.TotalAmountForDecember + dateSalesModel.CountSoldModels;
                    break;
            }

            salesModel.TotalCostForYear = salesModel.TotalCostForJanuary +
                                          salesModel.TotalCostForFebruary +
                                          salesModel.TotalCostForMarch +
                                          salesModel.TotalCostForApril +
                                          salesModel.TotalCostForMay +
                                          salesModel.TotalCostForJune +
                                          salesModel.TotalCostForJuly +
                                          salesModel.TotalCostForAugust +
                                          salesModel.TotalCostForSeptember +
                                          salesModel.TotalCostForOctober +
                                          salesModel.TotalCostForNovember +
                                          salesModel.TotalCostForDecember;

            salesModel.TotalAmountForYear = salesModel.TotalAmountForJanuary +
                                            salesModel.TotalAmountForFebruary +
                                            salesModel.TotalAmountForMarch +
                                            salesModel.TotalAmountForApril +
                                            salesModel.TotalAmountForMay +
                                            salesModel.TotalAmountForJune +
                                            salesModel.TotalAmountForJuly +
                                            salesModel.TotalAmountForAugust +
                                            salesModel.TotalAmountForSeptember +
                                            salesModel.TotalAmountForOctober +
                                            salesModel.TotalAmountForNovember +
                                            salesModel.TotalAmountForDecember;
        }

        /// <summary>
        /// Показать все модели
        /// </summary>
        private void ShowAllModelsCommand_Execute(object parameter)
        {
            SelectedModel = null;

            UpdateDataInTableForAllModels?.Invoke();
        }


        /// <summary>
        /// Удаление модели
        /// </summary>
        private void DeleteModelCommand_Execute(object parameter)
        {
            if (SelectedModel != null)
            {
                string text = "Удалить: " + SelectedModel.NameModel;
                DeleteModelInBd(text, SelectedModel);
            }
        }

        /// <summary>
        /// Кнопка для сохранения таблицы в Excel
        /// </summary>
        private void SaveToExcelCommand_Execute(object parameter)
        {
            SaveToExcel(ListSalesModels);
        }

        /// <summary>
        /// Сохранение в Excel
        /// </summary>
        /// <param name="listSalesModels"></param>
        public void SaveToExcel(ObservableCollection<SalesModel> listSalesModels)
        {
            _iWorkingWithExcel?.SaveToExcel(listSalesModels);
        }

        /// <summary>
        /// Создание строк с датами продаж
        /// </summary>
        public void CreateRecordsInBd()
        {
            using MyDbContext db = new MyDbContext();

            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                db.DatesSale.Add(new DateSale()
                {
                    DateSaleModel = DateTime.Now.Date.AddDays(-i),
                    IdModel = random.Next(1, 8),
                    CountSoldModels = random.Next(0, 10)
                });
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Выполнить команду "Добавление даты продаж"
        /// </summary>
        private void AddDateSaleCommand_Execute(object parameter)
        {
            AddDatSaleInDb();
        }

        /// <summary>
        /// Добавить новую дату продаж в БД
        /// </summary>
        public void AddDatSaleInDb()
        {
            using MyDbContext myDbContext = new MyDbContext();
            var listModels = myDbContext.Models.ToList<Model>();

            WindowForAddingDateSaleViewModel windowForAddingDateSaleViewModel = new WindowForAddingDateSaleViewModel( listModels );
            WindowForAddingDateSaleView windowForAddingDateSale = new()
            {
                DataContext = windowForAddingDateSaleViewModel
            };
            windowForAddingDateSale.ShowDialog();

            if (windowForAddingDateSaleViewModel.IsSave)
            {
                myDbContext.DatesSale.Add(windowForAddingDateSaleViewModel.NewDateSale);
                myDbContext.SaveChanges();
            }
        }

        #endregion

    }
}
