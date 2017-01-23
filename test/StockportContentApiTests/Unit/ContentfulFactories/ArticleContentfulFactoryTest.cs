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
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ArticleContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAnArticleFromAContentfulArticle()
        {
            var contentfulArticle = new ContentfulArticleBuilder().Build();
            var section = new Section("title", "slug", "body", new List<Profile>(), new List<Document>(), DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime());
            var sectionFactory = new Mock<IContentfulFactory<ContentfulSection, Section>>();
            sectionFactory.Setup(o => o.ToModel(contentfulArticle.Sections.First())).Returns(section);
            var crumb = new Crumb("title", "slug", "type");
            var crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            crumbFactory.Setup(o => o.ToModel(contentfulArticle.Breadcrumbs.First())).Returns(crumb);
            var profile = new Profile("type", "title", "slug", "subtitle", "body", "icon", "image", new List<Crumb> { crumb });
            var profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            profileFactory.Setup(o => o.ToModel(contentfulArticle.Profiles.First())).Returns(profile);
            var subItems = new List<SubItem> { new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue) };
            var topic = new Topic("slug", "name", "teaser", "summary", "icon", "image", subItems, subItems, subItems, 
                new List<Crumb> {crumb}, 
                new List<Alert> { new Alert("title", "subHeading", "body", "severity", DateTime.MinValue, DateTime.MaxValue) }, 
                DateTime.MinValue, DateTime.MaxValue, false, "id");
            var topicFactory = new Mock<IContentfulFactory<ContentfulTopic, Topic>>();
            topicFactory.Setup(o => o.ToModel(contentfulArticle.ParentTopic)).Returns(topic);

            var articleFactory = new ArticleContentfulFactory(sectionFactory.Object, crumbFactory.Object, profileFactory.Object, 
                topicFactory.Object);
            var article = articleFactory.ToModel(contentfulArticle);

            article.ShouldBeEquivalentTo(contentfulArticle, o => o.Excluding(e => e.BackgroundImage)
                                                                  .Excluding(e => e.Documents)
                                                                  .Excluding(e => e.Sections)
                                                                  .Excluding(e => e.Profiles)
                                                                  .Excluding(e => e.ParentTopic)
                                                                  .Excluding(e => e.Breadcrumbs));

            article.BackgroundImage.Should().Be(contentfulArticle.BackgroundImage.File.Url);
            article.Documents.Count.Should().Be(contentfulArticle.Documents.Count);
            article.Documents.First().Url.Should().Be(contentfulArticle.Documents.First().File.Url);
            article.Documents.First().Title.Should().Be(contentfulArticle.Documents.First().Description);
            article.Documents.First().FileName.Should().Be(contentfulArticle.Documents.First().File.FileName);
            article.Documents.First().Size.Should().Be((int)contentfulArticle.Documents.First().File.Details.Size);
            article.Documents.First().LastUpdated.Should().Be(contentfulArticle.Documents.First().SystemProperties.UpdatedAt.Value);

            sectionFactory.Verify(o => o.ToModel(contentfulArticle.Sections.First()), Times.Once);
            article.Sections.First().ShouldBeEquivalentTo(section);

            crumbFactory.Verify(o => o.ToModel(contentfulArticle.Breadcrumbs.First()), Times.Once);
            article.Breadcrumbs.First().ShouldBeEquivalentTo(crumb);

            profileFactory.Verify(o => o.ToModel(contentfulArticle.Profiles.First()), Times.Once);
            article.Profiles.First().ShouldBeEquivalentTo(profile);

            topicFactory.Verify(o => o.ToModel(contentfulArticle.ParentTopic), Times.Once);
            article.ParentTopic.ShouldBeEquivalentTo(topic);

        }
    }
}
