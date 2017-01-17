using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Newtonsoft.Json;
using Xunit;
using HttpClient = System.Net.Http.HttpClient;
using FluentAssertions;
using Moq;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using File = System.IO.File;

namespace StockportContentApiTests.Integration
{
    public class RoutesTest
    {
        private HttpClient _client;
        private readonly DateTime DEFAULT_DATE = new DateTime(2016, 09, 01);
        private const string ENTRIES_BASE_URL = "https://test-host.com/spaces/XX/entries?access_token=XX";
        private const string CONTENT_TYPES_BASE_URL = "https://test-host.com/spaces/XX/content_types?access_token=XX";

        public RoutesTest()
        {
            TestAppFactory.FakeHttpClientFactory.MakeFakeHttpClientWithConfiguration(fakeHttpClient =>
            {
                fakeHttpClient.For(UrlFor("topic", 1, "test-topic")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ListOfArticlesForTopic.json"));
                fakeHttpClient.For(UrlFor("topic", 1, "test-topic-with-subtopic")).Return(CreateHttpResponse("Unit/MockContentfulResponses/TopicWithPrimarySecondaryAndTertiaryItems.json"));
                fakeHttpClient.For(UrlFor("topic", 1, "test-topic-with-alerts")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ListOfArticlesForTopicWithAlerts.json"));
                fakeHttpClient.For(UrlFor("article", 2, "test-article")).Return(CreateHttpResponse("Unit/MockContentfulResponses/Article.json"));
                fakeHttpClient.For(UrlFor("article", 2, "about-us")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ArticleWithoutSections.json"));
                fakeHttpClient.For(UrlFor("article", 2, "test-me")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ArticleWithParentTopic.json"));
                fakeHttpClient.For("https://buto-host.tv/video/kQl5D").Return(CreateHttpResponse("Unit/MockContentfulResponses/Article.json"));
                fakeHttpClient.For(UrlFor("profile", 1, "test-profile")).Return((CreateHttpResponse("Unit/MockContentfulResponses/ProfileWithBreadcrumbs.json")));
                fakeHttpClient.For(UrlFor("startPage", 1, "new-start-page")).Return((CreateHttpResponse("Unit/MockContentfulResponses/StartPage.json")));
                fakeHttpClient.For(UrlFor("homepage", 2)).Return((CreateHttpResponse("Unit/MockContentfulResponses/Homepage.json")));
                fakeHttpClient.For(UrlFor("news", 1, "news_item")).Return(CreateHttpResponse("Unit/MockContentfulResponses/News.json"));
                fakeHttpClient.For(UrlFor("news", 1, limit: ContentfulQueryValues.LIMIT_MAX)).Return(CreateHttpResponse("Unit/MockContentfulResponses/NewsListing.json"));
                fakeHttpClient.For(UrlFor("newsroom", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/Newsroom.json"));
                fakeHttpClient.For(UrlFor("news", 1, tag: "Events", limit: ContentfulQueryValues.LIMIT_MAX)).Return(CreateHttpResponse("Unit/MockContentfulResponses/NewsListing.json"));
                fakeHttpClient.For(UrlFor("news", 1, category: "A category", limit: ContentfulQueryValues.LIMIT_MAX)).Return(CreateHttpResponse("Unit/MockContentfulResponses/NewsListing.json"));
                fakeHttpClient.For(UrlFor("article", displayOnAz: true)).Return(CreateHttpResponse("Unit/MockContentfulResponses/AtoZ.json"));             
                fakeHttpClient.For(UrlFor("topic", displayOnAz: true)).Return(CreateHttpResponse("Unit/MockContentfulResponses/AtoZTopic.json"));             
                fakeHttpClient.For(UrlFor("redirect")).Return(CreateHttpResponse("Unit/MockContentfulResponses/Redirects.json"));
                fakeHttpClient.For(UrlFor("footer", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/Footer.json"));
                fakeHttpClient.For(ContentTypesUrlFor()).Return(CreateHttpResponse("Unit/MockContentfulResponses/ContentTypes.json"));
            });   

            TestAppFactory.FakeContentfulClientFactory.MakeContentfulClientWithConfiguration(httpClient =>
            {
                httpClient.Setup(o => o.GetEntriesAsync<Event>(
                                It.Is<QueryBuilder>(q => q.Build() == new QueryBuilder().ContentTypeIs("events").FieldEquals("fields.slug", "event_item").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> {
                                    new Event("This is the event", "event-of-the-century", "Read more for the event", "", "The event  description", 
                                        "Free", "Bramall Hall, Carpark, SK7 6HG", "Friends of Stockport", "", "", false, 
                                        new DateTime(2016, 12, 30, 0, 0, 0, DateTimeKind.Utc), "10:00", "17:00",  0, EventFrequency.None, new List<Crumb> { new Crumb("Events", "", "events") })});
                httpClient.Setup(o => o.GetEntriesAsync<Event>(
                                It.Is<QueryBuilder>(q => q.Build() == new QueryBuilder().ContentTypeIs("events").Include(1).Limit(ContentfulQueryValues.LIMIT_MAX).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event> {
                                    new Event("This is the event", "event-of-the-century", "Read more for the event", "", "The event  description",
                                        "Free", "Bramall Hall, Carpark, SK7 6HG", "Friends of Stockport", "", "", false,
                                        new DateTime(2016, 12, 30, 0, 0, 0, DateTimeKind.Utc), "10:00", "17:00",  0, EventFrequency.None, new List<Crumb> { new Crumb("Events", "", "events") }),
                                    new Event("This is the second event", "second-event", "Read more for the event", "", "The event  description",
                                        "Free", "Bramall Hall, Carpark, SK7 6HG", "Friends of Stockport", "", "", false,
                                        new DateTime(2016, 12, 30, 0, 0, 0, DateTimeKind.Utc), "10:00", "17:00",  0, EventFrequency.None, new List<Crumb> { new Crumb("Events", "", "events") })});
                httpClient.Setup(o => o.GetEntriesAsync<ContentfulNews>(
                                It.Is<QueryBuilder>(q => q.Build() == new QueryBuilder().ContentTypeIs("news").FieldEquals("fields.slug", "news_item").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulNews> {
                                    new ContentfulNews {Title = "This is the news", Slug = "news-of-the-century", Teaser = "Read more for the news", Image = new Asset { File = new Contentful.Core.Models.File {Url = "image.jpg"} }, Body = "The news {{PDF:Stockport-Metroshuttle.pdf}} {{PDF:a-pdf.pdf}}",
                                        SunriseDate = new DateTime(2016, 07, 09, 23, 0, 0, DateTimeKind.Utc), SunsetDate = new DateTime(2016, 8, 23, 23, 0, 0, DateTimeKind.Utc),
                                        Breadcrumbs = new List<Crumb> { new Crumb("News", "", "news")}, Alerts = new List<Alert> { new Alert("New alert", "alert sub heading updated", "Alert body", "Error", new DateTime(2016, 6, 30, 23, 0, 0, DateTimeKind.Utc), new DateTime(2016, 9, 29, 23, 0, 0, DateTimeKind.Utc)) },
                                        Tags = new List<string> { "Bramall Hall", "Events" }, 
                                        Documents = new List<Asset> { new Asset { Description = "metroshuttle route map", File = new Contentful.Core.Models.File {Url = "document.pdf" , FileName = "Stockport-Metroshuttle.pdf", Details = new FileDetails {Size = 674192 }}, SystemProperties = new SystemProperties
                                        {
                                            UpdatedAt = new DateTime(2016, 10, 5, 11, 09, 48, DateTimeKind.Utc) 
                                        }}},
                                        Categories = new List<string> { "Category 1", "Category 2" } } });
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

        private static string ContentTypesUrlFor()
        {
            return CONTENT_TYPES_BASE_URL;
        }

        private string UrlFor(string type, int referenceLevelLimit = -1, string slug = null,
            bool displayOnAz = false, string tag = null, string category = null, int limit = -1)
        {
            var url = $"{ENTRIES_BASE_URL}&content_type={type}";
            if (referenceLevelLimit >= 0) url = $"{url}&include={referenceLevelLimit}";
            if (!string.IsNullOrEmpty(slug)) url = $"{url}&fields.slug={slug}";
            if (displayOnAz) url = $"{url}&fields.displayOnAZ={displayOnAz.ToString().ToLower()}";
            if (!string.IsNullOrEmpty(tag)) url = $"{url}&fields.tags[in]={tag}";
            if (!string.IsNullOrEmpty(category)) url = $"{url}&fields.categories[in]={category}";
            if (limit >= 0) url = $"{url}&limit={limit}";

            return url;
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