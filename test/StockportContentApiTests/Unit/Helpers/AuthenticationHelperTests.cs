using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using StockportContentApi.Config;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Helpers
{
    public class AuthenticationHelperTests
    {
        private Mock<ITimeProvider> _timeProvider;
        private AuthenticationHelper _helper;
        private readonly Mock<Func<string, ContentfulConfig>> _createConfig;
        public Mock<IApiKeyRepository> _repository { get; set; }
        Mock<Func<ContentfulConfig, IApiKeyRepository>> _createRepository;

        public AuthenticationHelperTests()
        {
            _createConfig = new Mock<Func<string, ContentfulConfig>>();
            _timeProvider = new Mock<ITimeProvider>();
            _createRepository = new Mock<Func<ContentfulConfig, IApiKeyRepository>>();
            _repository = new Mock<IApiKeyRepository>();
            _createRepository.Setup(_ => _.Invoke(It.IsAny<ContentfulConfig>())).Returns(_repository.Object);

            _helper = new AuthenticationHelper(_timeProvider.Object, _createRepository.Object, _createConfig.Object);
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
            var authenticationKey = "test";
            var businessId = "stockportgov";
            var endpoint = "articles";
            var version = 1;
            var verb = "GET";
            _repository.Setup(_ => _.Get()).ReturnsAsync(new List<ApiKey>());

            // Act Assert
            Assert.ThrowsAsync<AuthenticationException>(async () =>
            {
                    await _helper.GetValidKey(authenticationKey, businessId, endpoint, version, verb);
            });
        }

        [Fact]
        public void GetValidKey_ShouldThrowExceptionWhenInvalidKey()
        {
            // Arrange

            // Act

            // Assert

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
