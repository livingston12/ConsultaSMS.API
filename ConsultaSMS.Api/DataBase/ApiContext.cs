using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WebConsultaSMS.Models.Entities;

namespace WebConsultaSMS.DataBase
{
    [System.Data.Entity.DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class ApiContext : DbContext
    {
        public IWebHostEnvironment Environment { get; }

        public ApiContext(
            DbContextOptions<ApiContext> options,
            IWebHostEnvironment environment = null
        ) : base(options)
        {
            Environment = environment;
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PhoneEntity> Phones { get; set; }
        public DbSet<RolEntity> Roles { get; set; }
        public DbSet<MediatorEntity> Mediators { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<TransactionLogEntity> TransactionLogs { get; set; }
        public DbSet<UserTransactionEntity> userTransactions { get; set; }
        public DbSet<GeneralParameterEntity> GeneralParameters { get; set; }
        public DbSet<RoleTransactionEntity> RoleTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Environment.EnvironmentName != "InMemory")
            {
                // Set appsetting depend of the enviroment
                string appSetting = Environment.IsDevelopment()
                    ? "appsettings.Development.json"
                    : "appsettings.json";

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(appSetting)
                    .Build();
                var connectionString = configuration.GetConnectionString("ConsultaSMSConnection");
                optionsBuilder.UseMySQL(connectionString);
            }

        }
    }
}
