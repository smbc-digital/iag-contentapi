using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Services;
using StockportContentApi.Utils;
using Microsoft.Extensions.Caching.Distributed;
using StockportContentApi.Extensions;
using StockportContentApi.Middleware;

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
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // add other services
            services.AddSingleton(new ButoConfig(Configuration["ButoBaseUrl"]));
            services.AddSingleton<IHttpClient>(p => new LoggingHttpClient(new HttpClient(new MsHttpClientWrapper(), p.GetService<ILogger<HttpClient>>()), p.GetService<ILogger<LoggingHttpClient>>()));
            services.AddTransient<IHealthcheckService>(p => new HealthcheckService($"{_contentRootPath}/version.txt", $"{_contentRootPath}/sha.txt", new FileWrapper(), _appEnvironment));
            services.AddTransient<ResponseHandler>();
            services.AddSingleton<ITimeProvider>(new TimeProvider());
            services.AddSingleton<IConfiguration>(Configuration);

            // add service extensions
            services.AddRedis(Configuration, _useRedisSession);
            services.AddRedirects(Configuration);
            services.AddContentfulConfig(Configuration);
            services.AddOptions();
            services.AddContentfulClients();
            services.AddContentfulFactories();
            services.AddCache(_useRedisSession);
            services.AddRepositories();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();
            services.AddAutoMapper();
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDistributedCache cache)
        {
            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();
            app.UseStaticFiles();
            app.UseMvc();

            loggerFactory.AddNLog();
        }
    }
}