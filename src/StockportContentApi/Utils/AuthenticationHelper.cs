using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using StockportContentApi.Exceptions;

namespace StockportContentApi.Utils
{
    public interface IAuthenticationHelper
    {
        AuthenticationData ExtractAuthenticationDataFromContext(HttpContext context);
        string GetApiEndPoint(string endpoint);
        void CheckVersionIsProvided(AuthenticationData authenticationData);
    }

    public class AuthenticationHelper : IAuthenticationHelper
    {
        private readonly DateComparer _dateComparer;
        private const string BeginsWithV = "v";
        private const string ThenZeroOrMoreIntegers = "[0-9]+";

        public AuthenticationHelper(ITimeProvider timeProvider)
        {
            _dateComparer = new DateComparer(timeProvider);
        }

        public AuthenticationData ExtractAuthenticationDataFromContext(HttpContext context)
        {
            var authenticationData = new AuthenticationData
            {
                AuthenticationKey = context.Request.Headers["Authorization"]
            };

            var routeValues = context.Request.Path.Value.Split('/');

            authenticationData.VersionText = routeValues.Length > 1 ? routeValues[1] : string.Empty;
            int.TryParse(authenticationData.VersionText.Replace("v", string.Empty), out var version);
            authenticationData.Version = version;

            authenticationData.BusinessId = routeValues.Length > 2 ? routeValues[2] : string.Empty;
            authenticationData.Endpoint = routeValues.Length > 3 ? routeValues[3] : string.Empty;
            authenticationData.Verb = context.Request.Method;

            return authenticationData;
        }

        public string GetApiEndPoint(string endpoint)
        {
            switch (endpoint.ToLower())
            {
                case "article":
                case "articles":
                    return "articles";
                case "group":
                case "groups":
                case "group-categories":
                case "group-results":
                    return "groups";
                case "payment":
                case "payments":
                    return "payments";
                case "event":
                case "events":
                case "event-categories":
                case "eventhomepage":
                    return "events";
                case "topic":
                case "topics":
                    return "topics";
                case "profile":
                case "profiles":
                    return "profiles";
                case "start-page":
                case "start-pages":
                    return "start pages";
                case "showcase":
                case "showcases":
                    return "showcase";
                case "organisation":
                case "organisations":
                    return "organisations";
                default:
                    return endpoint.ToLower();
            }
        }

        public void CheckVersionIsProvided(AuthenticationData authenticationData)
        {
            var versionPattern = new Regex(BeginsWithV + ThenZeroOrMoreIntegers);
            if (!versionPattern.IsMatch(authenticationData.VersionText))
            {
                throw new AuthorizationException();
            }
        }
    }

    public class AuthenticationData
    {
        public string AuthenticationKey { get; set; }
        public string VersionText { get; set; }
        public int Version { get; set; }
        public string BusinessId { get; set; }
        public string Endpoint { get; set; }
        public string Verb { get; set; }
    }
}
