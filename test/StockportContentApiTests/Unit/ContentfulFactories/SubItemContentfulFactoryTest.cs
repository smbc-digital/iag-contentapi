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
        [Fact]
        public void ShouldCreateASubItemFromAContentfulSubItem()
        {
            var contentfulSubItem = 
                new Entry<ContentfulSubItem> {
                    Fields = new ContentfulSubItemBuilder().Build(),
                    SystemProperties = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
                };
 
            var subItem = new SubItemContentfulFactory().ToModel(contentfulSubItem);

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
        public void ShouldSetStartPageToADifferentIDThanProvided()
        {
            var contentfulSubItem =
                new Entry<ContentfulSubItem>
                {
                    Fields = new ContentfulSubItemBuilder().Build(),
                    SystemProperties = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
                };

            var subItem = new SubItemContentfulFactory().ToModel(contentfulSubItem);

            subItem.Type.Should().Be("start-page");
        }
    }
}
