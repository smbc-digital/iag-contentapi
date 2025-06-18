namespace StockportContentApi.Utils;

public interface IAuthenticationHelper
{
    AuthenticationData ExtractAuthenticationDataFromContext(HttpContext context);
    string GetApiEndPoint(string endpoint);
    void CheckVersionIsProvided(AuthenticationData authenticationData);
}

[ExcludeFromCodeCoverage]
public class AuthenticationHelper : IAuthenticationHelper
{
    private const string BeginsWithV = "v";
    private const string ThenZeroOrMoreIntegers = "[0-9]+";

    public AuthenticationHelper() { }

    public AuthenticationData ExtractAuthenticationDataFromContext(HttpContext context)
    {
        AuthenticationData authenticationData = new()
        {
            AuthenticationKey = context.Request.Headers["Authorization"]
        };

        string[] routeValues = context.Request.Path.Value.Split('/');

        authenticationData.VersionText = routeValues.Length > 1
            ? routeValues[1]
            : string.Empty;

        int.TryParse(authenticationData.VersionText.Replace("v", string.Empty), out int version);
        authenticationData.Version = version;

        authenticationData.BusinessId = routeValues.Length > 2
            ? routeValues[2]
            : string.Empty;

        authenticationData.Endpoint = routeValues.Length > 3
            ? routeValues[3]
            : string.Empty;

        authenticationData.Verb = context.Request.Method;

        return authenticationData;
    }

    public string GetApiEndPoint(string endpoint)
    {
        return endpoint.ToLower() switch
        {
            "article" or "articles" => "articles",
            "payment" or "payments" => "payments",
            "event" or "events" or "event-categories" or "eventhomepage" => "events",
            "topic" or "topics" => "topics",
            "profile" or "profiles" => "profiles",
            "start-page" or "start-pages" => "start pages",
            "showcase" or "showcases" => "showcase",
            _ => endpoint.ToLower(),
        };
    }

    public void CheckVersionIsProvided(AuthenticationData authenticationData)
    {
        Regex versionPattern = new(BeginsWithV + ThenZeroOrMoreIntegers);

        if (!versionPattern.IsMatch(authenticationData.VersionText))
            throw new AuthorizationException();
    }
}

[ExcludeFromCodeCoverage]
public class AuthenticationData
{
    public string AuthenticationKey { get; set; }
    public string VersionText { get; set; }
    public int Version { get; set; }
    public string BusinessId { get; set; }
    public string Endpoint { get; set; }
    public string Verb { get; set; }
}