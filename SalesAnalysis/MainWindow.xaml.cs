using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
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

                if (SelectedModel != null)
                {
                    UpdateDataInTableForSelectedModel?.Invoke();
                }
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

                //CreateRecordsInBd();
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
        /// Старая Проверка соединения с БД
        /// </summary>
        /// <returns></returns>
        public bool OldCheckConnect()
        {
            bool isConnect = false;
            string connectionString =
                    "Server=(localdb)\\MSSQLLocalDB;" +
                    "Database=BdForSalesAnalysis;" +
                    "Trusted_Connection=True;" +
                    "MultipleActiveResultSets=true";

            using (MyDbContext db = new MyDbContext())
            {
                var asdf = db.Database.CanConnect();
            }
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.OpenAsync();
                //General.ShowNotificationMessageBox("Есть доступ к БД");
                isConnect = true;
            }
            catch (SqlException ex)
            {
                General.ShowNotification("Нет доступа к БД. Ошибка: " + ex.Message);
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
                General.ShowNotification("Нет доступа к БД. Ошибка: " + ex.Message);
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

        public void DeleteModelInBd(string text, Model idModel)
        {
            if (General.ShowSelectionWindow(text) == MessageBoxResult.Yes)
            {
                using MyDbContext db = new MyDbContext();

                db.Models.Remove(idModel);
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

            salesModel.TotalAmountForYear = salesModel.ListSaleForJanuary.Count +
                                            salesModel.ListSaleForFebruary.Count +
                                            salesModel.ListSaleForMarch.Count +
                                            salesModel.ListSaleForApril.Count +
                                            salesModel.ListSaleForMay.Count +
                                            salesModel.ListSaleForJune.Count +
                                            salesModel.ListSaleForJuly.Count +
                                            salesModel.ListSaleForAugust.Count +
                                            salesModel.ListSaleForSeptember.Count +
                                            salesModel.ListSaleForOctober.Count +
                                            salesModel.ListSaleForNovember.Count +
                                            salesModel.ListSaleForDecember.Count;
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

        /// <summary>
        /// Удаление модели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteModel_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedModel != null)
            {
                string text = "Удалить: " + SelectedModel.NameModel;
                DeleteModelInBd(text, SelectedModel);

                SelectedModel = null;
                UpdateDataInTableForAllModels?.Invoke();
            }
        }

        private void ButtonSaveToExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveToExcel1();
        }

        public void SaveToExcel1()
        {
            try
            {
                var nameFile = "Продажи " + DateTime.Now.ToShortDateString() + ".xlsx";

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel.xlsx|*.xlsx";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = nameFile;
                saveFileDialog.ShowDialog();

                using (SpreadsheetDocument document = SpreadsheetDocument.Create(saveFileDialog.FileName, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Лист1"
                    };
                    sheets.Append(sheet);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>() ?? new SheetData();

                    sheetData.AppendChild(CreateRowHeader(1));

                    int i = 2;
                    foreach (SalesModel salesModel in ListSalesModels)
                    {
                        sheetData.AppendChild(CreateRow(i, salesModel));
                        i++;
                    }

                    workbookPart.Workbook.Save();

                    General.ShowNotification("Экспорт в Excel завершен.");
                }
            }
            catch (Exception ex)
            {
                General.ShowNotification(ex.Message);
            }
        }

        /// <summary>
        /// Создание ячейки
        /// </summary>
        /// <param name="numberColumn"></param>
        /// <param name="row"></param>
        /// <param name="valueCell"></param>
        /// <param name="typeCell"></param>
        /// <returns></returns>
        public Cell CreateCell(string numberColumn, Row row, string valueCell, CellValues typeCell)
        {
            Cell cell = new();

            var numberRow = int.TryParse(row.RowIndex, out int result) ? result : 0;
            cell.CellReference = numberColumn.ToString() + numberRow.ToString();

            if (typeCell == CellValues.Number)
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
            }
            else
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
            }
            cell.CellValue = new CellValue(valueCell);

            return cell;    
        }

        /// <summary>
        /// Создание строки заголовка
        /// </summary>
        /// <param name="indexRow"></param>
        /// <returns></returns>
        public Row CreateRowHeader(int indexRow)
        {
            Row row = new Row() { RowIndex = Convert.ToUInt32(indexRow) };

            row.AppendChild(CreateCell("A", row, "Порядковый номер", CellValues.String));
            row.AppendChild(CreateCell("B", row, "Id", CellValues.String));
            row.AppendChild(CreateCell("C", row, "Название", CellValues.String));

            row.AppendChild(CreateCell("D", row, "Январь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("E", row, "Январь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("F", row, "Февраль (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("G", row, "Февраль (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("H", row, "Март (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("I", row, "Март (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("J", row, "Апрель (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("K", row, "Апрель (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("L", row, "Май (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("M", row, "Май (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("N", row, "Июнь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("O", row, "Июнь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("P", row, "Июль (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("Q", row, "Июль (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("R", row, "Август (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("S", row, "Август (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("T", row, "Сентябрь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("U", row, "Сентябрь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("V", row, "Октябрь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("W", row, "Октябрь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("X", row, "Ноябрь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("Y", row, "Ноябрь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("Z", row, "Декабрь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("AA", row, "Декабрь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("AB", row, "Год (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("AC", row, "Год (общая стоимость)", CellValues.String));

            return row;
        }

        public Row CreateRow(int indexRow, SalesModel salesModel)
        {
            Row row = new Row() { RowIndex = Convert.ToUInt32(indexRow) };

            row.AppendChild(CreateCell("A", row, (indexRow - 1).ToString(), CellValues.Number));
            row.AppendChild(CreateCell("B", row, salesModel.IdModel.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("C", row, salesModel.GetNameModel(), CellValues.String));

            row.AppendChild(CreateCell("D", row, salesModel.ListSaleForJanuary.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("E", row, salesModel.TotalCostForJanuary.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("F", row, salesModel.ListSaleForFebruary.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("G", row, salesModel.TotalCostForFebruary.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("H", row, salesModel.ListSaleForMarch.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("I", row, salesModel.TotalCostForMarch.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("J", row, salesModel.ListSaleForApril.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("K", row, salesModel.TotalCostForApril.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("L", row, salesModel.ListSaleForMay.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("M", row, salesModel.TotalCostForMay.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("N", row, salesModel.ListSaleForJune.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("O", row, salesModel.TotalCostForJune.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("P", row, salesModel.ListSaleForJuly.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("Q", row, salesModel.TotalCostForJuly.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("R", row, salesModel.ListSaleForAugust.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("S", row, salesModel.TotalCostForAugust.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("T", row, salesModel.ListSaleForSeptember.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("U", row, salesModel.TotalCostForSeptember.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("V", row, salesModel.ListSaleForOctober.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("W", row, salesModel.TotalCostForOctober.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("X", row, salesModel.ListSaleForNovember.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("Y", row, salesModel.TotalCostForNovember.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("Z", row, salesModel.ListSaleForDecember.Count.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("AA", row, salesModel.TotalCostForDecember.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("AB", row, salesModel.TotalAmountForYear.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("AC", row, salesModel.TotalCostForYear.ToString(), CellValues.Number));

            return row;
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

    }
}