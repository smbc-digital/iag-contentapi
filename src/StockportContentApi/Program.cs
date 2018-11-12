﻿using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StockportContentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://0.0.0.0:5001")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.Sources.Clear();
                    config.SetBasePath(Directory.GetCurrentDirectory() + "/app-config");
                    config
                        .AddJsonFile("appsettings.json", true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                    var tempConfig = config.Build();
                    config.AddJsonFile(
                        $"{tempConfig["secrets-location"]}/appsettings.{hostContext.HostingEnvironment.EnvironmentName}.secrets.json",
                        true);
                });
    }
}
