using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using SalesAnalysis.Models;

namespace SalesAnalysis.Data
{
    /// <summary>
    /// Датаконтекст БД
    /// </summary>
    public class MyDbContext : DbContext
    {
        public DbSet<Model> Models { get; set; } = null!;

        public DbSet<DateSale> DatesSale { get; set; } = null!;

        public DbSet<Month> Months { get; set; } = null!;


        //public MyDbContext(DbContextOptions<MyDbContext> options) 
        //    : base(options)
        //{
        //    //Database.EnsureCreated();
        //}

        // Конфигурация подключения
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder();

                builder.SetBasePath(Directory.GetCurrentDirectory());

                builder.AddJsonFile("appsettings.json");
                // создаем конфигурацию
                var config = builder.Build();
                // получаем строку подключения
                var connectionString = config.GetConnectionString("DefaultConnection");

                //string connectionString =
                //    "Server=(localdb)\\MSSQLLocalDB;" +
                //    "Database=BdForSalesAnalysis;" +
                //    "Trusted_Connection=True;" +
                //    "MultipleActiveResultSets=true";

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

    }
}
