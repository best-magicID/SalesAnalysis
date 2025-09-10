using Microsoft.Win32;
using SalesAnalysis.Data;
using SalesAnalysis.Helpers;
using SalesAnalysis.Models;
using SalesAnalysis.Services;
using SalesAnalysis.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace SalesAnalysis.ViewModels
{
    public delegate void LoadDataFromBD();

    /// <summary>
    /// ViewModel для главного окна
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Работа с Excel
        /// </summary>
        private readonly IWorkingWithExcel? _iWorkingWithExcel;

        /// <summary>
        /// Работа с БД
        /// </summary>
        private readonly IOperationsDb? _iOperationsDb;

        private readonly IWindowFactory? _iWindowFactory;

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
        public List<Months> ListMonths { get; set; } = [];

        /// <summary>
        /// Лист проданных моделей, на основе его отображается основная таблица с моделями
        /// </summary>
        public ObservableCollection<SalesByYear> ListSalesModels { get; set; } = [];

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
        private int _SelectedYear { get; set; }

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

                //if (SelectedModel != null)
                //{
                    UpdateDataInTableForSelectedModel?.Invoke();
                //}
            }
        }
        private Model? _SelectedModel;


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

        public MainWindowViewModel(IWorkingWithExcel newIWorkingWithExcel, 
                                   IOperationsDb newIOperationsDb, 
                                   IWindowFactory newIWindowFactory)
        {
            _iOperationsDb = newIOperationsDb;

            if (CheckConnect())
            {
                _iWorkingWithExcel = newIWorkingWithExcel;
                _iWindowFactory = newIWindowFactory;

                ListMonths = Enum.GetValues(typeof(Months)).Cast<Months>().ToList();
                LoadCommands();

                GetListYearsFromBd();

                UpdateDataInTableForAllModels += GetModelsFromBd;
                UpdateDataInTableForAllModels += GetDatesSalesModelsFromDb;
                UpdateDataInTableForAllModels += ConvertDataFromBD;
                UpdateDataInTableForAllModels.Invoke();

                UpdateDataInTableForSelectedModel += GetDatesSalesModelsFromDb;
                UpdateDataInTableForSelectedModel += ConvertDataFromBD;
            }
        }

        #endregion


        #region МЕТОДЫ

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
            return _iOperationsDb?.CheckConnect() ?? false;
        }

        /// <summary>
        /// Добавление новой модели в БД
        /// </summary>
        /// <param name="name"></param>
        /// <param name="price"></param>
        public void AddModelInBd(string? name = "", string? price = "")
        {
            WindowForAddNewModel_View WindowForAddNewModel_View = _iWindowFactory.CreateWindow<WindowForAddNewModel_View, WindowForAddNewModel_ViewModel>();
            
            WindowForAddNewModel_View.ShowDialog();

            var windowForAddNewModel_ViewModel = WindowForAddNewModel_View.DataContext as WindowForAddNewModel_ViewModel;

            if (windowForAddNewModel_ViewModel?.IsSave ?? false)
            {
                using MyDbContext db = new ();

                var newModel = new Model(newNameModel: windowForAddNewModel_ViewModel.NameModel,
                                         newPriceModel: windowForAddNewModel_ViewModel.PriceModel);
                db.Models.Add(newModel);
                db.SaveChanges();

                ListModels.Add(newModel);
            }
        }

        /// <summary>
        /// Удалить модель из БД
        /// </summary>
        /// <param name="text"></param>
        /// <param name="model"></param>
        public void DeleteModelInBd(string text, Model model)
        {
            if (GeneralMethods.ShowSelectionWindow(text) == MessageBoxResult.Yes)
            {
                SelectedModel = null;

                using MyDbContext db = new();

                db.Models.Remove(model);
                db.SaveChanges();

                ListModels.Remove(model);

                //UpdateDataInTableForAllModels?.Invoke();
            }
        }

        /// <summary>
        /// Получение списка моделей из БД
        /// </summary>
        public void GetModelsFromBd()
        {
            var tempList = _iOperationsDb?.GetModelsFromBd(SelectedModel);

            ListModels.Clear();
            tempList?.ForEach(x => ListModels.Add(x));
        }

        /// <summary>
        /// Получить список лет, за которые продавались модели
        /// </summary>
        public void GetListYearsFromBd()
        {
            var listYears = _iOperationsDb?.GetListYearsFromBd();

            if (listYears != null)
            {
                ListYears.Clear();
                listYears.ForEach(x => ListYears.Add(x));

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
        }

        /// <summary>
        /// Получение дат продаж моделей из БД за выбранный год
        /// </summary>
        public void GetDatesSalesModelsFromDb()
        {
            var tempList = _iOperationsDb?.GetDatesSalesModelsFromDb(SelectedModel, SelectedYear);

            ListAllDatesSalesModels.Clear();
            tempList?.ForEach(x => ListAllDatesSalesModels.Add(x));
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
                    ListSalesModels.Add(new SalesByYear(model));
                }
                else if (SelectedModel.IdModel == model.IdModel)
                {
                    ListSalesModels.Add(new SalesByYear(model));
                    break;
                }
            }

            foreach (SalesByYear salesModel in ListSalesModels)
            {
                foreach (DateSalesModel dateSalesModel in ListAllDatesSalesModels)
                {
                    if (salesModel.IdModel == dateSalesModel.IdModel)
                    {
                        var numberMonth = dateSalesModel.DateSaleModel.Month;

                        SalesByMonth salesByMonth = new SalesByMonth(newIdModel: dateSalesModel.IdModel,
                                                                     newNameModel: dateSalesModel.NameModel,
                                                                     newPriceModel: dateSalesModel.PriceModel,
                                                                     newMonth: ListMonths.Single(month => (int)month == numberMonth),
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
        public void AddInListDateSaleAndCalculatingSums(int numberMonth, SalesByYear salesModel, SalesByMonth salesByMonth, DateSalesModel dateSalesModel)
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
        public void SaveToExcel(ObservableCollection<SalesByYear> listSalesModels)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Excel файлы|*.xlsx",
                RestoreDirectory = true,
                FileName = "Продажи " + DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss") + ".xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _iWorkingWithExcel?.SaveToExcel(listSalesModels, saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Создание строк с датами продаж (Тестовые данные)
        /// </summary>
        public void CreateRecordsInBd()
        {
            using MyDbContext db = new();

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
        /// Добавление новой даты продажи в БД
        /// </summary>
        public void AddDatSaleInDb()
        {
            var listModels = _iOperationsDb?.GetModelsFromBd(null);

            WindowForAddingDateSaleView windowForAddingDateSaleView = _iWindowFactory.CreateWindow<WindowForAddingDateSaleView, WindowForAddingDateSaleViewModel>(listModels);

            windowForAddingDateSaleView.ShowDialog();

            var windowForAddingDateSaleViewModel = windowForAddingDateSaleView.DataContext as WindowForAddingDateSaleViewModel;

            if (windowForAddingDateSaleViewModel?.IsSave ?? false)
            {
                /// изменить на использование IOperationsDb
                MyDbContext myDbContext = new();
                myDbContext.DatesSale.Add(windowForAddingDateSaleViewModel.NewDateSale);
                myDbContext.SaveChanges();
            }
        }

        #endregion

    }
}
