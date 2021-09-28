using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulFactories.TopicFactories;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ParentTopicContentfulFactoryTests
    {
        private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemContentfulFactory;
        private readonly ParentTopicContentfulFactory _parentTopicContentfulFactory;
        private readonly Mock<ITimeProvider> _timeProvider;

        public ParentTopicContentfulFactoryTests()
        {
            // create mocks
            _subitemContentfulFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
            _timeProvider = new Mock<ITimeProvider>();

            // setup mocks
            _subitemContentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
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
            var subItemEntry = new List<ContentfulReference>
            {
                new ContentfulReferenceBuilder().Slug("sub-slug").Build()
            };

            var ContentfulReferences = new ContentfulReferenceBuilder()
                .Name("test topic")
                .Slug("test-topic")
                .SubItems(subItemEntry)
                .SystemContentTypeId("topic")
                .Build();

            var contentfulArticleEntry = new ContentfulArticleBuilder().Breadcrumbs(new List<ContentfulReference>() { ContentfulReferences }).Build();
           

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

            result.Name.Should().Be("test topic");
            result.Slug.Should().Be("test-topic");
            result.SubItems.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldReturnNullTopicIfBreadcrumbDoesNotHaveTypeOfTopic()
        {
            var subItemEntry = new List<ContentfulReference>
            {
                new ContentfulReferenceBuilder().Slug("sub-slug").Build()
            };

            var ContentfulReferences = new ContentfulReferenceBuilder()
                .Name("test topic")
                .Slug("test-topic")
                .SubItems(subItemEntry)
                .SystemContentTypeId("id")
                .Build();

            var contentfulArticleEntry = new ContentfulArticleBuilder().Breadcrumbs(new List<ContentfulReference>() { ContentfulReferences }).Build();

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

            result.Should().BeOfType<NullTopic>();
        }

        [Fact]
        public void ShouldReturnNullTopicIfNoBreadcrumbs()
        {
            var contentfulArticle = new ContentfulArticleBuilder()
                .Breadcrumbs(new List<ContentfulReference>())
                .Build();

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticle);

            result.Should().BeOfType<NullTopic>();
        }

        [Fact]
        public void ShouldReturnTopicWithFirstSubItemOfTheArticle()
        {
            var subItemEntry = new ContentfulReferenceBuilder()
                .Slug("sub-slug")
                .SystemId("same-id-as-article")
                .Build();

            var subItemEntryOther = new ContentfulReferenceBuilder()
                .Slug("sub-slug")
                .SystemId("not-same-id-as-article")
                .Build();

            var subItemEntryList = new List<ContentfulReference>
            {
                subItemEntry,
                subItemEntryOther
            };

            var ContentfulReferences = new ContentfulReferenceBuilder()
                .Name("test topic")
                .Slug("test-topic")
                .SubItems(subItemEntryList)
                .SystemContentTypeId("topic")
                .Build();

            var contentfulArticle = new ContentfulArticleBuilder()
                .Breadcrumbs(new List<ContentfulReference>()
                {
                    ContentfulReferences
                })
                .Title("title")
                .Slug("slug")
                .SystemId("same-id-as-article")
                .Build();

            _subitemContentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulReference>(x => x.Slug == "article-slug")))
                .Returns(new SubItem("slug", "title", string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue, string.Empty, new List<SubItem>()));

            var result = _parentTopicContentfulFactory.ToModel(contentfulArticle);

            result.Should().BeOfType<Topic>();
            result.SubItems.Should().HaveCount(2);
            result.SubItems.First().Title.Should().Be("title");
            result.SubItems.First().Slug.Should().Be("slug");
            result.SubItems.ToList()[1].Title.Should().Be("title");
            result.SubItems.ToList()[1].Slug.Should().Be("slug");
        }
    }
}
