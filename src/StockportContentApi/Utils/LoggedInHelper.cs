﻿namespace StockportContentApi.Utils;

public interface ILoggedInHelper
{
    LoggedInPerson GetLoggedInPerson();
}

public class LoggedInHelper : ILoggedInHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtDecoder _decoder;
    private readonly ILogger<LoggedInHelper> _logger;

    public LoggedInHelper(IHttpContextAccessor httpContextAccessor, IJwtDecoder decoder, ILogger<LoggedInHelper> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _decoder = decoder;
        _logger = logger;
    }

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
