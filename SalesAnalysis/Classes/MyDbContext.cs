using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesAnalysis.Classes
{
    class MyDbContext : DbContext
    {
        public DbSet<Model> Models { get; set; } = null!;

        public DbSet<DateSale> DateSale { get; set; } = null!;

        // Конфигурация подключения (для консольного приложения)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString =
                    "Server=(localdb)\\MSSQLLocalDB;" +
                    "Database=BdForSalesAnalysis;" +
                    "Trusted_Connection=True;" +
                    "MultipleActiveResultSets=true";

                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
