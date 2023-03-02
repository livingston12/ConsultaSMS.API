using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebConsultaSMS.DataBase;

namespace WebConsultaSMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            RunSeeding(host);
            RunIntialData(host);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        var env = hostingContext.HostingEnvironment;
                        if (env.IsDevelopment())
                        {
                            config.AddJsonFile(
                                $"appsettings.{env.EnvironmentName}.json",
                                optional: false,
                                reloadOnChange: true
                            );
                        }
                        else
                        {
                            config.AddJsonFile(
                                "appsettings.json",
                                optional: false,
                                reloadOnChange: true
                            );
                        }
                    }
                );

        private static void RunSeeding(IHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<SeedD>();
                seeder.SeedAsync().Wait();
            }
        }

        private static void RunIntialData(IHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<Utils.UtilsResponse>();
                seeder.GetConfiguration();
            }
        }
    }
}
