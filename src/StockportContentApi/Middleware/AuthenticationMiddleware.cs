﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using System.Text.RegularExpressions;

namespace StockportContentApi.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationMiddleware> _logger;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, IApiKeyRepository> _repository;
        private readonly DateComparer _dateComparer;

        public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration,
            ILogger<AuthenticationMiddleware> logger, ITimeProvider timeProvider,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, IApiKeyRepository> repository)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
            _createConfig = createConfig;
            _repository = repository;
            _dateComparer = new DateComparer(timeProvider);
        }

        public async Task Invoke(HttpContext context)
        {
            var key = _configuration["Authorization"] ?? string.Empty;

            if (string.IsNullOrEmpty(key))
            {
                _logger.LogCritical("API Authentication Key is missing from the config");
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("API Authentication Key is missing from the config");
                return;
            }

            var authenticationKey = context.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authenticationKey))
            {
                _logger.LogError("API Authentication Key is either missing or wrong");
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("API Authentication Key is either missing or wrong");
                return;
            }

            if (authenticationKey != key)
            {
                var routeValues = context.Request.Path.Value.Split('/');

                var versionText = routeValues[2];
                var pattern = new Regex("v[0-9]+");
                if (!pattern.IsMatch(versionText))
                {
                    _logger.LogError("Invalid attempt to access API from API Key without a version");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("API Authentication Key is either missing or wrong");
                    return;
                }

                int version;
                int.TryParse(versionText.Replace("v", string.Empty), out version);
                var businessId = routeValues[3];
                var endpoint = routeValues[4];

                var validKey = await GetValidKey(authenticationKey, businessId, endpoint, version);

                if (validKey == null)
                {
                    _logger.LogError("API Authentication Key is either missing or wrong");
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("API Authentication Key is either missing or wrong");
                    return;
                }
            }

            await _next.Invoke(context);
        }

        private async Task<ApiKey> GetValidKey(string authenticationKey, string businessId, string endpoint, int version)
        {
            var repo = _repository(_createConfig(businessId));
            var keys = await repo.Get();

            var validEndPoint = GetApiEndPoint(endpoint);

            var validKey = keys.FirstOrDefault(k => "Bearer " + k.Key == authenticationKey.ToString().Trim() 
                                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(k.ActiveFrom,
                                                        k.ActiveTo)
                                                    && k.EndPoints.Any(e => e.ToLower() == validEndPoint)
                                                    && k.Version >= version);

            return validKey;
        }

        private static string GetApiEndPoint(string endpoint)
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
                default:
                    return endpoint.ToLower();
            }
        }
    }
}