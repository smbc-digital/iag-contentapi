using System;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Config;
using StockportContentApi.Middleware;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using Contentful.Core.Search;
using StockportContentApi.ContentfulModels;
using System.Collections.Generic;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Middleware
{
    public class AuthenticationMiddlewareTest
    {
        private readonly AuthenticationMiddleware _middleware;
        private readonly Mock<RequestDelegate> _requestDelegate;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<AuthenticationMiddleware>> _logger;
        private readonly Mock<ITimeProvider> _timeProvider;
        private readonly Mock<Func<string, ContentfulConfig>> _createConfig;
        private readonly Mock<Func<ContentfulConfig, IApiKeyRepository>> _repository;
        private readonly Mock<IApiKeyRepository> _apiRepository;

        public AuthenticationMiddlewareTest()
        {
            _configuration = new Mock<IConfiguration>();
            _requestDelegate = new Mock<RequestDelegate>();
            _logger = new Mock<ILogger<AuthenticationMiddleware>>();
            _createConfig = new Mock<Func<string, ContentfulConfig>>();
            _repository = new Mock<Func<ContentfulConfig, IApiKeyRepository>>();
            _timeProvider = new Mock<ITimeProvider>();
            _apiRepository = new Mock<IApiKeyRepository>();
            _repository.Setup(x => x(It.IsAny<ContentfulConfig>())).Returns(_apiRepository.Object);
            _middleware = new AuthenticationMiddleware(_requestDelegate.Object, _configuration.Object, _logger.Object, _timeProvider.Object, _createConfig.Object, _repository.Object);
        }

        [Fact]
        public async void ShouldReturnNextIfValidAuthenticationKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var builder = new QueryBuilder<ContentfulApiKey>().ContentTypeIs("apiKey");
            _apiRepository.Setup(_ => _.Get()).ReturnsAsync(
                new List<ApiKey>()                {
                    new ApiKey("name", "test", "email", DateTime.MinValue, DateTime.MaxValue, new List<string>() { "test", "test" }, 4)
                });
            context.Request.Path = "/api/stockportgov/test";

            context.Request.Headers.Add("Authorization", "test");
            _configuration.Setup(_ => _["Authorization"]).Returns("test");

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
            var builder = new QueryBuilder<ContentfulApiKey>().ContentTypeIs("apiKey");
            _apiRepository.Setup(_ => _.Get()).ReturnsAsync(
                new List<ApiKey>()                {
                    new ApiKey("name", "key", "email", DateTime.MinValue, DateTime.MaxValue, new List<string>() { "test", "test" }, 4)
                });
            context.Request.Path = "/api/stockportgov/test";

            context.Request.Headers.Add("Authorization", "test-invalid");
            _configuration.Setup(_ => _["Authorization"]).Returns("test");

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
            _configuration.Setup(_ => _["Authorization"]).Returns("test");

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
            context.Request.Headers.Add("Authorization", "test-invalid");

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            LogTesting.Assert(_logger, LogLevel.Critical, "API Authentication Key is missing from the config");
        }
    }
}
