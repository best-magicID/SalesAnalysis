using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SalesAnalysis.Classes;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using General = SalesAnalysis.Classes.GeneralMethods;


namespace SalesAnalysis
{
    /// <summary>
    /// Главное окно
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ПОЛЯ И СВОЙСТВА

        public ObservableCollection<Model> ListModels { get; set; } = [];

        #endregion

        #region Конструктор

        public MainWindow()
        {
            InitializeComponent();

            //CheckConnect();
            GetModels();

            DataContext = this;
        }

        #endregion

        #region МЕТОДЫ

        async Task CheckConnect()
        {
            string connectionString =
                    "Server=(localdb)\\MSSQLLocalDB;" +
                    "Database=BdForSalesAnalysis;" +
                    "Trusted_Connection=True;" +
                    "MultipleActiveResultSets=true";

            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                // Открываем подключение
                await connection.OpenAsync();
                General.ShowNotificationMessageBox("Подключение открыто");
            }
            catch (SqlException ex)
            {
                General.ShowNotificationMessageBox(ex.Message);
            }
            finally
            {
                // если подключение открыто
                if (connection.State == ConnectionState.Open)
                {
                    // закрываем подключение
                    await connection.CloseAsync();
                    General.ShowNotificationMessageBox("Подключение закрыто...");
                }
            }
        }

        
        public void GetModels()
        {
            using (MyDbContext db = new MyDbContext())
            {
                var models = db.Models.ToList();
                var dateSale = db.DateSale.ToList();

                foreach (Model model in models)
                {
                    ListModels.Add(new Model()
                    {
                        IdModel = model.IdModel,
                        NameModel = model.NameModel,
                        PriceModel = model.PriceModel
                    });

                }
            }
        }

        #endregion

    }
}