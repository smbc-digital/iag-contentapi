using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Middleware;
using Xunit;

namespace StockportContentApiTests.Unit.Middleware
{
    public class AuthenticationMiddlewareTest
    {
        private readonly AuthenticationMiddleware _middleware;
        private readonly Mock<RequestDelegate> _requestDelegate;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<AuthenticationMiddleware>> _logger;

        public AuthenticationMiddlewareTest()
        {
            _configuration = new Mock<IConfiguration>();
            _requestDelegate = new Mock<RequestDelegate>();
            _logger = new Mock<ILogger<AuthenticationMiddleware>>();
            _middleware = new AuthenticationMiddleware(_requestDelegate.Object, _configuration.Object, _logger.Object);
        }

        [Fact]
        public async void ShouldReturnNextIfValidAuthenticationKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("AuthenticationKey", "test");
            _configuration.Setup(_ => _["ContentApiAuthenticationKey"]).Returns("test");

            // Act
            await _middleware.Invoke(context);
            
            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async void ShouldReturn401IfInvalidAuthenticationKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("AuthenticationKey", "test-invalid");
            _configuration.Setup(_ => _["ContentApiAuthenticationKey"]).Returns("test");

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async void ShouldReturn401IfNoAuthenticationKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _configuration.Setup(_ => _["ContentApiAuthenticationKey"]).Returns("test");

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
            LogTesting.Assert(_logger, LogLevel.Error, "API Authentication Key is either missing or wrong");
        }

        [Fact]
        public async void ShouldReturn500IfConfigKeyIsMissing()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("AuthenticationKey", "test-invalid");

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            LogTesting.Assert(_logger, LogLevel.Critical, "API Authentication Key is missing from the config");
        }
    }
}
