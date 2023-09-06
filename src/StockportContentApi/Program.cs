Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Configuration.SetBasePath(builder.Environment.ContentRootPath + "/app-config");
    builder.Configuration
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json");

    builder.AddSecrets();

    Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                    .CreateLogger();

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteToElasticsearchAws(builder.Configuration));

    var startup = new Startup(builder.Configuration, builder.Environment, Log.Logger);
    startup.ConfigureServices(builder.Services);

    var app = builder.Build();

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

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}