using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using SalesAnalysis.Classes;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Azure.Core.HttpHeader;
using General = SalesAnalysis.Classes.GeneralMethods;


namespace SalesAnalysis
{
    /// <summary>
    /// Главное окно
    /// </summary>
    public partial class MainWindow : Window
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

        public ObservableCollection<SalesModel> ListSalesModels { get; set; } = [];

        #endregion


        #region Конструктор

        public MainWindow()
        {
            InitializeComponent();

            //CreateDbContextOptions();
            //CheckConnect();

            GetMonthsFromBd();
            GetModelsFromBd();

            GetDatesSalesModelsFromDb();

            Convert();

            DataContext = this;
        }

        #endregion


        #region МЕТОДЫ

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
        async Task CheckConnect()
        {
            string connectionString =
                    "Server=(localdb)\\MSSQLLocalDB;" +
                    "Database=BdForSalesAnalysis;" +
                    "Trusted_Connection=True;" +
                    "MultipleActiveResultSets=true";

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();
                General.ShowNotificationMessageBox("Подключение открыто");
            }
            catch (SqlException ex)
            {
                General.ShowNotificationMessageBox(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                    General.ShowNotificationMessageBox("Подключение закрыто...");
                }
            }
        }

        /// <summary>
        /// Добавдение новой модели в БД
        /// </summary>
        /// <param name="name"></param>
        /// <param name="price"></param>
        public void AddModelInBd(string name, double price)
        {
            using MyDbContext db = new MyDbContext();

            db.Models.Add(new Model(newIdModel: -1,
                                    newNameModel: name,
                                    newPriceModel: price));
            db.SaveChanges();
        }

        /// <summary>
        /// Получение списка моделей из БД
        /// </summary>
        public void GetModelsFromBd()
        {
            using (MyDbContext db = new MyDbContext())
            {
                ListModels.Clear();

                var models = db.Models.ToList();

                foreach (Model model in models)
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
        /// Кнопка добавления новой модели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddModel_Click(object sender, RoutedEventArgs e)
        {
            AddModelInBd("Новая модель", 600);
            GetModelsFromBd();
        }

        /// <summary>
        /// Получение дат продаж моделей из БД
        /// </summary>
        public void GetDatesSalesModelsFromDb()
        {
            using (MyDbContext db = new MyDbContext())
            {
                var data = (from tModels in db.Models
                           join tDatesSale in db.DatesSale on tModels.IdModel equals tDatesSale.IdModel
                           select new { tModels.IdModel,
                                        tModels.NameModel,
                                        tModels.PriceModel,
                                        tDatesSale.IdDateSale,
                                        tDatesSale.DateSaleModel,
                                        tDatesSale.CountSoldModels }).ToList();

                int i = 1;
                foreach (var item in data)
                {
                    ListAllDatesSalesModels.Add(new (newIndexNumber: i++,
                                                     newIdModel: item.IdModel,
                                                     newNameModel: item.NameModel,
                                                     newPriceModel: item.PriceModel,
                                                     newIdDateSale: item.IdDateSale,
                                                     newDateSaleModel: item.DateSaleModel,
                                                     newCountSoldModels: item.CountSoldModels));
                }
            }
        }

        public void Convert ()
        {
            foreach (Model model in ListModels)
            { 
                ListSalesModels.Add(new SalesModel(model));
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

                        //switch (numberMonth)
                        //{
                        //    case 1:
                                
                        //        salesModel.ListSaleForJanuary.Add(salesByMonth);
                        //        salesModel.TotalCostForJanuary = salesModel.TotalCostForJanuary + (dateSalesModel.CostAllModelsSold);
                        //        break;
                        //    case 2:
                        //        salesModel.ListSaleForFebruary.Add(salesByMonth);
                        //        salesModel.TotalCostForFebruary = salesModel.ListSaleForFebruary.Count * salesModel.PriceModel;
                        //        break;
                        //    case 3:
                        //        salesModel.ListSaleForMarch.Add(salesByMonth);
                        //        salesModel.TotalCostForMarch = salesModel.ListSaleForMarch.Count * salesModel.PriceModel;
                        //        break;
                        //    case 4:
                        //        salesModel.ListSaleForApril.Add(salesByMonth);
                        //        salesModel.TotalCostForApril = salesModel.ListSaleForApril.Count * salesModel.PriceModel;
                        //        break;
                        //    case 5:
                        //        salesModel.ListSaleForMay.Add(salesByMonth);
                        //        salesModel.TotalCostForMay = salesModel.ListSaleForMay.Count * salesModel.PriceModel;
                        //        break;
                        //    case 6:
                        //        salesModel.ListSaleForJune.Add(salesByMonth);
                        //        salesModel.TotalCostForJune = salesModel.ListSaleForJune.Count * salesModel.PriceModel;
                        //        break;
                        //    case 7:
                        //        salesModel.ListSaleForJuly.Add(salesByMonth);
                        //        salesModel.TotalCostForJuly = salesModel.ListSaleForJuly.Count * salesModel.PriceModel;
                        //        break;
                        //    case 8:
                        //        salesModel.ListSaleForAugust.Add(salesByMonth);
                        //        salesModel.TotalCostForAugust = salesModel.ListSaleForAugust.Count * salesModel.PriceModel;
                        //        break;
                        //    case 9:
                        //        salesModel.ListSaleForSeptember.Add(salesByMonth);
                        //        salesModel.TotalCostForSeptember = salesModel.ListSaleForSeptember.Count * salesModel.PriceModel;
                        //        break;
                        //    case 10:
                        //        salesModel.ListSaleForOctober.Add(salesByMonth);
                        //        salesModel.TotalCostForOctober = salesModel.ListSaleForOctober.Count * salesModel.PriceModel;
                        //        break;
                        //    case 11:
                        //        salesModel.ListSaleForNovember.Add(salesByMonth);
                        //        salesModel.TotalCostForNovember = salesModel.ListSaleForNovember.Count * salesModel.PriceModel;
                        //        break;
                        //    case 12:
                        //        salesModel.ListSaleForDecember.Add(salesByMonth);
                        //        salesModel.TotalCostForDecember = salesModel.ListSaleForDecember.Count * salesModel.PriceModel;
                        //        break;
                        //}
                    }

                }
            }
        }

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

        #endregion

    }
}