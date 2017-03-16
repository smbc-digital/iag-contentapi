using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ParentTopicContentfulFactoryTests
    {
        private readonly Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>> _subitemContentfulFactory;
        private readonly ParentTopicContentfulFactory _parentTopicContentfulFactory;
        private readonly Mock<ITimeProvider> _timeProvider;

        public ParentTopicContentfulFactoryTests()
        {
            // create mocks
            _subitemContentfulFactory = new Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>();
            _timeProvider = new Mock<ITimeProvider>();

            // setup mocks
            _subitemContentfulFactory.Setup(o => o.ToModel(It.IsAny<Entry<ContentfulSubItem>>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue,
                    "image"));
            _timeProvider.Setup(o => o.Now())
                .Returns(new DateTime(2017, 01, 02));

            // call constructor
            _parentTopicContentfulFactory = new ParentTopicContentfulFactory(_subitemContentfulFactory.Object, _timeProvider.Object);
        }

        [Fact]
        public void ShouldReturnATopicFromAContentfulArticleBasedOnTheBreadcrumbs()
        {
            var subItemEntry = new List<Entry<ContentfulSubItem>>
            {
                new ContentfulEntryBuilder<ContentfulSubItem>()
                .Fields(new ContentfulSubItemBuilder().Slug("sub-slug").Build()).Build()
            };

            var contentfulCrumbs = new ContentfulCrumbBuilder()
                .Name("test topic")
                .Slug("test-topic")
                .SubItems(subItemEntry)
                .Build();

            var contentfulCrumbsEntry = new ContentfulEntryBuilder<ContentfulCrumb>()
                .Fields(contentfulCrumbs)
                .ContentTypeSystemId("topic")
                .Build();

            var contentfulArticle = new ContentfulArticleBuilder()
                .Breadcrumbs(new List<Entry<ContentfulCrumb>>()
                {
                    contentfulCrumbsEntry
                })
                .Build();

            var contentfulArticleEntry = new ContentfulEntryBuilder<ContentfulArticle>()
                .Fields(contentfulArticle).Build();

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

            result.Name.Should().Be("test topic");
            result.Slug.Should().Be("test-topic");
            result.SubItems.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldReturnNullTopicIfBreadcrumbDoesNotHaveTypeOfTopic()
        {
            var subItemEntry = new List<Entry<ContentfulSubItem>>
            {
                new ContentfulEntryBuilder<ContentfulSubItem>()
                .Fields(new ContentfulSubItemBuilder().Slug("sub-slug").Build()).Build()
            };

            var contentfulCrumbs = new ContentfulCrumbBuilder()
                .Name("test topic")
                .Slug("test-topic")
                .SubItems(subItemEntry)
                .Build();

            var contentfulCrumbsEntry = new ContentfulEntryBuilder<ContentfulCrumb>()
                .Fields(contentfulCrumbs)
                .ContentTypeSystemId("article")
                .Build();

            var contentfulArticle = new ContentfulArticleBuilder()
                .Breadcrumbs(new List<Entry<ContentfulCrumb>>()
                {
                    contentfulCrumbsEntry
                })
                .Build();

            var contentfulArticleEntry = new ContentfulEntryBuilder<ContentfulArticle>()
                .Fields(contentfulArticle).Build();

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

            result.Should().BeOfType<NullTopic>();
        }

        [Fact]
        public void ShouldReturnNullTopicIfNoBreadcrumbs()
        {
            var contentfulArticle = new ContentfulArticleBuilder()
                .Breadcrumbs(new List<Entry<ContentfulCrumb>>())
                .Build();

            var contentfulArticleEntry = new ContentfulEntryBuilder<ContentfulArticle>()
                .Fields(contentfulArticle).Build();

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

            result.Should().BeOfType<NullTopic>();
        }

        [Fact]
        public void ShouldReturnTopicWithFirstSubItemOfTheArticle()
        {
            var subItemEntry = new ContentfulEntryBuilder<ContentfulSubItem>()
                .SystemId("same-id-as-article")
                .Fields(new ContentfulSubItemBuilder()
                    .Slug("sub-slug")
                    .Build())
                .Build();

            var subItemEntryOther = new ContentfulEntryBuilder<ContentfulSubItem>()
                .SystemId("not-same-id-as-article")
                .Fields(new ContentfulSubItemBuilder()
                    .Slug("sub-slug")
                    .Build())
                .Build();

            var subItemEntryList = new List<Entry<ContentfulSubItem>>
            {
                subItemEntry,
                subItemEntryOther
            };

            var contentfulCrumbs = new ContentfulCrumbBuilder()
                .Name("test topic")
                .Slug("test-topic")
                .SubItems(subItemEntryList)
                .Build();

            var contentfulCrumbsEntry = new ContentfulEntryBuilder<ContentfulCrumb>()
                .Fields(contentfulCrumbs)
                .ContentTypeSystemId("topic")
                .Build();

            var contentfulArticle = new ContentfulArticleBuilder()
                .Breadcrumbs(new List<Entry<ContentfulCrumb>>()
                {
                    contentfulCrumbsEntry
                })
                .Title("article-title")
                .Slug("article-slug")
                .Build();

            var contentfulArticleEntry = new ContentfulEntryBuilder<ContentfulArticle>()
                .SystemId("same-id-as-article")
                .Fields(contentfulArticle)
                .Build();

            _subitemContentfulFactory.Setup(o => o.ToModel(subItemEntry))
                .Returns(
                new SubItem("article-slug", "article-title", string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue, string.Empty));

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

            result.Should().BeOfType<Topic>();
            result.SubItems.Should().HaveCount(2);
            result.SubItems.First().Title.Should().Be("article-title");
            result.SubItems.First().Slug.Should().Be("article-slug");
            result.SubItems.ToList()[1].Title.Should().Be("title");
            result.SubItems.ToList()[1].Slug.Should().Be("slug");
        }
    }
}
