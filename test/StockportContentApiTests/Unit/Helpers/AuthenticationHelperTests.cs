using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using StockportContentApi.Config;
using StockportContentApi.Exceptions;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Helpers
{
    public class AuthenticationHelperTests
    {
        private readonly AuthenticationHelper _helper;
        private readonly Mock<IApiKeyRepository> _repository;

        public AuthenticationHelperTests()
        {
            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(_ => _.Now()).Returns(new DateTime(2017, 10, 25));
            _repository = new Mock<IApiKeyRepository>();

            _helper = new AuthenticationHelper(timeProvider.Object);
        }

        [Fact]
        public void ExtractAuthenticationDataFromContext_ShouldReturnAuthenticationDataWithCorrectValues()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/v1/stockportgov/articles/test";
            context.Request.Headers.Add("Authorization", "test");
            context.Request.Method = "GET";

            // Act
            var data = _helper.ExtractAuthenticationDataFromContext(context);

            // Assert
            data.Version.Should().Be(1);
            data.AuthenticationKey.Should().Be("test");
            data.BusinessId.Should().Be("stockportgov");
            data.Endpoint.Should().Be("articles");
            data.Verb.Should().Be("GET");
            data.VersionText.Should().Be("v1");
        }

        [Fact]
        public void GetValidKey_ShouldThrowExceptionIfNoKeysAreReturned()
        {
            // Arrange
            const string authenticationKey = "test";
            const string businessId = "stockportgov";
            const string endpoint = "articles";
            const int version = 1;
            const string verb = "GET";
            _repository.Setup(_ => _.Get()).ReturnsAsync(new List<ApiKey>());

            // Act Assert
            Assert.ThrowsAsync<AuthorizationException>(async () =>
            {
                    await _helper.GetValidKey(_repository.Object, authenticationKey, businessId, endpoint, version, verb);
            });
        }

        [Fact]
        public async Task GetValidKey_ShouldThrowExceptionWhenKeyDoesNotMatchAsync()
        {
            // Arrange
            var apiKey = new ApiKey("name", "test-fail", "email", DateTime.MinValue, DateTime.MaxValue, new List<string>() { "articles" }, 1, true, new List<string>() { "GET" });
            _repository.Setup(_ => _.Get()).ReturnsAsync(new List<ApiKey>() { apiKey });

            // Act Assert
            await Assert.ThrowsAsync<AuthorizationException>(() => _helper.GetValidKey(_repository.Object, "Bearer test", "stockportgov", "articles", 1, "GET"));

        }

        [Fact]
        public void GetValidKey_ShouldReturnApiKeyIfValid()
        {
            // Arrange

            // Act

            // Assert

        }
    }
}
