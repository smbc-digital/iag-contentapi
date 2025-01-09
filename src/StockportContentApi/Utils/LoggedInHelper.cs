namespace StockportContentApi.Utils;

public interface ILoggedInHelper
{
    LoggedInPerson GetLoggedInPerson();
}

[ExcludeFromCodeCoverage]
public class LoggedInHelper(IHttpContextAccessor httpContextAccessor,
                            IJwtDecoder decoder,
                            ILogger<LoggedInHelper> logger) : ILoggedInHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IJwtDecoder _decoder = decoder;
    private readonly ILogger<LoggedInHelper> _logger = logger;

    public LoggedInPerson GetLoggedInPerson()
    {
        LoggedInPerson person = new();

        try
        {
            Microsoft.Extensions.Primitives.StringValues token = _httpContextAccessor.HttpContext.Request.Headers["jwtCookie"];

            if (!string.IsNullOrEmpty(token)) person = _decoder.Decode(token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Exception thrown in GroupAuthorisation, {ex.Message}");
        }

        return person;
    }
}