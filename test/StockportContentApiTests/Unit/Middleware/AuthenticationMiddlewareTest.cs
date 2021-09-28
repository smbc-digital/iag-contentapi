using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Config;
using StockportContentApi.Middleware;
using StockportContentApi.Utils;
using Xunit;

namespace StockportContentApiTests.Unit.Middleware
{
    public class AuthenticationMiddlewareTest
    {
        private readonly AuthenticationMiddleware _middleware;
        private readonly Mock<RequestDelegate> _requestDelegate;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<AuthenticationMiddleware>> _logger;
        private Mock<IAuthenticationHelper> _authHelper;
        private readonly Mock<Func<string, ContentfulConfig>> _createConfig;

        public AuthenticationMiddlewareTest()
        {
            _createConfig = new Mock<Func<string, ContentfulConfig>>();
            _configuration = new Mock<IConfiguration>();
            _requestDelegate = new Mock<RequestDelegate>();
            _logger = new Mock<ILogger<AuthenticationMiddleware>>();
            _authHelper = new Mock<IAuthenticationHelper>();
            _middleware = new AuthenticationMiddleware(_requestDelegate.Object, _configuration.Object, _logger.Object, _authHelper.Object, _createConfig.Object);
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
    }
}
