﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Newtonsoft.Json;
using Xunit;
using HttpClient = System.Net.Http.HttpClient;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using StockportContentApiTests.Unit.Builders;
using Microsoft.Extensions.Configuration;

namespace StockportContentApiTests.Integration
{
    public class RoutesTest : TestingBaseClass
    {
        private HttpClient _client;
        private readonly DateTime DEFAULT_DATE = new DateTime(2016, 09, 01);
        private const string ENTRIES_BASE_URL = "https://test-host.com/spaces/XX/entries?access_token=XX";
        private const string CONTENT_TYPES_BASE_URL = "https://test-host.com/spaces/XX/content_types?access_token=XX";
        private readonly Mock<ICache> _cache;
        private readonly Mock<IConfiguration> _configuration;

        public RoutesTest()
        {
            _configuration = new Mock<IConfiguration>();
            _configuration.Setup(_ => _["redisExpiryTimes:AtoZ"]).Returns("60");
            _cache = new Mock<ICache>();

            TestAppFactory.FakeContentfulClientFactory.MakeContentfulClientWithConfiguration(httpClient =>
            {
                var eventCollection = new ContentfulCollection<ContentfulEvent>();
                eventCollection.Items = new List<ContentfulEvent>
                {
                    new ContentfulEventBuilder().Slug("event_item").UpdatedAt(new DateTime(2016,10,5)).EventDate(new DateTime(2016, 12, 30)).Build()
                };
                httpClient.Setup(o => o.GetEntries<ContentfulEvent>(
                                It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.slug", "event_item").Include(2).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(eventCollection);

                eventCollection = new ContentfulCollection<ContentfulEvent>();
                eventCollection.Items = new List<ContentfulEvent>
                {
                    new ContentfulEventBuilder().Slug("event1").UpdatedAt(new DateTime(9999,9,9)).Build(),
                    new ContentfulEventBuilder().Slug("event2").UpdatedAt(new DateTime(9999,9,9)).Build()
                };
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() ==
                                new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").Include(2).Limit(ContentfulQueryValues.LIMIT_MAX).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(eventCollection);

                var groupCollection = new ContentfulCollection<ContentfulGroup>();
                groupCollection.Items = new List<ContentfulGroup>
                {
                    new ContentfulGroupBuilder().Slug("zumba-fitness").Build()
                };
                httpClient.Setup(o => o.GetEntries(
                    It.IsAny<QueryBuilder<ContentfulGroup>>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(groupCollection);

                httpClient.Setup(o => o.GetEntries<ContentfulGroup>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(groupCollection);

                var newsCollection = new ContentfulCollection<ContentfulNews>();
                newsCollection.Items = new List<ContentfulNews>
                {
                    new ContentfulNewsBuilder().Slug("news_item").SunriseDate(DateTime.MinValue).SunsetDate(new DateTime(9999, 09, 09, 0, 0, 0, DateTimeKind.Utc)).Document().Build(),
                };
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulNews>>(q => q.Build() == new QueryBuilder<ContentfulNews>().ContentTypeIs("news").FieldEquals("fields.slug", "news_item").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(newsCollection);

                var newsListCollection = new ContentfulCollection<ContentfulNews>();
                newsListCollection.Items = new List<ContentfulNews>
                {
                    new ContentfulNewsBuilder().Title("Another news article").Slug("another-news-article").Teaser("This is another news article").SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc)).Build(),
                    new ContentfulNewsBuilder().Title("This is the news").Slug("news-of-the-century").Teaser("Read more for the news").SunriseDate(new DateTime(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc)).SunsetDate(new DateTime(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc)).Build(),
                };
                httpClient.Setup(o => o.GetEntries<ContentfulNews>(
                                It.Is<string>(q => !q.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").FieldEquals("fields.slug", "news_item").Include(1).Build())),
                               It.IsAny<CancellationToken>())).ReturnsAsync(newsListCollection);

                var newsContent = new ContentType()
                {
                    Fields = new List<Field>()
                    {
                        new Field()
                        {
                            Name = "Categories",
                            Items = new Contentful.Core.Models.Schema()
                            {
                                Validations = new List<IFieldValidator>()
                                {
                                    new InValuesValidator {RequiredValues = new List<string>() { "Benefits","Business","Council leader","Crime prevention and safety","Children and families","Environment","Elections","Health and social care","Housing","Jobs","Leisure and culture","Libraries","Licensing","Partner organisations","Planning and building","Roads and travel","Schools and education","Waste and recycling","Test Category" } }
                                }
                            }
                        }
                    }
                };
               
                httpClient.Setup(o => o.GetContentType("news", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(newsContent);


                var newsroomCollection = new ContentfulCollection<ContentfulNewsRoom>();
                newsroomCollection.Items = new List<ContentfulNewsRoom>
                {
                    new ContentfulNewsRoomBuilder().Build()
                };

                httpClient.Setup(o => o.GetEntries(
                               It.Is<QueryBuilder<ContentfulNewsRoom>>(q => q.Build() == new QueryBuilder<ContentfulNewsRoom>().ContentTypeIs("newsroom").Include(1).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(newsroomCollection);

               var topicCollection = new ContentfulCollection<ContentfulTopic>();
                topicCollection.Items = new List<ContentfulTopic>
                {
                    new ContentfulTopicBuilder().Slug("topic_slug").Breadcrumbs(new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("id").Build()}).Build()
                };
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulTopic>>(q => q.Build() == new QueryBuilder<ContentfulTopic>().ContentTypeIs("topic").FieldEquals("fields.slug", "topic_slug").Include(2).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(topicCollection);

                var profileCollection = new ContentfulCollection<ContentfulProfile>();
                profileCollection.Items = new List<ContentfulProfile>
                {
                    new ContentfulProfileBuilder().Slug("profile_slug").Build()
                };
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulProfile>>(q => q.Build() ==
                                new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", "profile_slug").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(profileCollection);

                var articleCollection = new ContentfulCollection<ContentfulArticle>();
                articleCollection.Items = new List<ContentfulArticle>
                {
                    new ContentfulArticleBuilder().Breadcrumbs(new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build()}).Slug("test-article").Build()
                };
                httpClient.Setup(o => o.GetEntries(
                               It.Is<QueryBuilder<ContentfulArticle>>(q => q.Build() ==
                               new QueryBuilder<ContentfulArticle>().ContentTypeIs("article").FieldEquals("fields.slug", "test-article").Include(3).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(articleCollection);

                articleCollection = new ContentfulCollection<ContentfulArticle>();
                articleCollection.Items = new List<ContentfulArticle>
                {
                    new ContentfulArticleBuilder().Slug("about-us").WithOutSection().Breadcrumbs(new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build()}).Build()
                };
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulArticle>>(q => q.Build() ==
                                new QueryBuilder<ContentfulArticle>().ContentTypeIs("article").FieldEquals("fields.slug", "about-us").Include(3).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(articleCollection);

                articleCollection = new ContentfulCollection<ContentfulArticle>();
                articleCollection.Items = new List<ContentfulArticle>
                {
                    new ContentfulArticleBuilder().Slug("test-me").Breadcrumbs(new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build()}).Build()
                };
                httpClient.Setup(o => o.GetEntries(
                               It.Is<QueryBuilder<ContentfulArticle>>(q => q.Build() ==
                               new QueryBuilder<ContentfulArticle>().ContentTypeIs("article").FieldEquals("fields.slug", "test-me").Include(3).Build()),
                               It.IsAny<CancellationToken>())).ReturnsAsync(articleCollection);

                var paymentCollection = new ContentfulCollection<ContentfulPayment>();
                paymentCollection.Items = new List<ContentfulPayment>
                {
                    new ContentfulPaymentBuilder().Slug("payment_slug").Build()
                };
                httpClient.Setup(o => o.GetEntries(
                                    It.Is<QueryBuilder<ContentfulPayment>>(q => q.Build() == new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", "payment_slug").Include(1).Build()),
                                    It.IsAny<CancellationToken>())).ReturnsAsync(paymentCollection);

                var contactUsIdCollection = new ContentfulCollection<ContentfulContactUsId>();
                contactUsIdCollection.Items = new List<ContentfulContactUsId>
                {
                    new ContentfulContactUsId() {Slug = "test-email", EmailAddress = "test@stockport.gov.uk", Name = "Test email", SuccessPageReturnUrl = "test button url", SuccessPageButtonText = "test button text"}
                };
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulContactUsId>>(q => q.Build() == new QueryBuilder<ContentfulContactUsId>().ContentTypeIs("contactUsId").FieldEquals("fields.slug", "test-email").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(contactUsIdCollection);

                var showcaseCollection = new ContentfulCollection<ContentfulShowcase>();
                showcaseCollection.Items = new List<ContentfulShowcase>
                {
                    new ContentfulShowcaseBuilder().Slug("showcase_slug")
                                        .Consultations(new List<ContentfulConsultation>() { new ContentfulConsultation() { Title = "title", ClosingDate = DateTime.MinValue, Link = "http://link.url" } })
                                        .SocialMediaLinks(new List<ContentfulSocialMediaLink>() { new ContentfulSocialMediaLink() { Title = "sm-link-title", Slug = "sm-link-slug", Url = "http://link.url" , Icon = "sm-link-icon" } })
                                        .Build()
                };
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", "showcase_slug").Include(3).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(showcaseCollection);

                var footerCollection = new ContentfulCollection<ContentfulFooter>();
                footerCollection.Items = new List<ContentfulFooter>
                {
                   new ContentfulFooterBuilder().Build()
                };

               httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulFooter>>(q => q.Build() == new QueryBuilder<ContentfulFooter>().ContentTypeIs("footer").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(footerCollection);

                var catGroupCollection = new ContentfulCollection<ContentfulGroupCategory>();
                catGroupCollection.Items = new List<ContentfulGroupCategory>
                {
                    new ContentfulGroupCategoryBuilder().Slug("groupCategory_slug").Build()
                };
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(catGroupCollection);

                eventCollection = new ContentfulCollection<ContentfulEvent>();
                eventCollection.Items = new List<ContentfulEvent>
                {
                    new ContentfulEventBuilder().Slug("event-slug").Build()
                };
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.group.sys.contentType.sys.id", "group").FieldEquals("fields.group.fields.slug", "zumba-fitness").Include(2).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(eventCollection);
                
                var homepageCollection = new ContentfulCollection<ContentfulHomepage>();
                homepageCollection.Items = new List<ContentfulHomepage>
                {
                    new ContentfulHomepageBuilder().Build()
                };

                var homepageBuilder = new QueryBuilder<ContentfulHomepage>().ContentTypeIs("homepage").Include(2);
                httpClient.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulHomepage>>(q => q.Build() == homepageBuilder.Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(homepageCollection);

                var smartAnswer = new ContentfulCollection<ContentfulSmartAnswers>();
                smartAnswer.Items = new List<ContentfulSmartAnswers>()
                {
                    new ContentfulSmartAnswerBuilder().Slug("smartAnswer_slug").Build()
                };
                httpClient.Setup(o => o.GetEntries(
                    It.Is<QueryBuilder<ContentfulSmartAnswers>>(
                        q => q.Build() == new QueryBuilder<ContentfulSmartAnswers>().ContentTypeIs("smartAnswers")
                                 .FieldEquals("fields.slug", "smartAnswer_slug").Include(1).Build()),
                    It.IsAny<CancellationToken>())).ReturnsAsync(smartAnswer);

                var Redirects = new ContentfulCollection<ContentfulRedirect>();
                Redirects.Items = new List<ContentfulRedirect>()
                {
                    new ContentfulRedirectBuilder().BuildForRouteTest()
                };
                httpClient.Setup(o => o.GetEntries(
                    It.Is<QueryBuilder<ContentfulRedirect>>(
                        q => q.Build() == new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1).Build()),
                    It.IsAny<CancellationToken>())).ReturnsAsync(Redirects);

                var startPages = new ContentfulCollection<ContentfulStartPage>();
                startPages.Items = new List<ContentfulStartPage>()
                {
                    new ContentfulStartPageBuilder().Slug("new-start-page").Build()
                };
                httpClient.Setup(o => o.GetEntries(
                    It.Is<QueryBuilder<ContentfulStartPage>>(
                        q => q.Build() == new QueryBuilder<ContentfulStartPage>().ContentTypeIs("startPage").FieldEquals("fields.slug", "new-start-page").Include(3).Build()),
                    It.IsAny<CancellationToken>())).ReturnsAsync(startPages);
                
                var organisations = new ContentfulCollection<ContentfulOrganisation>();
                organisations.Items = new List<ContentfulOrganisation>()
                {
                    new ContentfulOrganisation()
                    {
                        AboutUs = "about us",
                        Email = "email",
                        Image = null,
                        Phone = "phone",
                        Slug = "slug",
                        Title = "title",
                        Volunteering = true,
                        VolunteeringText = "help wanted"
                    }
                };
                httpClient.Setup(o => o.GetEntries(
                    It.Is<QueryBuilder<ContentfulOrganisation>>(
                        q => q.Build() == new QueryBuilder<ContentfulStartPage>().ContentTypeIs("organisation")
                                 .FieldEquals("fields.slug", "slug").Build()),
                    It.IsAny<CancellationToken>())).ReturnsAsync(organisations);

                var groupHomepage = new ContentfulGroupHomepageBuilder().Build();

                var groupHomepageCollection = new ContentfulCollection<ContentfulGroupHomepage>
                {
                    Items = new List<ContentfulGroupHomepage>() { groupHomepage }
                };

                httpClient.Setup(o => o.GetEntries(
                    It.Is<QueryBuilder<ContentfulGroupHomepage>>(
                        q => q.Build() == new QueryBuilder<ContentfulGroupHomepage>().ContentTypeIs("groupHomepage").Include(1).Build()),
                    It.IsAny<CancellationToken>())).ReturnsAsync(groupHomepageCollection);

                var groupAdvisor = new ContentfulCollection<ContentfulGroupAdvisor>
                {
                    Items = new List<ContentfulGroupAdvisor> { new ContentfulGroupAdvisorBuilder().Email("testemail@notandomain.xyz").Build() }
                };

                httpClient.Setup(o => o.GetEntries(
                    It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "testemail@notandomain.xyz").Include(1).Build()),
                    It.IsAny<CancellationToken>())).ReturnsAsync(groupAdvisor);

                httpClient.Setup(o => o.GetEntries(
                    It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").Include(1).Build()),
                    It.IsAny<CancellationToken>())).ReturnsAsync(groupAdvisor);

                var privacyNotice = new ContentfulCollection<ContentfulPrivacyNotice>()
                {
                    Items = new List<ContentfulPrivacyNotice>()
                    {
                        new ContentfulPrivacyNotice()
                        {
                            Slug = "test-slug",
                            Title = "test-title",
                            Category = "test-category",
                            Purpose = "test-purpose",
                            TypeOfData = "test-type-of-data",
                            Legislation = "test-legislation",
                            Obtained = "test-obtained",
                            ExternallyShared = "test-externally-shared",
                            RetentionPeriod = "test-retention-period",
                            OutsideEu = false,
                            AutomatedDecision = false,
                            UrlOne = "test-url-1",
                            UrlTwo = "test-url-2",
                            UrlThree = "test-url-3"
                        }
                    }
                };

                httpClient.Setup(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulPrivacyNotice>>(q => q.Build() == new QueryBuilder<ContentfulPrivacyNotice>().ContentTypeIs("privacyNotice").FieldEquals("fields.slug", "slug").Include(3).Build()), It.IsAny<CancellationToken>())).ReturnsAsync(privacyNotice);

                var privacyNotices = new ContentfulCollection<ContentfulPrivacyNotice>()
                {
                    Items = new List<ContentfulPrivacyNotice>()
                    {
                        new ContentfulPrivacyNotice()
                        {
                            Slug = "test-slug-1",
                            Title = "test-title-1",
                            Category = "test-category-1",
                            Purpose = "test-purpose-1",
                            TypeOfData = "test-type-of-data-1",
                            Legislation = "test-legislation-1",
                            Obtained = "test-obtained-1",
                            ExternallyShared = "test-externally-shared-1",
                            RetentionPeriod = "test-retention-period-1",
                            OutsideEu = false,
                            AutomatedDecision = false,
                            UrlOne = "test-url-1-1",
                            UrlTwo = "test-url-2-1",
                            UrlThree = "test-url-3-1"
                        },
                        new ContentfulPrivacyNotice()
                        {
                            Slug = "test-slug-2",
                            Title = "test-title-2",
                            Category = "test-category-2",
                            Purpose = "test-purpose-2",
                            TypeOfData = "test-type-of-data-2",
                            Legislation = "test-legislation-2",
                            Obtained = "test-obtained-2",
                            ExternallyShared = "test-externally-shared-2",
                            RetentionPeriod = "test-retention-period-2",
                            OutsideEu = false,
                            AutomatedDecision = false,
                            UrlOne = "test-url-1-2",
                            UrlTwo = "test-url-2-2",
                            UrlThree = "test-url-3-2"
                        }
                    }
                };

                httpClient.Setup(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulPrivacyNotice>>(q => q.Build() == new QueryBuilder<ContentfulPrivacyNotice>().ContentTypeIs("privacyNotice").Include(6).Limit(1000).Build()), It.IsAny<CancellationToken>())).ReturnsAsync(privacyNotices);
            });
        }

        [Theory]
        [InlineData("StartPage", "/unittest/start-page/new-start-page")]
        [InlineData("Profile", "/unittest/profiles/profile_slug")]
        [InlineData("Topic", "/unittest/topics/topic_slug")]
        [InlineData("Homepage", "/unittest/homepage")]
        [InlineData("AtoZ", "/unittest/atoz/v")]
        [InlineData("AtoZTopic", "/unittest/atoz/b")]
        [InlineData("AtoZArticleAndTopic", "/unittest/atoz/c")]
        [InlineData("RedirectDictionary", "/redirects")]
        [InlineData("Footer", "/unittest/footer")]
        [InlineData("Payment", "/unittest/payments/payment_slug")]
        //[InlineData("Showcase", "/unittest/showcases/showcase_slug")]
        [InlineData("GroupCategory", "/unittest/group-categories")]
        [InlineData("ContactUsId", "/unittest/contact-us-id/test-email")]
        [InlineData("Organisation", "/unittest/organisations/slug")]
        [InlineData("GroupHomePage", "/unittest/grouphomepage")]
        [InlineData("GroupAdvisor", "/unittest/groups/advisors/testemail@notandomain.xyz")]
        [InlineData("GroupAdvisorList", "/unittest/groups/slug/advisors")]
        [InlineData("PrivacyNotice", "/unittest/privacy-notices/slug")]
        public async Task EndToEnd_ReturnsPageForASlug(string file, string path)
        {
            StartServer(DEFAULT_DATE);

            var expectedResponse = GetStringResponseFromFile($"StockportContentApiTests.Integration.ExpectedContentApiResponses.{file}.json");
            var contentResponse = JsonNormalize(expectedResponse);

            var response = await _client.GetAsync(path);
            var responseString = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonNormalize(responseString);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            parsedResponse.Should().Be(contentResponse);
        }

        [Theory]
        [InlineData("News", "/unittest/news/news_item", "2016-08-10T01:00:00+01:00")]
        [InlineData("NewsListing", "/unittest/news", "2016-08-10T01:00:00+01:00")]
        [InlineData("NewsListing", "/unittest/news?tag=Events", "2016-08-10T01:00:00+01:00")]
        [InlineData("NewsListing", "/unittest/news?category=A category", "2016-08-10T01:00:00+01:00")]
        [InlineData("NewsListingFilteredByDate", "/unittest/news?dateFrom=2016-06-01&dateTo=2016-08-31", "2017-08-02T01:00:00+01:00")]
        [InlineData("Event", "/unittest/events/event1?date=9999-09-09", "2016-12-10T01:00:00+01:00")]
        [InlineData("EventsCalendar", "/unittest/events", "2016-12-10T01:00:00+01:00")]
        [InlineData("EventsLatest", "/unittest/events/latest/1", "2016-12-10T01:00:00+01:00")]
        public async Task EndToEnd_ReturnsPageForASlug_WithTimeframeCheck(string file, string path, string stringDate)
        {
            var date = DateTime.Parse(stringDate);
            StartServer(date);

            var expectedResponse = GetStringResponseFromFile($"StockportContentApiTests.Integration.ExpectedContentApiResponses.{file}.json");
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

        private void StartServer(DateTime date)
        {
            TestAppFactory.FakeTimeProvider.SetDateTime(date);
            var server = TestAppFactory.MakeFakeApp();
            _client = server.CreateClient();
        }
    }
}