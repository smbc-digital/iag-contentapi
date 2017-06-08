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
using StockportContentApi.ContentfulModels;
using StockportContentApi.Helpers;
using StockportContentApi.Http;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using StockportContentApiTests.Unit.Builders;
using File = System.IO.File;
using StockportContentApiTests.Unit.Repositories;

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
                fakeHttpClient.For(UrlFor("article", 2, "test-article")).Return(CreateHttpResponse("Unit/MockContentfulResponses/Article.json"));
                fakeHttpClient.For(UrlFor("article", 2, "about-us")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ArticleWithoutSections.json"));
                fakeHttpClient.For(UrlFor("article", 2, "test-me")).Return(CreateHttpResponse("Unit/MockContentfulResponses/ArticleWithParentTopic.json"));
                fakeHttpClient.For("https://buto-host.tv/video/kQl5D").Return(CreateHttpResponse("Unit/MockContentfulResponses/Article.json"));
                fakeHttpClient.For(UrlFor("startPage", 1, "new-start-page")).Return((CreateHttpResponse("Unit/MockContentfulResponses/StartPage.json")));
                fakeHttpClient.For(UrlFor("homepage", 2)).Return((CreateHttpResponse("Unit/MockContentfulResponses/Homepage.json")));
                fakeHttpClient.For(UrlFor("news", 1, limit: ContentfulQueryValues.LIMIT_MAX)).Return(CreateHttpResponse("Unit/MockContentfulResponses/NewsListing.json"));
                fakeHttpClient.For(UrlFor("newsroom", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/Newsroom.json"));
                fakeHttpClient.For(UrlFor("news", 1, tag: "Events", limit: ContentfulQueryValues.LIMIT_MAX)).Return(CreateHttpResponse("Unit/MockContentfulResponses/NewsListing.json"));
                fakeHttpClient.For(UrlFor("news", 1, category: "A category", limit: ContentfulQueryValues.LIMIT_MAX)).Return(CreateHttpResponse("Unit/MockContentfulResponses/NewsListing.json"));
                fakeHttpClient.For(UrlFor("article", displayOnAz: true)).Return(CreateHttpResponse("Unit/MockContentfulResponses/AtoZ.json"));
                fakeHttpClient.For(UrlFor("topic", displayOnAz: true)).Return(CreateHttpResponse("Unit/MockContentfulResponses/AtoZTopic.json"));
                fakeHttpClient.For(UrlFor("redirect")).Return(CreateHttpResponse("Unit/MockContentfulResponses/Redirects.json"));
                fakeHttpClient.For(UrlFor("footer", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/Footer.json"));
                fakeHttpClient.For(UrlFor("group", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/Group.json"));
                fakeHttpClient.For(UrlFor("contactUsId", 1)).Return(CreateHttpResponse("Unit/MockContentfulResponses/ContactUsId.json"));
                fakeHttpClient.For(ContentTypesUrlFor()).Return(CreateHttpResponse("Unit/MockContentfulResponses/ContentTypes.json"));
            });

            TestAppFactory.FakeContentfulClientFactory.MakeContentfulClientWithConfiguration(httpClient =>
            {
                httpClient.Setup(o => o.GetEntriesAsync<ContentfulEvent>(
                                It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.slug", "event_item").Include(2).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent> {
                                    new ContentfulEventBuilder().Slug("event_item").UpdatedAt(new DateTime(2016,10,5)).EventDate(new DateTime(2016, 12, 30)).Build()
                                });
                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() ==
                                new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").Include(2).Limit(ContentfulQueryValues.LIMIT_MAX).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent> {
                                    new ContentfulEventBuilder().Slug("event1").UpdatedAt(new DateTime(9999,9,9)).Build(),
                                    new ContentfulEventBuilder().Slug("event2").UpdatedAt(new DateTime(9999,9,9)).Build()
                                });
                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", "group_slug").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroup> {
                                    new ContentfulGroupBuilder().Slug("group_slug").Build()
                               });
                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<ContentfulNews>>(q => q.Build() == new QueryBuilder<ContentfulNews>().ContentTypeIs("news").FieldEquals("fields.slug", "news_item").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulNews> {
                                    new ContentfulNewsBuilder().Slug("news_item").Build()
                                });
                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<ContentfulTopic>>(q => q.Build() == new QueryBuilder<ContentfulTopic>().ContentTypeIs("topic").FieldEquals("fields.slug", "topic_slug").Include(2).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulTopic> {
                                    new ContentfulTopicBuilder().Slug("topic_slug").Build()
                                });
                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<ContentfulProfile>>(q => q.Build() ==
                                new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", "profile_slug").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulProfile> {
                                    new ContentfulProfileBuilder().Slug("profile_slug").Build()
                                });
                httpClient.Setup(o => o.GetEntriesAsync(
                               It.Is<QueryBuilder<Entry<ContentfulArticle>>>(q => q.Build() ==
                               new QueryBuilder<Entry<ContentfulArticle>>().ContentTypeIs("article").FieldEquals("fields.slug", "test-article").Include(3).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(new List<Entry<ContentfulArticle>> {
                                    new ContentfulEntryBuilder<ContentfulArticle>().Fields(
                                    new ContentfulArticleBuilder().Slug("test-article").Build()
                                    ).Build()
                               });
                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<Entry<ContentfulArticle>>>(q => q.Build() ==
                                new QueryBuilder<Entry<ContentfulArticle>>().ContentTypeIs("article").FieldEquals("fields.slug", "about-us").Include(3).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<Entry<ContentfulArticle>> {
                                    new ContentfulEntryBuilder<ContentfulArticle>().Fields(
                                    new ContentfulArticleBuilder().Slug("about-us").WithOutSection().Build()
                                    ).Build()
                                });
                httpClient.Setup(o => o.GetEntriesAsync(
                               It.Is<QueryBuilder<Entry<ContentfulArticle>>>(q => q.Build() ==
                               new QueryBuilder<Entry<ContentfulArticle>>().ContentTypeIs("article").FieldEquals("fields.slug", "test-me").Include(3).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(new List<Entry<ContentfulArticle>> {
                                    new ContentfulEntryBuilder<ContentfulArticle>().Fields(
                                    new ContentfulArticleBuilder().Slug("test-me").Build()
                                    ).Build()
                               });
                httpClient.Setup(o => o.GetEntriesAsync(
                                    It.Is<QueryBuilder<ContentfulPayment>>(q => q.Build() == new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", "payment_slug").Include(1).Build()),
                                    It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulPayment> {
                                    new ContentfulPaymentBuilder().Slug("payment_slug").Build()
                                    });

                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<ContentfulContactUsId>>(q => q.Build() == new QueryBuilder<ContentfulContactUsId>().ContentTypeIs("contactUsId").FieldEquals("fields.slug", "test-email").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulContactUsId> {
                                    new ContentfulContactUsId() {Slug = "test-email", EmailAddress = "test@stockport.gov.uk", Name = "Test email"}
                                });

                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", "showcase_slug").Include(3).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulShowcase> {
                                    new ContentfulShowcaseBuilder().Slug("showcase_slug")
                                        .Consultations(new List<ContentfulConsultation>() { new ContentfulConsultation() { Title = "title", ClosingDate = DateTime.MinValue, Link = "http://link.url" } })
                                        .SocialMediaLinks(new List<ContentfulSocialMediaLink>() { new ContentfulSocialMediaLink() { Title = "sm-link-title", Slug = "sm-link-slug", Url = "http://link.url" , Icon = "sm-link-icon" } })
                                        .Build()
                                });

                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroupCategory> {
                                new ContentfulGroupCategoryBuilder().Slug("groupCategory_slug").Build()
                                });

                httpClient.Setup(o => o.GetEntriesAsync(
                                It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.group.sys.contentType.sys.id", "group").FieldEquals("fields.group.fields.slug", "group_slug").Include(2).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulEvent> {
                                new ContentfulEventBuilder().Slug("event-slug").Build()
                                });
            });
        }

        [Theory]
        [InlineData("StartPage", "/api/unittest/start-page/new-start-page")]
        [InlineData("Profile", "/api/unittest/profile/profile_slug")]
        [InlineData("Topic", "/api/unittest/topic/topic_slug")]
        [InlineData("ArticleWithoutSections", "/api/unittest/article/about-us")]
        [InlineData("Article", "/api/unittest/article/test-article")]
        [InlineData("Homepage", "/api/unittest/homepage")]
        [InlineData("ArticleWithParentTopic", "/api/unittest/article/test-me")]
        [InlineData("AtoZ", "/api/unittest/atoz/v")]
        [InlineData("AtoZTopic", "/api/unittest/atoz/b")]
        [InlineData("AtoZArticleAndTopic", "/api/unittest/atoz/c")]
        [InlineData("RedirectDictionary", "/api/redirects")]
        [InlineData("Footer", "/api/unittest/footer")]
        [InlineData("Group", "/api/unittest/group/group_slug")]
        [InlineData("Payment", "/api/unittest/payment/payment_slug")]
        [InlineData("Showcase", "/api/unittest/showcase/showcase_slug")]
        [InlineData("GroupCategory", "/api/unittest/groupCategory")]
        [InlineData("ContactUsId", "/api/unittest/contactUsId/test-email")]
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
        [InlineData("Event", "/api/unittest/events/event_item?date=2016-12-30", "2016-12-10T01:00:00+01:00")]
        [InlineData("EventsCalendar", "/api/unittest/events", "2016-12-10T01:00:00+01:00")]
        [InlineData("EventsLatest", "/api/unittest/events/latest/1", "2016-12-10T01:00:00+01:00")]
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

        private static string UrlFor(string type, int referenceLevelLimit = -1, string slug = null,
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