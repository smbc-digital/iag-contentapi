using Xunit;
using FluentAssertions;
using StockportContentApi.Model;
using StockportContentApiTests.Builders;
using StockportContentApi.ContentfulFactories.EventFactories;
using StockportContentApi.Fakes;

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

            var contentfulFactory = new EventCategoryContentfulFactory(HttpContextFake.GetHttpContextFake());

            var category = contentfulFactory.ToModel(contentfulShowcase);

            category.Should().BeOfType<EventCategory>();
            category.Name.Should().Be("category name");
            category.Slug.Should().Be("category-slug");
            category.Icon.Should().Be("icon");
        }
    }
}
