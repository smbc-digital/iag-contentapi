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
        private readonly Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>> _subItemFactory;

        public CrumbContentfulFactoryTest()
        {
            _subItemFactory = new Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>();
        }

        [Fact]
        public void ShouldCreateACrumbFromAContentfulCrumb()
        {
            var contentfulCrumb = 
                new Entry<ContentfulCrumb> {
                    Fields = new ContentfulCrumbBuilder().Build(),
                    SystemProperties = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
                };
 
            var crumb = new CrumbContentfulFactory(_subItemFactory.Object).ToModel(contentfulCrumb);

            crumb.Slug.Should().Be(contentfulCrumb.Fields.Slug);
            crumb.Title.Should().Be(contentfulCrumb.Fields.Title);
            crumb.Type.Should().Be(contentfulCrumb.SystemProperties.ContentType.SystemProperties.Id);
        }

        [Fact]
        public void ShouldCreateACrumbWithNameIfSet()
        {
            var contentfulCrumb =
               new Entry<ContentfulCrumb>
               {
                   Fields = new ContentfulCrumbBuilder().Name("name").Title(string.Empty).Build(),
                   SystemProperties = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
               };

            var crumb = new CrumbContentfulFactory(_subItemFactory.Object).ToModel(contentfulCrumb);

            crumb.Title.Should().Be(contentfulCrumb.Fields.Name);
        }
    }
}
