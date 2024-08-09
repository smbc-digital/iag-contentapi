try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    if(!builder.Environment.EnvironmentName.Equals("local"))
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("C:\\Program Files\\Amazon\\ElasticBeanstalk\\logs\\ContentAPIStartupLog.log")
            .CreateBootstrapLogger();

    Log.Logger.Information($"CONTENTAPI : ENVIRONMENT : {builder.Environment.EnvironmentName}");

    builder.Configuration.SetBasePath(builder.Environment.ContentRootPath + "/app-config");
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
        string location  = $"{builder.Configuration.GetSection("secrets-location").Value}/appsettings.{builder.Environment.EnvironmentName}.secrets.json";
        builder.Configuration.AddJsonFile(location);
        Log.Logger.Information($"CONTENTAPI : INITIALISE SECRETS {builder.Environment.EnvironmentName}: Load JSON Secrets from file system, {location}");
    }

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteToElasticsearchAws(builder.Configuration));

    Log.Logger.Information($"CONTENTAPI : CONFIGURE APPLICATION START");
    Startup startup = new(builder.Configuration, builder.Environment, Log.Logger);
    startup.ConfigureServices(builder.Services);
    
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
    app.UseStaticFiles();
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

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