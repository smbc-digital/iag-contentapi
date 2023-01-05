using FluentAssertions;
using StockportContentApi.ContentfulFactories.EventFactories;
using StockportContentApi.Model;
using StockportContentApiTests.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class EventCategoryContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAEventCategoryFromAContentfulEventCategory()
        {

            var contentfulShowcase = new ContentfulEventCategoryBuilder()
                .Name("category name")
                .Slug("category-slug")
                .Icon("icon")
                .Build();

            var contentfulFactory = new EventCategoryContentfulFactory();

            var category = contentfulFactory.ToModel(contentfulShowcase);

            category.Should().BeOfType<EventCategory>();
            category.Name.Should().Be("category name");
            category.Slug.Should().Be("category-slug");
            category.Icon.Should().Be("icon");
        }
    }
}
