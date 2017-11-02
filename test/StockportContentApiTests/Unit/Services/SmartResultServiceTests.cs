using System;
using System.Collections.Generic;
using System.Text;
using Contentful.Core.Models;
using Moq;
using StockportContentApi.Builders;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Exceptions;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Services;
using Xunit;

namespace StockportContentApiTests.Unit.Services
{
    public class SmartResultServiceTests
    {

        private readonly Mock<Func<ContentfulConfig, ISmartResultRepository>> _mockSmartResultRepositoryFunc = new Mock<Func<ContentfulConfig, ISmartResultRepository>>();
        private readonly Mock<ISmartResultRepository> _mockSmartResultRepository = new Mock<ISmartResultRepository>();
        private readonly Mock<IContentfulFactory<ContentfulSmartResult, SmartResult>> _mockSmartResultFactory = new Mock<IContentfulFactory<ContentfulSmartResult, SmartResult>>();
        private readonly Mock<IContentfulConfigBuilder> _mockContentfulConfigBuilder = new Mock<IContentfulConfigBuilder>();
        private readonly SmartResultService _service;


        public SmartResultServiceTests()
        {
            _mockSmartResultRepositoryFunc.Setup(o => o(It.IsAny<ContentfulConfig>()))
                .Returns(_mockSmartResultRepository.Object);
            _service = new SmartResultService(_mockSmartResultRepositoryFunc.Object, _mockSmartResultFactory.Object, _mockContentfulConfigBuilder.Object);
        }

        [Fact]
        public async void GetSmartResultBySlug_ShouldCallRepository()
        {
            // Arrange
            var slug = "a-slug";
            var businessId = "stockportgov";
            var expectedResponse = new ContentfulSmartResult
            {
                Slug = slug,
            };

            _mockSmartResultRepository.Setup(_ => _.Get(slug)).ReturnsAsync(expectedResponse);

            // Act
            await _service.GetSmartResultBySlug(businessId, slug);

            // Assert
            _mockSmartResultRepository.Verify(_ => _.Get(slug), Times.Once);
        }

        [Fact]
        public async void GetSmartResultBySlug_ShouldThrowAnException_WhenResultNotFound()
        {
            // Arrange
            var slug = "a-slug";
            var bussinessId = "stockportgov";

            // Act
            // Assert
            await Assert.ThrowsAsync<ServiceException>(() => _service.GetSmartResultBySlug(bussinessId, slug));

        }

    }
}
