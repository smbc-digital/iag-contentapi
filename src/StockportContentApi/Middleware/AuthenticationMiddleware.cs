namespace StockportContentApi.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationMiddleware> _logger;
    private readonly IAuthenticationHelper _authHelper;
    private readonly Func<string, ContentfulConfig> _createConfig;

    public AuthenticationMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<AuthenticationMiddleware> logger,
        IAuthenticationHelper authHelper,
        Func<string, ContentfulConfig> createConfig)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
        _authHelper = authHelper;
        _createConfig = createConfig;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.Value.EndsWith("_healthcheck"))
            await _next.Invoke(context);

        string apiConfigurationkey = _configuration["Authorization"] ?? string.Empty;

        if (string.IsNullOrEmpty(apiConfigurationkey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("API Authentication Key is missing from the config");
            return;
        }

        AuthenticationData authenticationData = _authHelper.ExtractAuthenticationDataFromContext(context);

        if (string.IsNullOrEmpty(authenticationData.AuthenticationKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("API Authentication Key is either missing or wrong.");
            return;
        }

        if (!authenticationData.AuthenticationKey.Equals(apiConfigurationkey))
        {
            try
            {
                _authHelper.CheckVersionIsProvided(authenticationData);
            }
            catch (AuthorizationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("API Authentication Key is either missing or wrong-");
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("API Authentication Key is either missing or wrong.");
            return;
        }

        await _next.Invoke(context);
    }
}