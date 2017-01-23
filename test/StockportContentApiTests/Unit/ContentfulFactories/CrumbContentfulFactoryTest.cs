using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class CrumbContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateACrumbFromAContentfulCrumb()
        {
            var contentfulCrumb = 
                new Entry<ContentfulCrumb>() {
                    Fields = new ContentfulCrumbBuilder().Build(),
                    SystemProperties = new SystemProperties() { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
                };
 
            var crumb = new CrumbContentfulFactory().ToModel(contentfulCrumb);

            crumb.Slug.Should().Be(contentfulCrumb.Fields.Slug);
            crumb.Title.Should().Be(contentfulCrumb.Fields.Title);
            crumb.Type.Should().Be(contentfulCrumb.SystemProperties.Id);
        }
    }
}
