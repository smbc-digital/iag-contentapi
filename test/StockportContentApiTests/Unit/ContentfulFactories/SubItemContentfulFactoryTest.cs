using System;
using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class SubItemContentfulFactoryTest
    {
        private readonly SubItemContentfulFactory _subItemContentfulFactory;

        public SubItemContentfulFactoryTest()
        {
            _subItemContentfulFactory = new SubItemContentfulFactory();
        }

        [Fact]
        public void ShouldCreateASubItemFromAContentfulSubItem()
        {
            var contentfulSubItem = 
                new Entry<ContentfulSubItem> {
                    Fields = new ContentfulSubItemBuilder().Build(),
                    SystemProperties = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
                };
 
            var subItem = _subItemContentfulFactory.ToModel(contentfulSubItem);

            subItem.Slug.Should().Be(contentfulSubItem.Fields.Slug);
            subItem.Title.Should().Be(contentfulSubItem.Fields.Title);
            subItem.Icon.Should().Be(contentfulSubItem.Fields.Icon);
            subItem.Teaser.Should().Be(contentfulSubItem.Fields.Teaser);
            subItem.SunriseDate.Should().Be(contentfulSubItem.Fields.SunriseDate);
            subItem.SunsetDate.Should().Be(contentfulSubItem.Fields.SunsetDate);
            subItem.Type.Should().Be(contentfulSubItem.SystemProperties.ContentType.SystemProperties.Id);           
        }

        // TODO: remove start page inconsistency
        [Fact]
        public void ShouldSetStartPageToADifferentIdThanProvided()
        {
            var contentfulSubItem =
                new Entry<ContentfulSubItem>
                {
                    Fields = new ContentfulSubItemBuilder().Build(),
                    SystemProperties = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
                };

            var subItem = _subItemContentfulFactory.ToModel(contentfulSubItem);

            subItem.Type.Should().Be("start-page");
        }

        [Fact]
        public void ShouldCreateSubItemWithNameForTitleWhenNoTitleProvided()
        {
            var contentfulSubItem =
                new Entry<ContentfulSubItem>
                {
                    Fields = new ContentfulSubItemBuilder().Title(string.Empty).Name("name").Build(),
                    SystemProperties = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
                };

            var subItem = _subItemContentfulFactory.ToModel(contentfulSubItem);

            subItem.Title.Should().Be(contentfulSubItem.Fields.Name);

        }
    }
}
