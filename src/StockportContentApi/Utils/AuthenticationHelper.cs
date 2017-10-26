using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockportContentApi.Config;
using StockportContentApi.Exceptions;
using StockportContentApi.Model;
using StockportContentApi.Repositories;

namespace StockportContentApi.Utils
{
    public interface IAuthenticationHelper
    {
        AuthenticationData ExtractAuthenticationDataFromContext(HttpContext context);
        string GetApiEndPoint(string endpoint);
        Task<ApiKey> GetValidKey(IApiKeyRepository repository, string authenticationKey, string businessId, string endpoint, int version, string verb);
        void HandleSensitiveData(HttpContext context, ApiKey validKey);
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

        public async Task<ApiKey> GetValidKey(IApiKeyRepository repository, string authenticationKey, string businessId, string endpoint, int version, string verb)
        {
            var keys = await repository.Get();

            if (keys == null)
            {
                throw new AuthorizationException();
            }

            var validEndPoint = GetApiEndPoint(endpoint);

            var validKey = keys.FirstOrDefault(k => "Bearer " + k.Key == authenticationKey.Trim()
                                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(k.ActiveFrom,
                                                        k.ActiveTo)
                                                    && k.EndPoints.Any(e => e.ToLower() == validEndPoint)
                                                    && k.Version >= version
                                                    && k.AllowedVerbs.Any(v => string.Equals(v, verb, StringComparison.CurrentCultureIgnoreCase)));

            if (validKey == null)
            {
                throw new AuthorizationException();
            }

            return validKey;
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
                    return "groups";
                case "payment":
                case "payments":
                    return "payments";
                case "event":
                case "events":
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
                case "smart":
                    return "smart answers";
                case "organisation":
                case "organisations":
                    return "organisations";
                default:
                    return endpoint.ToLower();
            }
        }

        public void HandleSensitiveData(HttpContext context, ApiKey validKey)
        {
            if (validKey.CanViewSensitive)
            {
                context.Request.Headers["cannotViewSensitive"] = "false";
            }
            else
            {
                context.Request.Headers["cannotViewSensitive"] = "true";
            }
        }

        public void CheckVersionIsProvided(AuthenticationData authenticationData)
        {
            var versionPattern = new Regex(BeginsWithV + ThenZeroOrMoreIntegers);
            if (!versionPattern.IsMatch(authenticationData.VersionText))
            {
                throw new AuthenticationException();
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
