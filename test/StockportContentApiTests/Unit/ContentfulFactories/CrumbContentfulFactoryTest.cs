using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;
using Moq;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class CrumbContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateACrumbFromAContentfulCrumb()
        {
            var contentfulCrumb =
                new ContentfulCrumbBuilder().Build();                    
 
            var crumb = new CrumbContentfulFactory().ToModel(contentfulCrumb);

            crumb.Slug.Should().Be(contentfulCrumb.Slug);
            crumb.Title.Should().Be(contentfulCrumb.Title);
            crumb.Type.Should().Be(contentfulCrumb.Sys.ContentType.SystemProperties.Id);
        }

        [Fact]
        public void ShouldCreateACrumbWithNameIfSet()
        {
            var contentfulCrumb =
                new ContentfulCrumbBuilder().Name("name").Title(string.Empty).Build();

            var crumb = new CrumbContentfulFactory().ToModel(contentfulCrumb);

            crumb.Title.Should().Be(contentfulCrumb.Name);
        }
    }
}
