using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace StockportContentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://0.0.0.0:5001")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureKestrel((context, options) =>
                {
                    // Set properties and call methods on options
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.Sources.Clear();
                    config.SetBasePath(Directory.GetCurrentDirectory() + "/app-config");
                    config
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json");
                    var tempConfig = config.Build();
                    config.AddJsonFile(
                        $"{tempConfig["secrets-location"]}/appsettings.{hostContext.HostingEnvironment.EnvironmentName}.secrets.json");
                });
            });
                
    }
}
