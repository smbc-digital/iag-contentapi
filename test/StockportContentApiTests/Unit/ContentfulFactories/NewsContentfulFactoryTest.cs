using System;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using StockportContentApi.Fakes;
using StockportContentApi.Utils;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class NewsContentfulFactoryTest
    {
        private readonly Mock<IVideoRepository> _videoRepository;
        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertBuilder;
        private readonly Mock<ITimeProvider> _timeProvider = new Mock<ITimeProvider>();
        private readonly NewsContentfulFactory _newsContentfulFactory;
        private readonly ContentfulNews _contentfulNews;

        public NewsContentfulFactoryTest()
        {
            _contentfulNews = new ContentfulNewsBuilder().Document().Build();
            _videoRepository = new Mock<IVideoRepository>();
            _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            _alertBuilder = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _newsContentfulFactory = new NewsContentfulFactory(_videoRepository.Object, _documentFactory.Object, HttpContextFake.GetHttpContextFake(), _alertBuilder.Object, _timeProvider.Object);
        }

        [Fact]
        public void ShouldCreateANewsFromAContentfulNews()
        {
            // Arrange
            const string processedBody = "this is processed body";
            var document = new DocumentBuilder().Build();

            // Mock
            _videoRepository.Setup(o => o.Process(_contentfulNews.Body)).Returns(processedBody);
            _documentFactory.Setup(o => o.ToModel(_contentfulNews.Documents.First())).Returns(document);
            _alertBuilder.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new AlertBuilder().Build());
            
            // Act
            var news = _newsContentfulFactory.ToModel(_contentfulNews);

            // Assert
            news.ShouldBeEquivalentTo(_contentfulNews, o => o.Excluding(e => e.Image).Excluding(e => e.ThumbnailImage).Excluding(e => e.Documents).Excluding(e => e.Body).Excluding(e => e.Breadcrumbs));
            news.Body.Should().Be(processedBody);
            news.Image.Should().Be(_contentfulNews.Image.File.Url);
            news.ThumbnailImage.Should().Be(_contentfulNews.Image.File.Url + "?h=250");            
            news.Documents.Count.Should().Be(1);
            news.Documents.First().Should().Be(document);
            _documentFactory.Verify(o => o.ToModel(_contentfulNews.Documents.First()), Times.Once);
        }

        [Fact]
        public void ShouldNotAddDocumentsOrImageIfTheyAreLinks()
        {
            // Arrange
            _contentfulNews.Documents.First().SystemProperties.Type = "Link";
            _contentfulNews.Image.SystemProperties.Type = "Link";

            // Mock
            _videoRepository.Setup(o => o.Process(_contentfulNews.Body)).Returns(_contentfulNews.Body);

            // Act
            var news = _newsContentfulFactory.ToModel(_contentfulNews);

            // Assert
            _documentFactory.Verify(o => o.ToModel(It.IsAny<Asset>()), Times.Never);
            news.Documents.Count.Should().Be(0);
            news.Image.Should().BeEmpty();
            news.ThumbnailImage.Should().BeEmpty();
        }
    }
}
