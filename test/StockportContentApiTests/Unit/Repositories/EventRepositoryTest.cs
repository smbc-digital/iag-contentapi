using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using HttpResponse = StockportContentApi.Http.HttpResponse;

namespace StockportContentApiTests.Unit.Repositories
{
    public class EventRepositoryTest
    {

        private readonly Mock<IHttpClient> _httpClient;
        private readonly EventRepository _repository;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private const string Title = "This is the event";
        private const string Description = "The event";
        private const string Slug = "event-of-the-century";
        private const string Teaser = "Read more for the event";
        private readonly DateTime _sunriseDate = new DateTime(2016, 08, 01);
        private readonly DateTime _sunsetDate = new DateTime(2016, 08, 10);
        private const string Image = "image.jpg";
        private const string ThumbnailImage = "thumbnail.jpg";


        public EventRepositoryTest()
        {
            var config = new ContentfulConfig("test")
               .Add("DELIVERY_URL", "https://fake.url")
               .Add("TEST_SPACE", "SPACE")
               .Add("TEST_ACCESS_KEY", "KEY")
               .Build();

            _mockTimeProvider = new Mock<ITimeProvider>();
            _httpClient = new Mock<IHttpClient>();            
            var eventFactory = new Mock<IFactory<Event>>();            
            _repository = new EventRepository(config, _httpClient.Object, eventFactory.Object, _mockTimeProvider.Object);

            eventFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>())).Returns(
                new Event(Title, Slug, Teaser, Image, Description, _sunriseDate, _sunsetDate));            
        }


        [Fact]
        public void GetsASingleEventItemFromASlug()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=event&include=1&fields.slug=event-of-the-century"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Event.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetEvent("event-of-the-century"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventItem = response.Get<Event>();

            eventItem.Title.Should().Be(Title);
            eventItem.Description.Should().Be(Description);
            eventItem.Slug.Should().Be(Slug);
            eventItem.Teaser.Should().Be(Teaser);
            eventItem.SunriseDate.Should().Be(_sunriseDate);
            eventItem.SunsetDate.Should().Be(_sunsetDate);
            eventItem.Image.Should().Be(Image);                   
        }

        [Fact]
        public void GetsAllNewsItems()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 5));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=news&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/NewsListing.json")));

            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=newsroom&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Newsroom.json")));
          

            var response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, null));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var newsroom = response.Get<Newsroom>();

            newsroom.Alerts.Count.Should().Be(1);
            newsroom.Alerts.Should().BeEquivalentTo(_alerts);

            newsroom.EmailAlerts.Should().Be(true);
            newsroom.EmailAlertsTopicId.Should().Be("test-id");
            newsroom.Categories.Count.Should().Be(2);
            newsroom.Categories[0].Should().Be("news-category-1");
            newsroom.Categories[1].Should().Be("news-category-2");
            newsroom.Dates.Should().HaveCount(1);
            newsroom.Dates[0].Should().HaveMonth(8);
            newsroom.Dates[0].Should().HaveYear(2016);


            newsroom.News.Count.Should().Be(2);
            var firstNews = newsroom.News.First();
            firstNews.Title.Should().Be(Title);
            firstNews.Body.Should().Be(Body);
            firstNews.Slug.Should().Be(Slug);
            firstNews.Teaser.Should().Be(Teaser);
            firstNews.SunriseDate.Should().Be(_sunriseDate);
            firstNews.SunsetDate.Should().Be(_sunsetDate);
            firstNews.Image.Should().Be(Image);
            firstNews.ThumbnailImage.Should().Be(ThumbnailImage);
            firstNews.Breadcrumbs.Should().BeEquivalentTo(_crumbs);
            firstNews.Alerts.Should().BeEquivalentTo(_alerts);
            firstNews.Categories.Count.Should().Be(2);
            firstNews.Categories[0].Should().Be("news-category-1");
            firstNews.Categories[1].Should().Be("news-category-2");
        }


    }
}
