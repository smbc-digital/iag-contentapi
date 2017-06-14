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

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ArticleContentfulFactoryTest
    {
        private readonly Entry<ContentfulArticle> _contentfulArticle;
        private readonly Mock<IVideoRepository> _videoRepository;
        private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _sectionFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;       
        private readonly ArticleContentfulFactory _articleFactory;
        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulArticle>, Topic>> _parentTopicFactory;
        private readonly Mock<ITimeProvider> _timeProvider;

        public ArticleContentfulFactoryTest()
        {
            _contentfulArticle = new ContentfulEntryBuilder<ContentfulArticle>().Fields(new ContentfulArticleBuilder().Build()).Build();

            // set to topic for mocking
            // TODO: Refactor into builder
            _contentfulArticle.Fields.Breadcrumbs[0].SystemProperties.ContentType.SystemProperties.Id = "topic";

            _videoRepository = new Mock<IVideoRepository>();
            _sectionFactory = new Mock<IContentfulFactory<ContentfulSection, Section>>();
            _crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            _parentTopicFactory = new Mock<IContentfulFactory<Entry<ContentfulArticle>, Topic>>();

            _timeProvider = new Mock<ITimeProvider>();

            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _articleFactory = new ArticleContentfulFactory(_sectionFactory.Object, _crumbFactory.Object, _profileFactory.Object, 
                _parentTopicFactory.Object, _documentFactory.Object, _videoRepository.Object, _timeProvider.Object);
        }

        [Fact]
        public void ShouldCreateAnArticleFromAContentfulArticle()
        {
            const string processedBody = "this is processed body";
            _videoRepository.Setup(o => o.Process(_contentfulArticle.Fields.Body)).Returns(processedBody);
            var section = new Section("title", "slug", "body", new List<Profile>(), new List<Document>(),
                DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), new List<Alert>());
            _sectionFactory.Setup(o => o.ToModel(_contentfulArticle.Fields.Sections.First().Fields)).Returns(section);
            var crumb = new Crumb("title", "slug", "type");
            _crumbFactory.Setup(o => o.ToModel(_contentfulArticle.Fields.Breadcrumbs.First())).Returns(crumb);
            var profile = new Profile("type", "title", "slug", "subtitle", "body", "icon", "image",
                new List<Crumb> { crumb });
            _profileFactory.Setup(o => o.ToModel(_contentfulArticle.Fields.Profiles.First().Fields)).Returns(profile);
            var subItems = new List<SubItem> {
                new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()) };
            var topic = new Topic("slug", "name", "teaser", "summary", "icon", "image", "image", subItems, subItems, subItems,
                new List<Crumb> { crumb },
                new List<Alert> { new Alert("title", "subHeading", "body", "severity", DateTime.MinValue, DateTime.MaxValue) },
                DateTime.MinValue, DateTime.MaxValue, false, "id", new NullEventBanner(), "expandingLinkTitle", new List<ExpandingLinkBox>());
            _parentTopicFactory.Setup(o => o.ToModel(It.IsAny<Entry<ContentfulArticle>>()))
                .Returns(topic);
            var document = new Document("title", 1000, DateTime.MinValue.ToUniversalTime(), "url", "fileName");
            _documentFactory.Setup(o => o.ToModel(_contentfulArticle.Fields.Documents.First())).Returns(document);

            var article = _articleFactory.ToModel(_contentfulArticle);

            article.ShouldBeEquivalentTo(_contentfulArticle.Fields, o => o.Excluding(e => e.BackgroundImage)
                .Excluding(e => e.Image)
                .Excluding(e => e.Documents)
                .Excluding(e => e.Sections)
                .Excluding(e => e.Profiles)
                .Excluding(e => e.ParentTopic)
                .Excluding(e => e.Breadcrumbs)
                .Excluding(e => e.Body)
                .Excluding(e => e.Alerts)
                .Excluding(e => e.AlertsInline)
                .Excluding(e => e.LiveChat));

            article.Alerts.First().ShouldBeEquivalentTo(_contentfulArticle.Fields.Alerts.First().Fields);
            article.LiveChat.ShouldBeEquivalentTo(_contentfulArticle.Fields.LiveChatText.Fields);

            _videoRepository.Verify(o => o.Process(_contentfulArticle.Fields.Body), Times.Once());
            article.Body.Should().Be(processedBody);
            article.BackgroundImage.Should().Be(_contentfulArticle.Fields.BackgroundImage.File.Url);

            _sectionFactory.Verify(o => o.ToModel(_contentfulArticle.Fields.Sections.First().Fields), Times.Once);
            article.Sections.First().ShouldBeEquivalentTo(section);

            _crumbFactory.Verify(o => o.ToModel(_contentfulArticle.Fields.Breadcrumbs.First()), Times.Once);
            article.Breadcrumbs.First().ShouldBeEquivalentTo(crumb);

            _profileFactory.Verify(o => o.ToModel(_contentfulArticle.Fields.Profiles.First().Fields), Times.Once);
            article.Profiles.First().ShouldBeEquivalentTo(profile);

            _parentTopicFactory.Verify(o => o.ToModel(It.IsAny<Entry<ContentfulArticle>>()), Times.Once);
            article.ParentTopic.ShouldBeEquivalentTo(topic);

            _documentFactory.Verify(o => o.ToModel(_contentfulArticle.Fields.Documents.First()), Times.Once);
            article.Documents.Count.Should().Be(1);
            article.Documents.First().Should().Be(document);
        }

        [Fact]
        public void ShouldNotAddBackgroundImageOrSectionsOrBreadcrumbsOrAlertsOrProfilesOrParentTopicOrDocumentsOrLiveChatIfTheyAreLinks()
        {
            _contentfulArticle.Fields.BackgroundImage.SystemProperties.Type = "Link";
            _contentfulArticle.Fields.Sections.First().SystemProperties.Type = "Link";
            _contentfulArticle.Fields.Breadcrumbs.First().SystemProperties.Type = "Link";
            _contentfulArticle.Fields.Alerts.First().SystemProperties.Type = "Link";
            _contentfulArticle.Fields.Profiles.First().SystemProperties.Type = "Link";
            _contentfulArticle.Fields.ParentTopic.SystemProperties.Type = "Link";
            _contentfulArticle.Fields.Documents.First().SystemProperties.Type = "Link";
            _contentfulArticle.Fields.LiveChatText.SystemProperties.Type = "Link";

            var article = _articleFactory.ToModel(_contentfulArticle);

            article.BackgroundImage.Should().BeEmpty();

            article.Sections.Count.Should().Be(0);
            _sectionFactory.Verify(o => o.ToModel(It.IsAny<ContentfulSection>()), Times.Never);

            article.Breadcrumbs.Count().Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(It.IsAny<Entry<ContentfulCrumb>>()), Times.Never);

            article.Alerts.Count().Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(It.IsAny<Entry<ContentfulCrumb>>()), Times.Never);

            article.Profiles.Count().Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(It.IsAny<Entry<ContentfulCrumb>>()), Times.Never);
            article.LiveChat.ShouldBeEquivalentTo(new NullLiveChat());

            article.ParentTopic.ShouldBeEquivalentTo(new NullTopic());

            article.Documents.Count.Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(It.IsAny<Entry<ContentfulCrumb>>()), Times.Never);
        }

        [Fact]
        public void ItShouldRemoveAlertsInlineThatArePastSunsetDateOrBeforeSunriseDate()
        {

            Entry<Alert> _visibleAlert = new Entry<Alert> { Fields = new Alert("title", "SubHeading", "Body", "severity", new DateTime(2016, 12, 01), new DateTime(2017, 02, 01)), SystemProperties = new SystemProperties() };
            Entry<Alert> _invisibleAlert = new Entry<Alert> { Fields = new Alert("title", "SubHeading", "Body", "severity", new DateTime(2017, 05, 01), new DateTime(2017, 07, 01)), SystemProperties = new SystemProperties() };

            var contentfulArticle = new ContentfulEntryBuilder<ContentfulArticle>().Fields(new ContentfulArticleBuilder().AlertsInline(new List<Entry<Alert>> { _visibleAlert, _invisibleAlert }).Build()).Build();

            var article = _articleFactory.ToModel(contentfulArticle);

            article.AlertsInline.Count().Should().Be(1);
        }
        [Fact]
        public void ItShouldRemoveAlertsThatArePastSunsetDateOrBeforeSunriseDate()
        {
            Entry<Alert> _visibleAlert = new Entry<Alert> { Fields = new Alert("title", "SubHeading", "Body", "severity", new DateTime(2016, 12, 01), new DateTime(2017, 02, 01)), SystemProperties = new SystemProperties() };
            Entry<Alert> _invisibleAlert = new Entry<Alert> { Fields = new Alert("title", "SubHeading", "Body", "severity", new DateTime(2017, 05, 01), new DateTime(2017, 07, 01)), SystemProperties = new SystemProperties() };

            var contentfulArticle = new ContentfulEntryBuilder<ContentfulArticle>().Fields(new ContentfulArticleBuilder().Alerts(new List<Entry<Alert>> { _visibleAlert, _invisibleAlert }).Build()).Build();

            var article = _articleFactory.ToModel(contentfulArticle);

            article.Alerts.Count().Should().Be(1);
        }

        [Fact]
        public void ShouldParseArticleIfBodyIsNull()
        {
            var contentfulArticle = new ContentfulEntryBuilder<ContentfulArticle>()
                .Fields(new ContentfulArticleBuilder().Title("title").Body(null).Build())
                .Build();

            var article = _articleFactory.ToModel(contentfulArticle);

            article.Should().BeOfType<Article>();
            article.Body.Should().Be(string.Empty);
            article.Title.Should().Be("title");
        }

    }
}
