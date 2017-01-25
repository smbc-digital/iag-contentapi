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

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class NewsContentfulFactoryTest
    {
        private readonly Mock<IVideoRepository> _videoRepository;
        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
        private readonly NewsContentfulFactory _newsContentfulFactory;
        private readonly ContentfulNews _contentfulNews;

        public NewsContentfulFactoryTest()
        {
            _contentfulNews = new ContentfulNewsBuilder().Build();
            _videoRepository = new Mock<IVideoRepository>();
            _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            _newsContentfulFactory = new NewsContentfulFactory(_videoRepository.Object, _documentFactory.Object);
        }

        [Fact]
        public void ShouldCreateANewsFromAContentfulNews()
        {
            const string processedBody = "this is processed body";
            _videoRepository.Setup(o => o.Process(_contentfulNews.Body)).Returns(processedBody);
            var document = new Document("title", 1000, DateTime.MinValue.ToUniversalTime(), "url", "fileName");
            _documentFactory.Setup(o => o.ToModel(_contentfulNews.Documents.First())).Returns(document);
            
            var news = _newsContentfulFactory.ToModel(_contentfulNews);

            news.ShouldBeEquivalentTo(_contentfulNews, o => o.Excluding(e => e.Image).Excluding(e => e.ThumbnailImage).Excluding(e => e.Documents).Excluding(e => e.Body));
            news.Body.Should().Be(processedBody);
            news.Image.Should().Be(_contentfulNews.Image.File.Url);
            news.ThumbnailImage.Should().Be(_contentfulNews.Image.File.Url + "?h=250");            
            news.Documents.Count.Should().Be(1);
            news.Documents.First().Should().Be(document);
            _documentFactory.Verify(o => o.ToModel(_contentfulNews.Documents.First()), Times.Once);
        }

        public void ShouldNotAddDocumentsOrImageIfTheyAreLinks()
        {
            _contentfulNews.Documents.First().SystemProperties.Type = "Link";
            _contentfulNews.Image.SystemProperties.Type = "Link";
            _videoRepository.Setup(o => o.Process(_contentfulNews.Body)).Returns(_contentfulNews.Body);

            var news = _newsContentfulFactory.ToModel(_contentfulNews);

            _documentFactory.Verify(o => o.ToModel(It.IsAny<Asset>()), Times.Never);
            news.Documents.Count.Should().Be(0);
            news.Image.Should().BeEmpty();
            news.ThumbnailImage.Should().BeEmpty();
        }
    }
}
