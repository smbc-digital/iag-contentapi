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



        services.AddControllers().AddNewtonsoftJson();
        services.AddSingleton(new CurrentEnvironment(_appEnvironment));
        
        _logger.Information($"CONTENTAPI: STARTUP : ConfigureServices : Adding Cache");
        services.AddCache(_useRedisSession, _appEnvironment, Configuration, _logger, _useLocalCache);
        services.AddSingleton(new TwentyThreeConfig(Configuration["TwentyThreeBaseUrl"]));
        services.AddSingleton<IHttpClient>(p => new LoggingHttpClient(new HttpClient(new MsHttpClientWrapper(), p.GetService<ILogger<HttpClient>>()), p.GetService<ILogger<LoggingHttpClient>>()));
        services.AddTransient<IHealthcheckService>(p => new HealthcheckService($"{_contentRootPath}/version.txt", $"{_contentRootPath}/sha.txt", new FileWrapper(), _appEnvironment, p.GetService<ICache>()));
        services.AddTransient<ResponseHandler>();
        services.AddSingleton<ITimeProvider>(new TimeProvider());
        services.AddSingleton<IConfiguration>(Configuration);
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddSingleton(_ => new ShortUrlRedirects(new Dictionary<string, RedirectDictionary>()));
        services.AddSingleton(_ => new LegacyUrlRedirects(new Dictionary<string, RedirectDictionary>()));

        services.AddGroupConfiguration(Configuration, _logger);
        services.AddHelpers();
        services.AddRedirects(Configuration);
        services.AddContentfulConfig(Configuration);
        services.AddOptions();
        services.AddContentfulClients();
        services.AddContentfulFactories();
        services.AddRepositories();
        services.AddAutoMapper();
        services.AddServices();
        services.AddBuilders();
        services.Configure<RedisExpiryConfiguration>(Configuration.GetSection("redisExpiryTimes"));
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
    }
}