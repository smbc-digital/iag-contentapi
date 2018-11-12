using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Services;
using StockportContentApi.Utils;
using StockportContentApi.Extensions;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using StockportContentApi.Middleware;
using Swashbuckle.Swagger.Model;

namespace StockportContentApi
{
    public class Startup
    {
        private readonly string _contentRootPath;
        private readonly string _appEnvironment;
        private readonly bool _useRedisSession;
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _contentRootPath = env.ContentRootPath;
            _appEnvironment = env.EnvironmentName;
            _useRedisSession = Configuration["UseRedisSessions"]?.ToLower() == "true";
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigureSerilog();

            var loggerFactory = new LoggerFactory().AddSerilog();
            ILogger logger = loggerFactory.CreateLogger<Startup>();

            // add other services
            services.AddSingleton(new CurrentEnvironment(_appEnvironment));
            services.AddCache(_useRedisSession);
            services.AddSingleton(new ButoConfig(Configuration["ButoBaseUrl"]));
            services.AddSingleton<IHttpClient>(p => new LoggingHttpClient(new HttpClient(new MsHttpClientWrapper(), p.GetService<ILogger<HttpClient>>()), p.GetService<ILogger<LoggingHttpClient>>()));
            services.AddTransient<IHealthcheckService>(p => new HealthcheckService($"{_contentRootPath}/version.txt", $"{_contentRootPath}/sha.txt", new FileWrapper(), _appEnvironment, p.GetService<ICache>()));
            services.AddTransient<ResponseHandler>();
            services.AddSingleton<ITimeProvider>(new TimeProvider());
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));
            services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            // add service extensions

            if (_appEnvironment == "local")
            {
                services.AddRedisLocal(Configuration, _useRedisSession, logger);
            }
            else
            {
                services.AddRedis(Configuration, _useRedisSession, logger);
            }

            services.AddGroupConfiguration(Configuration, logger);
            services.AddHelpers();
            services.AddRedirects(Configuration);
            services.AddContentfulConfig(Configuration);
            services.AddOptions();
            services.AddContentfulClients();
            services.AddContentfulFactories();
            services.AddRepositories();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();
            services.AddAutoMapper();
            services.AddServices();
            services.AddBuilders();
            services.AddSwaggerGen(c =>
            {
                c.SingleApiVersion(new Info { Title = "Stockport Content API", Version = "v1" });

                c.DocumentFilter<SwaggerFilter>();

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    Description = "Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\".",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

            });
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDistributedCache cache, IApplicationLifetime appLifetime)
        {
            // add logging
            loggerFactory.AddSerilog();

            // swagger
            app.UseSwagger();
            app.UseSwaggerUi(swaggerUrl: _appEnvironment == "local" ? "/swagger/v1/swagger.json" : "/api/swagger/v1/swagger.json");

            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseClientRateLimiting();
            
            app.UseStaticFiles();
            app.UseMvc();

            // close logger
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }
       
        private void ConfigureSerilog()
        {
            var logConfig = new LoggerConfiguration()
                .ReadFrom
                .Configuration(Configuration);

            var esLogConfig = new ElasticSearchLogConfigurator(Configuration);
            esLogConfig.Configure(logConfig);

            Log.Logger = logConfig.CreateLogger();
            Log.Logger.Information("Completed logging configuration...");
        }
    }
}