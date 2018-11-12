using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Config;
using StockportContentApi.Middleware;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using StockportContentApi.Exceptions;

namespace StockportContentApiTests.Unit.Middleware
{
    public class AuthenticationMiddlewareTest
    {
        private readonly AuthenticationMiddleware _middleware;
        private readonly Mock<RequestDelegate> _requestDelegate;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<AuthenticationMiddleware>> _logger;
        private Mock<IAuthenticationHelper> _authHelper;
        private readonly Mock<Func<ContentfulConfig, IApiKeyRepository>> _repository;
        private readonly Mock<Func<string, ContentfulConfig>> _createConfig;

        public AuthenticationMiddlewareTest()
        {
            _repository = new Mock<Func<ContentfulConfig, IApiKeyRepository>>();
            _createConfig = new Mock<Func<string, ContentfulConfig>>();
            _configuration = new Mock<IConfiguration>();
            _requestDelegate = new Mock<RequestDelegate>();
            _logger = new Mock<ILogger<AuthenticationMiddleware>>();
            _authHelper = new Mock<IAuthenticationHelper>();
            _middleware = new AuthenticationMiddleware(_requestDelegate.Object, _configuration.Object, _logger.Object, _authHelper.Object, _createConfig.Object, _repository.Object);
        }

        [Fact]
        public async void Invoke_ShouldReturnIfNoApiKeyIsInTheConfig()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/v1/stockportgov/articles/test";
            context.Request.Headers.Add("Authorization", "test");
            context.Request.Method = "GET";

            // Act
            await _middleware.Invoke(context);

            // Assert
            LogTesting.Assert(_logger, LogLevel.Critical, "API Authentication Key is missing from the config");
            _authHelper.Verify(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>()), Times.Never); 
        }

        [Fact]
        public async void Invoke_ShouldReturnIfNoApiKeyIsInTheRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var authData = new AuthenticationData()
            {
                AuthenticationKey = "key",
                BusinessId = "businessid",
                Version = 1,
                Endpoint = "endpoint",
                Verb = "GET",
                VersionText = "incorrectText"
            };
            _authHelper.Setup(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>())).Returns(authData);
            _configuration.Setup(_ => _["Authorization"]).Returns("key");

            // Act
            await _middleware.Invoke(context);

            // Assert
            _authHelper.Verify(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async void Invoke_ShouldInvokeNextIfKeysMatch()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var authData = new AuthenticationData()
            {
                AuthenticationKey = "key",
                BusinessId = "businessid",
                Version = 1,
                Endpoint = "endpoint",
                Verb = "GET",
                VersionText = "incorrectText"
            };
            _authHelper.Setup(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>())).Returns(authData);
            _configuration.Setup(_ => _["Authorization"]).Returns("key");

            // Act
            await _middleware.Invoke(context);

            // Assert
            _requestDelegate.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async void Invoke_ShouldReturnIfNoVersionIsProvided()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var authData = new AuthenticationData()
            {
                AuthenticationKey = "key-not-matching",
                BusinessId = "businessid",
                Version = 1,
                Endpoint = "endpoint",
                Verb = "GET",
                VersionText = "incorrectText"
            };
            _authHelper.Setup(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>())).Returns(authData);
            _configuration.Setup(_ => _["Authorization"]).Returns("key");
            _authHelper.Setup(_ => _.CheckVersionIsProvided(It.IsAny<AuthenticationData>()))
                .Throws<AuthorizationException>();

            // Act
            await _middleware.Invoke(context);

            // Assert
            LogTesting.Assert(_logger, LogLevel.Error, "Invalid attempt to access API from API Key without a version");
        }

        [Fact]
        public async void Invoke_ShouldReturnIfKeyIsInvalid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var authData = new AuthenticationData()
            {
                AuthenticationKey = "key-not-matching",
                BusinessId = "businessid",
                Version = 1,
                Endpoint = "endpoint",
                Verb = "GET",
                VersionText = "incorrectText"
            };
            _authHelper.Setup(_ => _.ExtractAuthenticationDataFromContext(It.IsAny<HttpContext>())).Returns(authData);
            _configuration.Setup(_ => _["Authorization"]).Returns("key");
            _authHelper.Setup(_ => _.GetValidKey(It.IsAny<IApiKeyRepository>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Throws<AuthorizationException>();

            // Act
            await _middleware.Invoke(context);

            // Assert
            LogTesting.Assert(_logger, LogLevel.Error, "API Authentication Key is either missing or wrong");
        }
    }
}
