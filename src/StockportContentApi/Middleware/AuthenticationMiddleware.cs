namespace StockportContentApi.Middleware;

[ExcludeFromCodeCoverage]
public class AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, IAuthenticationHelper authHelper)
{
    private readonly IAuthenticationHelper _authHelper = authHelper;
    private readonly IConfiguration _configuration = configuration;
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.Value.EndsWith("_healthcheck"))
        {
            await _next.Invoke(context);
            return;
        }

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