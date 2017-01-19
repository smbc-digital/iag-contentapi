using System.Linq;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
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

            var newsContentfulFactory = new NewsContentfulFactory(videoRepository.Object);
            var news = newsContentfulFactory.ToModel(contentfulNews);

            news.ShouldBeEquivalentTo(contentfulNews, o => o.Excluding(e => e.Image).Excluding(e => e.ThumbnailImage).Excluding(e => e.Documents).Excluding(e => e.Body));
            news.Body.Should().Be(processedBody);
            news.Image.Should().Be(contentfulNews.Image.File.Url);
            news.ThumbnailImage.Should().Be(contentfulNews.Image.File.Url + "?h=250");
            news.Documents.Count.Should().Be(1);
            news.Documents.First().FileName.Should().Be(contentfulNews.Documents.First().File.FileName);
            news.Documents.First().LastUpdated.Should().Be(contentfulNews.Documents.First().SystemProperties.UpdatedAt.Value);
            news.Documents.First().Size.Should().Be((int)contentfulNews.Documents.First().File.Details.Size);
            news.Documents.First().Title.Should().Be(contentfulNews.Documents.First().Description);
            news.Documents.First().Url.Should().Be(contentfulNews.Documents.First().File.Url);
        }
    }
}
