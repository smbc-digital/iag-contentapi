using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Services;
using StockportContentApi.Utils;
using Microsoft.Extensions.Caching.Distributed;
using StockportContentApi.Extensions;
using StockportContentApi.Middleware;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using StockportWebapp.Configuration;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Specialized;

namespace StockportContentApi
{
    public class Startup
    {
        private readonly string _contentRootPath;
        private readonly string _appEnvironment;
        private const string ConfigDir = "app-config";
        private readonly bool _useRedisSession;
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            _contentRootPath = env.ContentRootPath;

            var configBuilder = new ConfigurationBuilder();
            var configLoader = new ConfigurationLoader(configBuilder, ConfigDir);

            Configuration = configLoader.LoadConfiguration(env, _contentRootPath);
            _appEnvironment = configLoader.EnvironmentName(env);

            _useRedisSession = Configuration["UseRedisSessions"]?.ToLower() == "true";

            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "IAG Content Api")
                .WriteTo.LiterateConsole();

            // elastic search
            var esConfig = new ElasticSearch();
            Configuration.GetSection("ElasticSearch").Bind(esConfig);

            if (esConfig.Enabled && !string.IsNullOrEmpty(esConfig.Url) && !string.IsNullOrEmpty(esConfig.Authorization))
            {
                loggerConfig.WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(esConfig.Url))
                    {
                        AutoRegisterTemplate = true,
                        MinimumLogEventLevel = esConfig.LogLevel,
                        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                        IndexFormat = esConfig.LogFormat,
                        ModifyConnectionSettings = (c) => c.GlobalHeaders(new NameValueCollection
                        {
                            {"Authorization", esConfig.Authorization}
                        })
                    });
            }

            Log.Logger = loggerConfig.CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // logging
            var loggerFactory = new LoggerFactory().AddSerilog();
            ILogger logger = loggerFactory.CreateLogger<Startup>();

            // add other services
            services.AddCache(_useRedisSession);
            services.AddSingleton(new ButoConfig(Configuration["ButoBaseUrl"]));
            services.AddSingleton<IHttpClient>(p => new LoggingHttpClient(new HttpClient(new MsHttpClientWrapper(), p.GetService<ILogger<HttpClient>>()), p.GetService<ILogger<LoggingHttpClient>>()));
            services.AddTransient<IHealthcheckService>(p => new HealthcheckService($"{_contentRootPath}/version.txt", $"{_contentRootPath}/sha.txt", new FileWrapper(), _appEnvironment, p.GetService<ICache>()));
            services.AddTransient<ResponseHandler>();
            services.AddSingleton<ITimeProvider>(new TimeProvider());
            services.AddSingleton<IConfiguration>(Configuration);

            // add service extensions

            if (_appEnvironment == "local")
            {
                services.AddRedisLocal(Configuration, _useRedisSession, logger);
            }
            else
            {
                services.AddRedis(Configuration, _useRedisSession, logger);
            }
            
            services.AddRedirects(Configuration);
            services.AddContentfulConfig(Configuration);
            services.AddOptions();
            services.AddContentfulClients();
            services.AddContentfulFactories();
            services.AddRepositories();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();
            services.AddAutoMapper();
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDistributedCache cache, IApplicationLifetime appLifetime)
        {
            // add logging
            loggerFactory.AddSerilog();

            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();
            app.UseStaticFiles();
            app.UseMvc();

            // close logger
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }
    }
}