using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using StockportContentApi.Exceptions;

namespace StockportContentApi.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly Func<ContentfulConfig, IApiKeyRepository> _repository;
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationMiddleware> _logger;
        private readonly IAuthenticationHelper _authHelper;
        private readonly Func<string, ContentfulConfig> _createConfig;

        public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration,
            ILogger<AuthenticationMiddleware> logger, IAuthenticationHelper authHelper,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, IApiKeyRepository> repository)
        {
            _repository = repository;
            _next = next;
            _configuration = configuration;
            _logger = logger;
            _authHelper = authHelper;
            _createConfig = createConfig;
        }

        public async Task Invoke(HttpContext context)
        {
            var apiConfigurationkey = _configuration["Authorization"] ?? string.Empty;

            if (string.IsNullOrEmpty(apiConfigurationkey))
            {
                _logger.LogCritical("API Authentication Key is missing from the config");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("API Authentication Key is missing from the config");
                return;
            }

            var authenticationData = _authHelper.ExtractAuthenticationDataFromContext(context);

            if (string.IsNullOrEmpty(authenticationData.AuthenticationKey))
            {
                _logger.LogError("API Authentication Key is either missing or wrong");
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("API Authentication Key is either missing or wrong.");
                return;
            }

            if (authenticationData.AuthenticationKey != apiConfigurationkey)
            {
                try
                {
                    _authHelper.CheckVersionIsProvided(authenticationData);
                }
                catch (AuthorizationException)
                {
                    _logger.LogError("Invalid attempt to access API from API Key without a version");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("API Authentication Key is either missing or wrong-");
                    return;
                }

                ApiKey validKey;

                try
                {
                    var repo = _repository(_createConfig(authenticationData.BusinessId));
                    validKey = await _authHelper.GetValidKey(repo, authenticationData.AuthenticationKey, authenticationData.BusinessId,
                        authenticationData.Endpoint, authenticationData.Version, authenticationData.Verb);
                }
                catch (AuthorizationException)
                {
                    _logger.LogError("API Authentication Key is either missing or wrong");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("API Authentication Key is either missing or wrong_");
                    return;
                }

                _authHelper.HandleSensitiveData(context, validKey);
            }
            else
            {
                context.Request.Headers["cannotViewSensitive"] = "false";
            }

            await _next.Invoke(context);
        }
    }
}