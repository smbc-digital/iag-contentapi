using TimeProvider = StockportContentApi.Utils.TimeProvider;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    if (!builder.Environment.EnvironmentName.Equals("local"))
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("C:\\Program Files\\Amazon\\ElasticBeanstalk\\logs\\ContentAPIStartupLog.log")
            .CreateBootstrapLogger();

    Log.Logger.Information($"CONTENTAPI : ENVIRONMENT : {builder.Environment.EnvironmentName}");

    builder.Configuration.SetBasePath($"{builder.Environment.ContentRootPath}/app-config");
    builder.Configuration
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json");

    bool useAwsSecretManager = bool.Parse(builder.Configuration.GetSection("UseAWSSecretManager").Value);

    Log.Logger.Information($"CONTENTAPI : ENVIRONMENT : {builder.Environment.EnvironmentName}");

    if (useAwsSecretManager)
    {
        builder.AddSecrets();
        Log.Logger.Information($"CONTENTAPI : INITIALISE SECRETS  {builder.Environment.EnvironmentName} : AWS Secrets Manager");
    }
    else
    {
        string location = $"{builder.Configuration.GetSection("secrets-location").Value}/appsettings.{builder.Environment.EnvironmentName}.secrets.json";
        builder.Configuration.AddJsonFile(location);
        Log.Logger.Information($"CONTENTAPI : INITIALISE SECRETS {builder.Environment.EnvironmentName}: Load JSON Secrets from file system, {location}");
    }

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteToOpenSearchAws(builder.Configuration)
        .CreateLogger();

    builder.Logging.ClearProviders();
    builder.Services.AddSerilog(Log.Logger);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteToOpenSearchAws(builder.Configuration));

    string appEnvironment = builder.Environment.EnvironmentName;
    string contentRootPath = builder.Environment.ContentRootPath;
    bool useRedisSession = builder.Configuration["UseRedisSessions"].Equals("true", StringComparison.OrdinalIgnoreCase);
    bool useLocalCache = builder.Configuration["UseLocalCache"].Equals("true", StringComparison.OrdinalIgnoreCase);

    Log.Logger.Information(
        $"CONTENTAPI: STARTUP : ConfigureServices : Env = {appEnvironment}, UseRedisSession = {useRedisSession}, UseLocalCache = {useLocalCache}, ContentRoot = {contentRootPath}");

    builder.Services.AddControllers().AddNewtonsoftJson();
    builder.Services.AddSingleton(new CurrentEnvironment(appEnvironment));
    builder.Services.AddCache(useRedisSession, appEnvironment, builder.Configuration, Log.Logger, useLocalCache);
    builder.Services.AddSingleton(new TwentyThreeConfig(builder.Configuration["TwentyThreeBaseUrl"]));

    builder.Services.AddSingleton<IHttpClient>(p =>
        new LoggingHttpClient(new HttpClient(new MsHttpClientWrapper(), p.GetService<ILogger<HttpClient>>()),
            p.GetService<ILogger<LoggingHttpClient>>()));

    builder.Services.AddTransient<IHealthcheckService>(p => new HealthcheckService($"{contentRootPath}/version.txt",
        $"{contentRootPath}/sha.txt", new FileWrapper(), appEnvironment));

    builder.Services.AddTransient<ResponseHandler>();
    builder.Services.AddSingleton<ITimeProvider>(new TimeProvider());
    builder.Services.AddSingleton(builder.Configuration);
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddSingleton(_ => new ShortUrlRedirects(new()));
    builder.Services.AddSingleton(_ => new LegacyUrlRedirects(new()));

    builder.Services.AddRedirects(builder.Configuration);
    builder.Services.AddContentfulConfig(builder.Configuration);
    builder.Services.AddCacheKeyConfig(builder.Configuration);
    builder.Services.AddOptions();
    builder.Services.AddContentfulClients();
    builder.Services.AddContentfulFactories();
    builder.Services.AddRepositories();
    builder.Services.AddAutoMapper();
    builder.Services.AddServices();
    builder.Services.AddBuilders();

    builder.Services.Configure<RedisExpiryConfiguration>(builder.Configuration.GetSection("redisExpiryTimes"));

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Stockport Content API", Version = "v1" });
        c.DocumentFilter<SwaggerFilter>();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            In = ParameterLocation.Header,
            Description = "Authorization using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
        });

        c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    });

    Log.Logger.Information($"CONTENTAPI : BUILDING APPLICATION");
    WebApplication app = builder.Build();

    if (!app.Environment.IsEnvironment("prod") && !app.Environment.IsEnvironment("stage"))
        app.UseDeveloperExceptionPage();

    app.UseSerilogRequestLogging();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(app.Environment.IsEnvironment("local") ?
            "/swagger/v1/swagger.json" :
            "/api/swagger/v1/swagger.json",
            "Stockport Content API");
    });

    app.UseMiddleware<AuthenticationMiddleware>();
    app.UseRouting();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CONTENTAPI : FAILURE : Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

[ExcludeFromCodeCoverage]
public partial class Program { }
