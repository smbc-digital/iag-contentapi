using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core.Search;
using Newtonsoft.Json;
using Xunit;
using HttpClient = System.Net.Http.HttpClient;
using FluentAssertions;
using Moq;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApiTests.Integration
{
    public class RoutesTest
    {
        private HttpClient _client;
        private readonly DateTime DEFAULT_DATE = new DateTime(2016, 09, 01);

        public RoutesTest()
        {
            TestAppFactory.FakeHttpClientFactory.MakeFakeHttpClientWithConfiguration(fakeHttpClient =>
            {
                fakeHttpClient.For(contentfulUrlFor("topic", 1, "test-topic")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ListOfArticlesForTopic.json"));
                fakeHttpClient.For(contentfulUrlFor("topic", 1, "test-topic-with-subtopic")).Return(CreateHttpResponse("Unit/MockContentfulResponses/TopicWithPrimarySecondaryAndTertiaryItems.json"));
                fakeHttpClient.For(contentfulUrlFor("topic", 1, "test-topic-with-alerts")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ListOfArticlesForTopicWithAlerts.json"));
                fakeHttpClient.For(contentfulUrlFor("article", 2, "test-article")).Return(CreateHttpResponse("Unit/MockContentfulResponses/Article.json"));
                fakeHttpClient.For(contentfulUrlFor("article", 2, "about-us")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ArticleWithoutSections.json"));
                fakeHttpClient.For(contentfulUrlFor("article", 2, "test-me")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ArticleWithParentTopic.json"));
                fakeHttpClient.For("https://buto-host.tv/video/kQl5D").Return(CreateHttpResponse("Unit/MockContentfulResponses/Article.json"));
                fakeHttpClient.For(contentfulUrlFor("profile", 1, "test-profile")).Return((CreateHttpResponse("Unit/MockContentfulResponses/ProfileWithBreadcrumbs.json")));
                fakeHttpClient.For(contentfulUrlFor("startPage", 1, "new-start-page")).Return((CreateHttpResponse("Unit/MockContentfulResponses/StartPage.json")));
                fakeHttpClient.For(contentfulUrlFor("homepage", 2)).Return((CreateHttpResponse("Unit/MockContentfulResponses/Homepage.json")));
                fakeHttpClient.For(contentfulUrlFor("news", 1, "news_item")).Return(CreateHttpResponse("Unit/MockContentfulResponses/News.json"));
                fakeHttpClient.For(contentfulUrlFor("news", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/NewsListing.json"));
                fakeHttpClient.For(contentfulUrlFor("newsroom", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/Newsroom.json"));
                fakeHttpClient.For(contentfulUrlForTag("news", 1, "Events")).Return(CreateHttpResponse("Unit/MockContentfulResponses/NewsListing.json"));
                fakeHttpClient.For(contentfulUrlForCategory("news", 1, "A category")).Return(CreateHttpResponse("Unit/MockContentfulResponses/NewsListing.json"));
                fakeHttpClient.For(contentfulUrlFor("article", true)).Return(CreateHttpResponse("Unit/MockContentfulResponses/AtoZ.json"));             
                fakeHttpClient.For(contentfulUrlFor("topic", true)).Return(CreateHttpResponse("Unit/MockContentfulResponses/AtoZTopic.json"));             
                fakeHttpClient.For("https://test-host.com/spaces/XX/entries?access_token=XX&content_type=redirect").Return(CreateHttpResponse("Unit/MockContentfulResponses/Redirects.json"));
                fakeHttpClient.For(contentfulUrlFor("footer", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/Footer.json"));
                fakeHttpClient.For(contentfulUrlFor("events", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/EventsCalendar.json"));
            });   

            TestAppFactory.FakeContentfulClientFactory.MakeContentfulClientWithConfiguration(httpClient =>
            {
                httpClient.Setup(o => o.GetEntriesAsync<Event>(
                                It.Is<QueryBuilder>(q => q.Build() == new QueryBuilder().ContentTypeIs("events").FieldEquals("fields.slug", "event_item").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> {
                                    new Event("This is the event", "event-of-the-century", "Read more for the event", "", "", "The event  description", 
                                        new DateTime(2016, 12, 08, 0, 0, 0, DateTimeKind.Utc), new DateTime(2016, 12, 22, 0, 0, 0, DateTimeKind.Utc), 
                                        "Free", "Bramall Hall, Carpark, SK7 6HG", "Friends of Stockport", "", "", false, 
                                        new DateTime(2016, 12, 30, 0, 0, 0, DateTimeKind.Utc), "10:00", "17:00", new List<Crumb> { new Crumb("Events", "", "events") })});
            });
        }
       
        [Theory]
        [InlineData("StartPage", "/api/unittest/start-page/new-start-page")]
        [InlineData("Profile", "/api/unittest/profile/test-profile")]
        [InlineData("TopicWithPrimarySecondaryAndTertiaryItems", "/api/unittest/topic/test-topic-with-subtopic")]
        [InlineData("ListOfArticlesForATopicWithAlerts", "/api/unittest/topic/test-topic-with-alerts")]
        [InlineData("ListOfArticlesForATopic", "/api/unittest/topic/test-topic")]
        [InlineData("ArticleWithoutSections", "/api/unittest/article/about-us")]
        [InlineData("Article", "/api/unittest/article/test-article")]
        [InlineData("Homepage", "/api/unittest/homepage")]
        [InlineData("ArticleWithParentTopic", "/api/unittest/article/test-me")]
        [InlineData("AtoZ", "/api/unittest/atoz/v")]
        [InlineData("AtoZTopic", "/api/unittest/atoz/b")]
        [InlineData("AtoZArticleAndTopic", "/api/unittest/atoz/c")]
        [InlineData("RedirectDictionary", "/api/redirects")]
        [InlineData("Footer", "/api/unittest/footer")]
        public async Task EndToEnd_ReturnsPageForASlug(string file, string path)
        {
            StartServer(DEFAULT_DATE);

            var expectedResponse = File.ReadAllText(GetFilePath(file));
            var contentResponse = JsonNormalize(expectedResponse);

            var response = await _client.GetAsync(path);
            var responseString = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonNormalize(responseString);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            parsedResponse.Should().Be(contentResponse);
        }

        [Theory]
        [InlineData("News", "/api/unittest/news/news_item", "2016-08-10T01:00:00+01:00")]
        [InlineData("NewsListing", "/api/unittest/news", "2016-08-10T01:00:00+01:00")]
        [InlineData("NewsListing", "/api/unittest/news?tag=Events", "2016-08-10T01:00:00+01:00")]
        [InlineData("NewsListing", "/api/unittest/news?category=A category", "2016-08-10T01:00:00+01:00")]
        [InlineData("NewsListingFilteredByDate", "/api/unittest/news?dateFrom=2016-08-01&dateTo=2016-08-31", "2017-08-02T01:00:00+01:00")]
        [InlineData("Event", "/api/unittest/events/event_item", "2016-12-10T01:00:00+01:00")]
        [InlineData("EventsCalendar", "/api/unittest/events", "2016-12-10T01:00:00+01:00")]
        public async Task EndToEnd_ReturnsPageForASlug_WithTimeframeCheck(string file, string path, string stringDate)
        {
            var date = DateTime.Parse(stringDate);
            StartServer(date);

            var expectedResponse = File.ReadAllText(GetFilePath(file));
            var contentResponse = JsonNormalize(expectedResponse);

            var response = await _client.GetAsync(path);
            var responseString = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonNormalize(responseString);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            parsedResponse.Should().Be(contentResponse);
        }

        [Fact]
        public async void ItReturnsAHealthcheck()
        {
            StartServer(DEFAULT_DATE);

            var response = await _client.GetAsync("/_healthcheck");
            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().Contain("appVersion");
            responseString.Should().Contain("sha");
        }

        private static string JsonNormalize(string jsonString)
        {
            return JsonConvert.SerializeObject(
                JsonConvert.DeserializeObject<dynamic>(jsonString, 
                new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.IsoDateFormat, DateTimeZoneHandling = DateTimeZoneHandling.Utc }));
        }

        private string contentfulUrlFor(string type, int referenceLevelLimit, string slug = null)
        {
            return slug == null
                ? $"https://test-host.com/spaces/XX/entries?access_token=XX&content_type={type}&include={referenceLevelLimit}"
                : $"https://test-host.com/spaces/XX/entries?access_token=XX&content_type={type}&include={referenceLevelLimit}&fields.slug={slug}";
        }

        private string contentfulUrlFor(string type, bool displayOnAz = false)
        {
            return $"https://test-host.com/spaces/XX/entries?access_token=XX&content_type={type}&fields.displayOnAZ={displayOnAz.ToString().ToLower()}";
        }

        private string contentfulUrlForTag(string type, int referenceLevelLimit, string tag)
        {
            return $"https://test-host.com/spaces/XX/entries?access_token=XX&content_type={type}&include={referenceLevelLimit}&fields.tags[in]={tag}";
        }

        private string contentfulUrlForCategory(string type, int referenceLevelLimit, string category)
        {
            return $"https://test-host.com/spaces/XX/entries?access_token=XX&content_type={type}&include={referenceLevelLimit}&fields.categories[in]={category}";
        }

        private static string GetFilePath(string file)
        {
            return $"Integration/ExpectedContentApiResponses/{file}.json";
        }

        private static HttpResponse CreateHttpResponse(string responseFile)
        {
            var jsonResponse = File.ReadAllText(responseFile);
            return HttpResponse.Successful(jsonResponse);
        }

        public void StartServer(DateTime date)
        {
            TestAppFactory.FakeTimeProvider.SetDateTime(date);
            var server = TestAppFactory.MakeFakeApp();
            _client = server.CreateClient();
        }
    }
}