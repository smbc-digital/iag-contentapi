using System;
using System.Collections.Generic;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using Xunit;
using System.Linq;
using FluentAssertions;
using Moq;
using StockportContentApi.Utils;
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

            var articleFactory = new ArticleContentfulFactory(sectionFactory.Object);
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

        }
    }

    public class ArticleContentfulFactory : IContentfulFactory<ContentfulArticle, Article>
    {
        private readonly IContentfulFactory<ContentfulSection, Section> _sectionFactory;

        public ArticleContentfulFactory(IContentfulFactory<ContentfulSection, Section> sectionFactory)
        {
            _sectionFactory = sectionFactory;
        }

        public Article ToModel(ContentfulArticle entry)
        {
            var sections = entry.Sections.Select(section => _sectionFactory.ToModel(section)).ToList();
            var breadcrumbs = new List<Crumb>();
            var profiles = new List<Profile>();
            var topic = new NullTopic();
            var documents = entry.Documents.Select(
                document =>
                    new Document(document.Description,
                        (int)document.File.Details.Size,
                        DateComparer.DateFieldToDate(document.SystemProperties.UpdatedAt),
                        document.File.Url, document.File.FileName)).ToList();

            return new Article(entry.Body, entry.Slug, entry.Title, entry.Teaser, entry.Icon, entry.BackgroundImage.File.Url, 
                               sections, breadcrumbs, entry.Alerts, profiles, topic, documents, entry.SunriseDate, entry.SunsetDate, 
                               entry.LiveChatVisible, entry.LiveChat);
        }
    }
}
