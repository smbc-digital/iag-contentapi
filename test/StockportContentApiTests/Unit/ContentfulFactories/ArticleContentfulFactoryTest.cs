using System;
using System.Collections.Generic;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using Xunit;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.Repositories;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulFactories.ArticleFactories;
using StockportContentApi.Fakes;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ArticleContentfulFactoryTest
    {
        private readonly ContentfulArticle _contentfulArticle;
        private readonly Mock<IVideoRepository> _videoRepository;
        private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _sectionFactory;
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;
        private readonly ArticleContentfulFactory _articleFactory;
        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
        private readonly Mock<IContentfulFactory<ContentfulArticle, Topic>> _parentTopicFactory;
        private readonly Mock<ITimeProvider> _timeProvider;
        private Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
        private Mock<IContentfulFactory<ContentfulLiveChat, LiveChat>> _LiveChatFactory;
        private Mock<IContentfulFactory<ContentfulAdvertisement, Advertisement>> _advertisementFactory;
        
        public ArticleContentfulFactoryTest()
        {
            _contentfulArticle = new ContentfulArticleBuilder().Build();

            // set to topic for mocking
            // TODO: Refactor into builder
            _contentfulArticle.Breadcrumbs[0].Sys.ContentType.SystemProperties.Id = "topic";

            _videoRepository = new Mock<IVideoRepository>();
            _sectionFactory = new Mock<IContentfulFactory<ContentfulSection, Section>>();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            _LiveChatFactory = new Mock<IContentfulFactory<ContentfulLiveChat, LiveChat>>();
            _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            _parentTopicFactory = new Mock<IContentfulFactory<ContentfulArticle, Topic>>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _advertisementFactory = new Mock<IContentfulFactory<ContentfulAdvertisement, Advertisement>>();

            _timeProvider = new Mock<ITimeProvider>();

            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _articleFactory = new ArticleContentfulFactory(_sectionFactory.Object, _crumbFactory.Object, _profileFactory.Object,
                _parentTopicFactory.Object, _LiveChatFactory.Object, _documentFactory.Object, _videoRepository.Object, _timeProvider.Object, _advertisementFactory.Object, _alertFactory.Object, HttpContextFake.GetHttpContextFake());
        }

        [Fact(Skip = "Fluent Assertions update")]
        public void ShouldCreateAnArticleFromAContentfulArticle()
        {
            const string processedBody = "this is processed body";
            _videoRepository.Setup(o => o.Process(_contentfulArticle.Body)).Returns(processedBody);
            var section = new Section("title", "slug", "body", new List<Profile>(), new List<Document>(),
                DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), new List<Alert>());
            _sectionFactory.Setup(o => o.ToModel(_contentfulArticle.Sections.First())).Returns(section);
            _LiveChatFactory.Setup(o => o.ToModel(It.IsAny<ContentfulLiveChat>())).Returns(new LiveChat("title", "text"));
            var crumb = new Crumb("title", "slug", "type");
            _crumbFactory.Setup(o => o.ToModel(_contentfulArticle.Breadcrumbs.First())).Returns(crumb);
            var profile = new Profile("type", "title", "slug", "subtitle", "body", "icon", "image",
                new List<Crumb> { crumb });
            _profileFactory.Setup(o => o.ToModel(_contentfulArticle.Profiles.First())).Returns(profile);
            var subItems = new List<SubItem> {
                new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()) };
            var topic = new Topic("slug", "name", "teaser", "summary", "icon", "image", "image", subItems, subItems, subItems,
                new List<Crumb> { crumb },
                new List<Alert> { new Alert("title", "subHeading", "body", "severity", DateTime.MinValue, DateTime.MaxValue, string.Empty) },
                DateTime.MinValue, DateTime.MaxValue, false, "id", new NullEventBanner(), "expandingLinkTitle", new NullAdvertisement(), new List<ExpandingLinkBox>());
            _parentTopicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulArticle>()))

                .Returns(topic);
            var document = new DocumentBuilder().Build();
            _documentFactory.Setup(o => o.ToModel(_contentfulArticle.Documents.First())).Returns(document);
            var alert = new Alert("title", "subHeading", "body", "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc), "slug");
            _alertFactory.Setup(o => o.ToModel(_contentfulArticle.Alerts.First())).Returns(alert);
            
            var advertisment = new Advertisement("Advert Title","advert slug","advert teaser",DateTime.MaxValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), true,"url","image.jpg");
            _advertisementFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulAdvertisement>())).Returns(advertisment);    
            
            var article = _articleFactory.ToModel(_contentfulArticle);

            article.Should().BeEquivalentTo(_contentfulArticle, o => o.ExcludingNestedObjects());

            article.Alerts.First().Should().BeEquivalentTo(_contentfulArticle.Alerts.First());
            article.LiveChat.Title.Should().BeEquivalentTo(_contentfulArticle.LiveChatText.Title);
            article.LiveChat.Text.Should().BeEquivalentTo(_contentfulArticle.LiveChatText.Text);

            _videoRepository.Verify(o => o.Process(_contentfulArticle.Body), Times.Once());
            article.Body.Should().Be(processedBody);
            article.BackgroundImage.Should().Be(_contentfulArticle.BackgroundImage.File.Url);

            _sectionFactory.Verify(o => o.ToModel(_contentfulArticle.Sections.First()), Times.Once);
            article.Sections.First().Should().BeEquivalentTo(section);

            _crumbFactory.Verify(o => o.ToModel(_contentfulArticle.Breadcrumbs.First()), Times.Once);
            article.Breadcrumbs.First().Should().BeEquivalentTo(crumb);

            _profileFactory.Verify(o => o.ToModel(_contentfulArticle.Profiles.First()), Times.Once);
            article.Profiles.First().Should().BeEquivalentTo(profile);

            _parentTopicFactory.Verify(o => o.ToModel(It.IsAny<ContentfulArticle>()), Times.Once);
            article.ParentTopic.Should().BeEquivalentTo(topic);

            _documentFactory.Verify(o => o.ToModel(_contentfulArticle.Documents.First()), Times.Once);
            article.Documents.Count.Should().Be(1);
            article.Documents.First().Should().Be(document);
            
            article.Advertisement.Image.Should().Be(advertisment.Image);
            article.Advertisement.Title.Should().Be(advertisment.Title);
            article.Advertisement.Teaser.Should().Be(advertisment.Teaser);
        }

        [Fact]
        public void ShouldNotAddBackgroundImageOrSectionsOrBreadcrumbsOrAlertsOrProfilesOrParentTopicOrDocumentsOrLiveChatIfTheyAreLinks()
        {
            _contentfulArticle.BackgroundImage.SystemProperties.Type = "Link";
            _contentfulArticle.Sections.First().Sys.Type = "Link";
            _contentfulArticle.Breadcrumbs.First().Sys.Type = "Link";
            _contentfulArticle.Alerts.First().Sys.Type = "Link";
            _contentfulArticle.Profiles.First().Sys.Type = "Link";
            _contentfulArticle.Documents.First().SystemProperties.Type = "Link";
            _contentfulArticle.LiveChatText.Sys.Type = "Link";

            var article = _articleFactory.ToModel(_contentfulArticle);

            article.BackgroundImage.Should().BeEmpty();

            article.Sections.Count.Should().Be(0);
            _sectionFactory.Verify(o => o.ToModel(It.IsAny<ContentfulSection>()), Times.Never);

            article.Breadcrumbs.Count().Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);

            article.Alerts.Count().Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);

            article.Profiles.Count().Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
            article.LiveChat.Should().BeEquivalentTo(new NullLiveChat());

            article.ParentTopic.Should().BeEquivalentTo(new NullTopic());

            article.Documents.Count.Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
        }

        [Fact]
        public void ItShouldRemoveAlertsInlineThatArePastSunsetDateOrBeforeSunriseDate()
        {
            ContentfulAlert _visibleAlert = new ContentfulAlert()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(2016, 12, 01),
                SunsetDate = new DateTime(2017, 02, 01),
                Sys = new SystemProperties() {Type = "Entry"}
            };

            ContentfulAlert _invisibleAlert =
                new ContentfulAlert()
                {
                    Title = "title",
                    SubHeading = "subHeading",
                    Body = "body",
                    Severity = "severity",
                    SunriseDate = new DateTime(2017, 05, 01),
                    SunsetDate = new DateTime(2017, 07, 01),
                    Sys = new SystemProperties() { Type = "Entry" }
                };

            var contentfulArticle =
                new ContentfulArticleBuilder().AlertsInline(new List<ContentfulAlert> { _visibleAlert, _invisibleAlert })
                    .Build();

            var article = _articleFactory.ToModel(contentfulArticle);

            article.AlertsInline.Count().Should().Be(1);
        }




        [Fact]
        public void ItShouldRemoveAlertsThatArePastSunsetDateOrBeforeSunriseDate()
        {
            

            ContentfulAlert _visibleAlert = new ContentfulAlert()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(2016, 12, 01),
                SunsetDate = new DateTime(2017, 02, 01),
                Sys = new SystemProperties() { Type = "Entry" }
            };
            ContentfulAlert _invisibleAlert = new ContentfulAlert()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(2017, 05, 01),
                SunsetDate = new DateTime(2017, 07, 01),
                Sys = new SystemProperties() { Type = "Entry" }
            };

            var contentfulArticle =
                new ContentfulArticleBuilder().Alerts(new List<ContentfulAlert> { _visibleAlert, _invisibleAlert }).Build();

            var article = _articleFactory.ToModel(contentfulArticle);

            article.Alerts.Count().Should().Be(1);
        }

        [Fact]
        public void ShouldParseArticleIfBodyIsNull()
        {
            var contentfulArticle = new ContentfulArticleBuilder().Title("title").Body(null).Build();

            var article = _articleFactory.ToModel(contentfulArticle);

            article.Should().BeOfType<Article>();
            article.Body.Should().Be(string.Empty);
            article.Title.Should().Be("title");
        }
    }
}

