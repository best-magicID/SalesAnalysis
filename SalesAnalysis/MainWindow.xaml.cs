using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SalesAnalysis.Classes;
using SalesAnalysis.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using General = SalesAnalysis.Classes.GeneralMethods;


namespace SalesAnalysis
{
    public delegate void LoadDataFromBD();

    /// <summary>
    /// Главное окно
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
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
                UpdateDataInTableForSelectedModel?.Invoke();
            }
        }
        private Model? _SelectedModel;

        #endregion


        #region Конструктор

        public MainWindow()
        {
            InitializeComponent();

            //CreateDbContextOptions();
            if (CheckConnect())
            {
                GetMonthsFromBd();

                UpdateDataInTableForAllModels += GetModelsFromBd;
                GetListYearsFromBd();

                UpdateDataInTableForAllModels += GetDatesSalesModelsFromDb;

                UpdateDataInTableForAllModels += ConvertDataFromBD;

                UpdateDataInTableForAllModels.Invoke();

                UpdateDataInTableForSelectedModel += GetDatesSalesModelsFromDb;
                UpdateDataInTableForSelectedModel += ConvertDataFromBD;
            }

            DataContext = this;
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
        /// Не используется, есть в MyDbContext
        /// </summary>
        private void CreateDbContextOptions()
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory());

            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            var connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();

            var DbContextOptions = optionsBuilder.UseSqlServer(connectionString).Options;
        }


        /// <summary>
        /// Проверка соединения с БД
        /// </summary>
        /// <returns></returns>
        public bool CheckConnect()
        {
            bool isConnect = false;
            string connectionString =
                    "Server=(localdb)\\MSSQLLocalDB;" +
                    "Database=BdForSalesAnalysis;" +
                    "Trusted_Connection=True;" +
                    "MultipleActiveResultSets=true";

            //using (MyDbContext db = new MyDbContext())
            //{
            //    var asdf = db.Database.GetDbConnection();
            //}
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.OpenAsync();
                //General.ShowNotificationMessageBox("Есть доступ к БД");
                isConnect = true;
            }
            catch (SqlException ex)
            {
                General.ShowNotificationMessageBox("Нет доступа к БД. Ошибка: " + ex.Message);
                isConnect = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.CloseAsync();
                }
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
            WindowForAddNewModel windowForAddNewModel = new();
            windowForAddNewModel.ShowDialog();

            if (windowForAddNewModel.IsSave)
            {
                using MyDbContext db = new MyDbContext();

                db.Models.Add(new Model(newNameModel: windowForAddNewModel.NameModel,
                                        newPriceModel: (double.TryParse(windowForAddNewModel.PriceModel, out double priceModel) ? priceModel : 0)));
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Получение списка моделей из БД
        /// </summary>
        public void GetModelsFromBd()
        {
            using (MyDbContext db = new ())
            {
                ListModels.Clear();

                List<Model> listModel = [];
                if(SelectedModel != null)
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
                    if(!ListYears.Any(x => x == year.Year))
                    {
                        ListYears.Add(year.Year);
                    }
                }
                if(ListYears.Count > 0)
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
        private void ButtonAddModel_Click(object sender, RoutedEventArgs e)
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
                if (SelectedModel == null)
                {
                    ListSalesModels.Add(new SalesModel(model));
                }
                else
                {
                    if(SelectedModel.IdModel == model.IdModel)
                    {
                        ListSalesModels.Add(new SalesModel(model));
                    }
                }
            }

            foreach (SalesModel salesModel in ListSalesModels)
            {
                foreach (DateSalesModel dateSalesModel in ListAllDatesSalesModels)
                {
                    var numberMonth = dateSalesModel.DateSaleModel.Month;

                    SalesByMonth salesByMonth = new SalesByMonth(newIdModel: dateSalesModel.IdModel,
                                                                 newNameModel: dateSalesModel.NameModel,
                                                                 newPriceModel: dateSalesModel.PriceModel,
                                                                 newMonth: ListMonths.Single(month => month.IdMonth == numberMonth),
                                                                 newDateSalesModels: dateSalesModel);

                    if (salesModel.IdModel == dateSalesModel.IdModel)
                    {
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
                    break;
                case 2:
                    salesModel.ListSaleForFebruary.Add(salesByMonth);
                    salesModel.TotalCostForFebruary = salesModel.TotalCostForFebruary + dateSalesModel.CostAllModelsSold;
                    break;
                case 3:
                    salesModel.ListSaleForMarch.Add(salesByMonth);
                    salesModel.TotalCostForMarch = salesModel.TotalCostForMarch + dateSalesModel.CostAllModelsSold;
                    break;
                case 4:
                    salesModel.ListSaleForApril.Add(salesByMonth);
                    salesModel.TotalCostForApril = salesModel.TotalCostForApril + dateSalesModel.CostAllModelsSold;
                    break;
                case 5:
                    salesModel.ListSaleForMay.Add(salesByMonth);
                    salesModel.TotalCostForMay = salesModel.TotalCostForMay + dateSalesModel.CostAllModelsSold;
                    break;
                case 6:
                    salesModel.ListSaleForJune.Add(salesByMonth);
                    salesModel.TotalCostForJune = salesModel.TotalCostForJune + dateSalesModel.CostAllModelsSold;
                    break;
                case 7:
                    salesModel.ListSaleForJuly.Add(salesByMonth);
                    salesModel.TotalCostForJuly = salesModel.TotalCostForJuly + dateSalesModel.CostAllModelsSold;
                    break;
                case 8:
                    salesModel.ListSaleForAugust.Add(salesByMonth);
                    salesModel.TotalCostForAugust = salesModel.TotalCostForAugust + dateSalesModel.CostAllModelsSold;
                    break;
                case 9:
                    salesModel.ListSaleForSeptember.Add(salesByMonth);
                    salesModel.TotalCostForSeptember = salesModel.TotalCostForSeptember + dateSalesModel.CostAllModelsSold;
                    break;
                case 10:
                    salesModel.ListSaleForOctober.Add(salesByMonth);
                    salesModel.TotalCostForOctober = salesModel.TotalCostForOctober + dateSalesModel.CostAllModelsSold;
                    break;
                case 11:
                    salesModel.ListSaleForNovember.Add(salesByMonth);
                    salesModel.TotalCostForNovember = salesModel.TotalCostForNovember + dateSalesModel.CostAllModelsSold;
                    break;
                case 12:
                    salesModel.ListSaleForDecember.Add(salesByMonth);
                    salesModel.TotalCostForDecember = salesModel.TotalCostForDecember + dateSalesModel.CostAllModelsSold;
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
        }

        /// <summary>
        /// Показать все модели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonShowAllModels_Click(object sender, RoutedEventArgs e)
        {
            SelectedModel = null;

            UpdateDataInTableForAllModels?.Invoke();
        }

        #endregion

    }
}