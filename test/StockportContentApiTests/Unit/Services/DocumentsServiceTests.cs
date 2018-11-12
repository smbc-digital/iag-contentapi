using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.Builders;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Services;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApiTests.Unit.Services
{
    public class DocumentsServiceTests
    {
        private readonly Mock<Func<ContentfulConfig, IAssetRepository>> _mockDocumentRepositoryFunc = new Mock<Func<ContentfulConfig, IAssetRepository>>();
        private readonly Mock<IAssetRepository> _mockDocumentRepository = new Mock<IAssetRepository>();
        private readonly Mock<Func<ContentfulConfig, IGroupAdvisorRepository>> _mockGroupAdvisorRepositoryFunc = new Mock<Func<ContentfulConfig, IGroupAdvisorRepository>>();
        private readonly Mock<IGroupAdvisorRepository> _mockGroupAdvisorRepository = new Mock<IGroupAdvisorRepository>();
        private readonly Mock<Func<ContentfulConfig, IGroupRepository>> _mockGroupRepositoryFunc = new Mock<Func<ContentfulConfig, IGroupRepository>>();
        private readonly Mock<IGroupRepository> _mockGroupRepository = new Mock<IGroupRepository>();
        private readonly Mock<IContentfulFactory<Asset, Document>> _mockDocumentFactory = new Mock<IContentfulFactory<Asset, Document>>();
        private readonly Mock<IContentfulConfigBuilder> _mockContentfulConfigBuilder = new Mock<IContentfulConfigBuilder>();
        private readonly Mock<ILoggedInHelper> _mockLoggedInHelper = new Mock<ILoggedInHelper>();

        public DocumentsServiceTests()
        {
            _mockDocumentRepositoryFunc.Setup(o => o(It.IsAny<ContentfulConfig>()))
                .Returns(_mockDocumentRepository.Object);

            _mockGroupAdvisorRepositoryFunc.Setup(o =>
                o(It.IsAny<ContentfulConfig>())).Returns(_mockGroupAdvisorRepository.Object);

            _mockGroupRepositoryFunc
                .Setup(o => o(It.IsAny<ContentfulConfig>())).Returns(_mockGroupRepository.Object);
        }

        [Fact]
        public async void GetSecureAssetByDocumentId_ShouldReturnDocument_ToAuthorisedUser()
        {
            // Arrange
            var expectedResult = new DocumentBuilder().Build();

            _mockDocumentRepository.Setup(o => o.Get(It.IsAny<string>()))
                .ReturnsAsync(new Asset()
                {
                    Description = "description",
                    DescriptionLocalized = new Dictionary<string, string>{{ "en-GB", "description"}},
                    File = new File()
                    {
                        Url = "url",
                        Details = new FileDetails()
                        {
                            Size = 22
                        }
                    },
                    FilesLocalized = new Dictionary<string, File> { {"en-GB", new File() } },
                    SystemProperties = new SystemProperties()
                    {
                        Id = "asset id"
                    },
                    Title = "title",
                    TitleLocalized = new Dictionary<string, string>{{ "en-GB", "title" } }
                });

            _mockGroupAdvisorRepository.Setup(o => o.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            
            _mockGroupRepository.Setup(o => o.GetGroup(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(HttpResponse.Successful(new GroupBuilder().AdditionalDocuments(new List<Document>{ expectedResult }).Build()));

            _mockDocumentFactory.Setup(o => o.ToModel(It.IsAny<Asset>())).Returns(expectedResult);

            _mockContentfulConfigBuilder.Setup(o => o.Build(It.IsAny<string>())).Returns(new ContentfulConfig("","",""));

            _mockLoggedInHelper.Setup(o => o.GetLoggedInPerson()).Returns(new LoggedInPerson()
            {
                Email = "email",
                Name = "name"
            });

            var documentsService = new DocumentsService(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);
            
            // Act
            var result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_ToUnauthorisedUser()
        {
            // Arrange
            var expectedResult = new DocumentBuilder().Build();

            _mockDocumentRepository.Setup(o => o.Get(It.IsAny<string>()))
                .ReturnsAsync(new Asset()
                {
                    Description = "description",
                    DescriptionLocalized = new Dictionary<string, string> { { "en-GB", "description" } },
                    File = new File()
                    {
                        Url = "url",
                        Details = new FileDetails()
                        {
                            Size = 22
                        }
                    },
                    FilesLocalized = new Dictionary<string, File> { { "en-GB", new File() } },
                    SystemProperties = new SystemProperties()
                    {
                        Id = "asset id"
                    },
                    Title = "title",
                    TitleLocalized = new Dictionary<string, string> { { "en-GB", "title" } }
                });

            _mockGroupAdvisorRepository.Setup(o => o.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            _mockGroupRepository.Setup(o => o.GetGroup(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(HttpResponse.Successful(new GroupBuilder().AdditionalDocuments(new List<Document> { expectedResult }).Build()));

            _mockDocumentFactory.Setup(o => o.ToModel(It.IsAny<Asset>())).Returns(expectedResult);

            _mockContentfulConfigBuilder.Setup(o => o.Build(It.IsAny<string>())).Returns(new ContentfulConfig("", "", ""));

            _mockLoggedInHelper.Setup(o => o.GetLoggedInPerson()).Returns(new LoggedInPerson()
            {
                Email = "email",
                Name = "name"
            });

            var documentsService = new DocumentsService(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);


            // Act
            var result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_ToNotLoggedInUser()
        {
            // Arrange
            _mockContentfulConfigBuilder.Setup(o => o.Build(It.IsAny<string>())).Returns(new ContentfulConfig("", "", ""));
            _mockLoggedInHelper.Setup(o => o.GetLoggedInPerson()).Returns(new LoggedInPerson());

            var documentsService = new DocumentsService(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);

            // Act
            var result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_IfAssetDoesNotExist()
        {
            // Arrange
            var document = new DocumentBuilder().Build();
            
            _mockDocumentRepository.Setup(o => o.Get(It.IsAny<string>()))
                .ReturnsAsync((Asset)null);

            _mockGroupAdvisorRepository.Setup(o => o.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            _mockGroupRepository.Setup(o => o.GetGroup(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(HttpResponse.Successful(new GroupBuilder().AdditionalDocuments(new List<Document> { document }).Build()));

            _mockDocumentFactory.Setup(o => o.ToModel(It.IsAny<Asset>())).Returns(document);

            _mockContentfulConfigBuilder.Setup(o => o.Build(It.IsAny<string>())).Returns(new ContentfulConfig("", "", ""));

            _mockLoggedInHelper.Setup(o => o.GetLoggedInPerson()).Returns(new LoggedInPerson()
            {
                Email = "email",
                Name = "name"
            });

            var documentsService = new DocumentsService(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);

            // Act
            var result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_IfGroupDoesNotReferenceAsset()
        {
            // Arrange
            var document = new DocumentBuilder().Build();

            _mockDocumentRepository.Setup(o => o.Get(It.IsAny<string>()))
                .ReturnsAsync((Asset)null);

            _mockGroupAdvisorRepository.Setup(o => o.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            _mockGroupRepository.Setup(o => o.GetGroup(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(HttpResponse.Successful(new GroupBuilder().AdditionalDocuments(new List<Document>()).Build()));

            _mockDocumentFactory.Setup(o => o.ToModel(It.IsAny<Asset>())).Returns(document);

            _mockContentfulConfigBuilder.Setup(o => o.Build(It.IsAny<string>())).Returns(new ContentfulConfig("", "", ""));

            _mockLoggedInHelper.Setup(o => o.GetLoggedInPerson()).Returns(new LoggedInPerson()
            {
                Email = "email",
                Name = "name"
            });

            var documentsService = new DocumentsService(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);

            // Act
            var result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

            // Assert
            result.Should().BeNull();
        }

    }
}
