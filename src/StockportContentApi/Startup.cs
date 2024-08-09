using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApi;

public class Startup
{
    private readonly string _contentRootPath;
    private readonly string _appEnvironment;
    private readonly bool _useRedisSession;
    private readonly bool _useLocalCache;
    private readonly Serilog.ILogger _logger;
    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration, IHostEnvironment env, Serilog.ILogger logger)
    {
        Configuration = configuration;
        _contentRootPath = env.ContentRootPath;
        _appEnvironment = env.EnvironmentName;
        _useRedisSession = Configuration["UseRedisSessions"].Equals("true", StringComparison.OrdinalIgnoreCase);
        _useLocalCache = Configuration["UseLocalCache"].Equals("true", StringComparison.OrdinalIgnoreCase);
        _logger = logger;
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Env = {_appEnvironment}, UseRedisSession = {_useRedisSession}, UseLocalCache = {_useLocalCache}, ContentRoot = {_contentRootPath}  ");

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Controllers");
        services.AddControllers().AddNewtonsoftJson();

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Environment {_appEnvironment}");
        services.AddSingleton(new CurrentEnvironment(_appEnvironment));
        
        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Cache");
        services.AddCache(_useRedisSession, _appEnvironment, Configuration, _logger, _useLocalCache);

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding TwenetyThreeConfig");
        services.AddSingleton(new TwentyThreeConfig(Configuration["TwentyThreeBaseUrl"]));

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding HTTP Clients");
        services.AddSingleton<IHttpClient>(p => new LoggingHttpClient(new HttpClient(new MsHttpClientWrapper(), p.GetService<ILogger<HttpClient>>()), p.GetService<ILogger<LoggingHttpClient>>()));

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Healthchecks");
        services.AddTransient<IHealthcheckService>(p => new HealthcheckService($"{_contentRootPath}/version.txt", $"{_contentRootPath}/sha.txt", new FileWrapper(), _appEnvironment, p.GetService<ICache>()));
        
        services.AddTransient<ResponseHandler>();
        services.AddSingleton<ITimeProvider>(new TimeProvider());

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Base Configuration");
        services.AddSingleton<IConfiguration>(Configuration);

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Add HTTP Context Accessor");
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Short URL Redirects");
        services.AddSingleton(_ => new ShortUrlRedirects(new Dictionary<string, RedirectDictionary>()));

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Legacy URL Redirects");
        services.AddSingleton(_ => new LegacyUrlRedirects(new Dictionary<string, RedirectDictionary>()));

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Group Configuration");
        services.AddGroupConfiguration(Configuration, _logger);
        services.AddHelpers();
        services.AddRedirects(Configuration);
        services.AddContentfulConfig(Configuration);
        services.AddOptions();

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Contentful Clients");
        services.AddContentfulClients();

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Contentful Factories");
        services.AddContentfulFactories();

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Respositories");
        services.AddRepositories();

        services.AddAutoMapper();
        services.AddServices();
        services.AddBuilders();

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Configuring RedisExpiryConfiguration");
        services.Configure<RedisExpiryConfiguration>(Configuration.GetSection("redisExpiryTimes"));

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Configuring Swagger");
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stockport Content API", Version = "v1" });
            c.DocumentFilter<SwaggerFilter>();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                In = ParameterLocation.Header,
                Description = "Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\".",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new List<string>()
                }
            });

        });

        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : COMPLETED");
    }
}