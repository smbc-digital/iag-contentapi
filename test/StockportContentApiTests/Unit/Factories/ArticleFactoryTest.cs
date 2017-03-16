using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class ArticleFactoryTest
    {
        private readonly ArticleFactory _articleFactory;
        private readonly Mock<IFactory<Topic>> _mockTopicBuilder;
        private readonly Mock<IBuildContentTypesFromReferences<Alert>> _mockAlertListFactory;
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly IBuildContentTypesFromReferences<Crumb> _breadcrumbFactory;
        private readonly IBuildContentTypesFromReferences<Section> _sectionListFactory;
        private readonly IBuildContentTypesFromReferences<Profile> _profileListFactory;
        private readonly Mock<IBuildContentTypesFromReferences<Document>> _mockDocumentListFactory;
        private readonly Mock<IBuildContentTypeFromReference<LiveChat>> _liveChatListFactory;

        private readonly List<Document> _documents = new List<Document>() { new Document("Title", 1212, new DateTime(2016, 12, 08), "/thisisaurl", "filename1.pdf"),
                                                                            new Document("Title 2", 3412, new DateTime(2016, 12, 09), "/anotherurl", "filename2.pdf") };

        private readonly LiveChat _liveChat = new LiveChat("Title", "Text");
        private List<LiveChat> _liveChats = new List<LiveChat>();

        public ArticleFactoryTest()
        {
            _mockTimeProvider = new Mock<ITimeProvider>();
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 01));

            _mockTopicBuilder = new Mock<IFactory<Topic>>();
            _mockTopicBuilder.Setup(
              o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
              .Returns(new Topic("main-topic", "Main Topic", "teaser", "summary", "", "", "", new List<SubItem>(), new List<SubItem>(),
              new List<SubItem>(), new List<Crumb>(), new List<Alert>(), DateTime.MinValue, DateTime.MinValue, false, string.Empty));

            _mockAlertListFactory = new Mock<IBuildContentTypesFromReferences<Alert>>();
            _mockAlertListFactory.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<Alert>());

            _mockDocumentListFactory = new Mock<IBuildContentTypesFromReferences<Document>>();
            _mockDocumentListFactory.Setup(
                    o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(_documents);

            _liveChatListFactory = new Mock<IBuildContentTypeFromReference<LiveChat>>();

            _liveChats.Add(_liveChat);
            _liveChatListFactory.Setup(
                    o => o.BuildFromReference(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(_liveChat);


            _breadcrumbFactory = new BreadcrumbFactory();
            _sectionListFactory = new SectionListFactory(new ProfileListFactory(), _mockDocumentListFactory.Object, _mockTimeProvider.Object, _mockAlertListFactory.Object);
            _profileListFactory = new ProfileListFactory();
            _articleFactory = new ArticleFactory(_mockTopicBuilder.Object, _mockAlertListFactory.Object, _breadcrumbFactory, _sectionListFactory, _profileListFactory, _mockDocumentListFactory.Object, _liveChatListFactory.Object);
        }

        [Fact]
        public void BuildsArticle()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.Title.Should().Be("Unit Test Article");
            article.Slug.Should().Be("unit-test-article");
            article.Teaser.Should().Be("An article made for unit testing");
            article.Body.Should().Be("An article made for unit testing body text\n\n{{PROFILE: profile-no-pic}}");
            article.Icon.Should().Be("icon");
            article.BackgroundImage.Should().Be("image.jpg");
            article.Image.Should().Be("image.jpg");
            article.Profiles.First().Title.Should().Be("Profile with no pic");
            article.Profiles.First().Slug.Should().Be("profile-no-pic");
            article.Sections.Should().HaveCount(1);
            article.Documents.Should().BeEquivalentTo(_documents);
            article.LiveChatVisible.Should().Be(true);
            article.LiveChat.Title.Should().Be("Title");
            article.LiveChat.Text.Should().NotBeEmpty();
        }

        [Fact]
        public void BuildsArticleWithNoSectionsFromContentfulResponseData()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithoutSections.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.Title.Should().Be("About Us");
            article.Slug.Should().Be("about-us");
            article.Teaser.Should().Be("Teaser");
            article.Body.Should().Be("Content {{PROFILE:test-profile}} test video: {{VIDEO:kQl5D}}");
            article.Icon.Should().Be("fa-about");
            article.BackgroundImage.Should().Be("image.jpg");
            article.Profiles.First().Title.Should().Be("A profile");
            article.Profiles.First().Image.Should().Be("image.jpg");
            article.Sections.Should().BeEmpty();
        }

        [Fact]
        public void ConvertsNullBodyToEmptyString()
        {
            dynamic mockContenfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithNullBody.json"));

            var contentfulResponse = new ContentfulResponse(mockContenfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.Body.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertsNullIconToEmptyString()
        {
            dynamic mockContenfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithNullIcon.json"));

            var contentfulResponse = new ContentfulResponse(mockContenfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.Icon.Should().Be(string.Empty);
        }

        [Fact]
        public void NullFieldsThrowsException()
        {
            dynamic mockContenfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithNullEverything.json"));

            var contentfulResponse = new ContentfulResponse(mockContenfulData);

            var entry = contentfulResponse.GetFirstItem();

            Assert.Throws<InvalidDataException>(() => _articleFactory.Build(entry, contentfulResponse));
        }


        [Fact]
        public void BuildsArticleWithSingleSectionFromContentfulResponseData()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithSingleSection.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.Slug.Should().Be("test");

            article.Sections.Should().NotBeNullOrEmpty();
            var section = article.Sections.FirstOrDefault();
            section.Title.Should().Be("Overview ");
            section.Slug.Should().Be("alcohol-overview");
            section.Body.Should().Be("It’s not always easy to recognise when moderate, social drinking or lower risk drinking crosses the line to problem drinking.");
        }


        [Fact]
        public void BuildsArticleWithoutBackgroundImage()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithoutBackgroundImage.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.BackgroundImage.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsNullIfContentfulResponseIsMissingItems()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);
            article.Should().BeNull();
        }

        [Fact]
        public void BuildArticleWithBreadcrumbs()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithBreadcrumbs.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.Title.Should().Be("Title");
            article.Slug.Should().Be("slug");
            article.Teaser.Should().Be("Teaser");
            article.Breadcrumbs.Count().Should().Be(2);
            article.Breadcrumbs.First().Title.Should().Be("title");
            article.Breadcrumbs.First().Slug.Should().Be("slug");
        }

        [Fact]
        public void BuildArticleWithAlerts()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithAlerts.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var alert = new Alert("Alert", "alert", "alert message", "Warning", new DateTime(2016, 08, 01), new DateTime(2216, 08, 01));
            var mockAlertBuilder = new Mock<IFactory<Alert>>();
            mockAlertBuilder.Setup(
                    o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(alert);
            var articleFactory = new ArticleFactory(_mockTopicBuilder.Object, new AlertListFactory(_mockTimeProvider.Object, mockAlertBuilder.Object), _breadcrumbFactory, _sectionListFactory, _profileListFactory, _mockDocumentListFactory.Object, _liveChatListFactory.Object);

            var entry = contentfulResponse.GetFirstItem();
            Article article = articleFactory.Build(entry, contentfulResponse);

            article.Title.Should().Be("Title");
            article.Slug.Should().Be("slug");
            article.Alerts.Count().Should().Be(2);
            article.Alerts.First().Title.Should().Be("Alert");
            article.Alerts.First().SubHeading.Should().Be("alert");
            article.Alerts.First().Body.Should().Be("alert message");
            article.Alerts.First().Severity.Should().Be("Warning");
        }

        [Fact]
        public void BuildArticleParentTopic()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithParentTopic.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.ParentTopic.Name.Should().Be("Main Topic");
            article.ParentTopic.Slug.Should().Be("main-topic");
        }

        [Fact]
        public void ParentTopicsSubItemsShouldIncludeCurrentArticleAsChild()
        {
            var realTopicFactory = new TopicFactory(_mockAlertListFactory.Object, new SubItemListFactory(new SubItemFactory(), _mockTimeProvider.Object),
                _breadcrumbFactory);
            var articleFactory = new ArticleFactory(realTopicFactory, _mockAlertListFactory.Object, _breadcrumbFactory, _sectionListFactory, _profileListFactory, _mockDocumentListFactory.Object, _liveChatListFactory.Object);

            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithParentTopic.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = articleFactory.Build(entry, contentfulResponse);

            var subItems = article.ParentTopic.SubItems;
            var currentArticle = subItems.FirstOrDefault(o => o.Slug == "test-me");
            currentArticle.Should().NotBeNull();
        }

        [Fact]
        public void BuildArticleWithNoSectionsOrParentTopic()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithoutSections.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.Title.Should().Be("About Us");
            article.Slug.Should().Be("about-us");
            article.ParentTopic.Name.Should().Be(string.Empty);
        }

        [Fact]
        public void BuildArticleWithBreadcrumbsButNoParentTopic()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithBreadcrumbsButNoParentTopic.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.Title.Should().Be("Article with no topic in the breadcrumb");
            article.Breadcrumbs.Should().HaveCount(2);
            article.ParentTopic.Name.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertsEmptySunsetAndSunriseDateToMinValue()
        {
            dynamic mockContenfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ArticleWithEmptySunsetAndSunriseDate.json"));

            var contentfulResponse = new ContentfulResponse(mockContenfulData);

            var entry = contentfulResponse.GetFirstItem();
            Article article = _articleFactory.Build(entry, contentfulResponse);

            article.SunriseDate.Should().Be(DateTime.MinValue);
            article.SunsetDate.Should().Be(DateTime.MinValue);
        }
    }
        
}
