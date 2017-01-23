using System;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class NewsContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateANewsFromAContentfulNews()
        {
            var videoRepository = new Mock<IVideoRepository>();
            var contentfulNews = new ContentfulNewsBuilder().Build();
            const string processedBody = "this is processed body";
            videoRepository.Setup(o => o.Process(contentfulNews.Body)).Returns(processedBody);
            var documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            var document = new Document("title", 1000, DateTime.MinValue.ToUniversalTime(), "url", "fileName");
            documentFactory.Setup(o => o.ToModel(contentfulNews.Documents.First())).Returns(document);
            
            var newsContentfulFactory = new NewsContentfulFactory(videoRepository.Object, documentFactory.Object);
            var news = newsContentfulFactory.ToModel(contentfulNews);

            news.ShouldBeEquivalentTo(contentfulNews, o => o.Excluding(e => e.Image).Excluding(e => e.ThumbnailImage).Excluding(e => e.Documents).Excluding(e => e.Body));
            news.Body.Should().Be(processedBody);
            news.Image.Should().Be(contentfulNews.Image.File.Url);
            news.ThumbnailImage.Should().Be(contentfulNews.Image.File.Url + "?h=250");
            
            news.Documents.Count.Should().Be(1);
            news.Documents.First().Should().Be(document);
            documentFactory.Verify(o => o.ToModel(contentfulNews.Documents.First()), Times.Once);
        }
    }
}
