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
        private readonly Mock<IContentfulFactory<ContentfulSubItem, SubItem>> _subitemContentfulFactory;
        private readonly ParentTopicContentfulFactory _parentTopicContentfulFactory;
        private readonly Mock<ITimeProvider> _timeProvider;

        public ParentTopicContentfulFactoryTests()
        {
            // create mocks
            _subitemContentfulFactory = new Mock<IContentfulFactory<ContentfulSubItem, SubItem>>();
            _timeProvider = new Mock<ITimeProvider>();

            // setup mocks
            _subitemContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSubItem>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue,
                    "image", new List<SubItem>()));
            _timeProvider.Setup(o => o.Now())
                .Returns(new DateTime(2017, 01, 02));

            // call constructor
            _parentTopicContentfulFactory = new ParentTopicContentfulFactory(_subitemContentfulFactory.Object, _timeProvider.Object);
        }

        [Fact]
        public void ShouldReturnATopicFromAContentfulArticleBasedOnTheBreadcrumbs()
        {
            var subItemEntry = new List<ContentfulSubItem>
            {
                new ContentfulSubItemBuilder().Slug("sub-slug").Build()
            };

            var contentfulCrumbs = new ContentfulCrumbBuilder()
                .Name("test topic")
                .Slug("test-topic")
                .SubItems(subItemEntry)
                .ContentTypeSystemId("topic")
                .Build();

            var contentfulArticleEntry = new ContentfulArticleBuilder().Breadcrumbs(new List<ContentfulCrumb>() { contentfulCrumbs }).Build();
           

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

            result.Name.Should().Be("test topic");
            result.Slug.Should().Be("test-topic");
            result.SubItems.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldReturnNullTopicIfBreadcrumbDoesNotHaveTypeOfTopic()
        {
            var subItemEntry = new List<ContentfulSubItem>
            {
                new ContentfulSubItemBuilder().Slug("sub-slug").Build()
            };

            var contentfulCrumbs = new ContentfulCrumbBuilder()
                .Name("test topic")
                .Slug("test-topic")
                .SubItems(subItemEntry)
                .ContentTypeSystemId("id")
                .Build();

            var contentfulArticleEntry = new ContentfulArticleBuilder().Breadcrumbs(new List<ContentfulCrumb>() { contentfulCrumbs }).Build();

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

            result.Should().BeOfType<NullTopic>();
        }

        [Fact]
        public void ShouldReturnNullTopicIfNoBreadcrumbs()
        {
            var contentfulArticle = new ContentfulArticleBuilder()
                .Breadcrumbs(new List<ContentfulCrumb>())
                .Build();

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticle);

            result.Should().BeOfType<NullTopic>();
        }

        [Fact]
        public void ShouldReturnTopicWithFirstSubItemOfTheArticle()
        {
            var subItemEntry = new ContentfulSubItemBuilder()
                .Slug("sub-slug")
                .SystemId("same-id-as-article")
                .Build();

            var subItemEntryOther = new ContentfulSubItemBuilder()
                .Slug("sub-slug")
                .SystemId("not-same-id-as-article")
                .Build();

            var subItemEntryList = new List<ContentfulSubItem>
            {
                subItemEntry,
                subItemEntryOther
            };

            var contentfulCrumbs = new ContentfulCrumbBuilder()
                .Name("test topic")
                .Slug("test-topic")
                .SubItems(subItemEntryList)
                .ContentTypeSystemId("topic")
                .Build();

            var contentfulArticle = new ContentfulArticleBuilder()
                .Breadcrumbs(new List<ContentfulCrumb>()
                {
                    contentfulCrumbs
                })
                .Title("article-title")
                .Slug("article-slug")
                .SystemId("same-id-as-article")
                .Build();

            _subitemContentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulSubItem>(x => x.Slug == "article-slug")))
                .Returns(new SubItem("article-slug", "article-title", string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue, string.Empty, new List<SubItem>()));

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticle);

            result.Should().BeOfType<Topic>();
            result.SubItems.Should().HaveCount(2);
            result.SubItems.First().Title.Should().Be("article-title");
            result.SubItems.First().Slug.Should().Be("article-slug");
            result.SubItems.ToList()[1].Title.Should().Be("title");
            result.SubItems.ToList()[1].Slug.Should().Be("slug");
        }
    }
}
