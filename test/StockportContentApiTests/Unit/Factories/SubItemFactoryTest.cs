using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class SubItemFactoryTest
    {
        private readonly IFactory<SubItem> _factory;

        public SubItemFactoryTest()
        {
            _factory = new SubItemFactory();
        }

        [Fact]
        public void ItBuildsASubItemFromEntryOfContentTypeTopic()
        {
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Entry/SubItemEntryOfTypeTopic.json"));

            SubItem subItem = _factory.Build(data, new NullContentfulResponse());

            subItem.Title.Should().Be("Healthy Living");
            subItem.Slug.Should().Be("healthy-living");
            subItem.Teaser.Should().Be("teaser");
            subItem.Icon.Should().Be("fa-leaf");
            subItem.Type.Should().Be("topic");
        }

        [Fact]
        public void ItBuildsASubItemFromEntryOfContentTypeArticle()
        {
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Entry/SubItemEntryOfTypeArticle.json"));

            SubItem subItem = _factory.Build(data, new NullContentfulResponse());

            subItem.Title.Should().Be("Test Video");
            subItem.Type.Should().Be("article");
        }

        [Fact]
        public void ItBuildsASubItemFromEntryOfContentTypeStartPage()
        {
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Entry/SubItemEntryOfTypeStartPage.json"));

            SubItem subItem = _factory.Build(data, new NullContentfulResponse());

            subItem.Type.Should().Be("start-page");
        }

        [Fact]
        public void ItBuildsASubItemFromEntryWithoutIcon()
        {
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Entry/SubItemEntryWithoutIcon.json"));

            SubItem subItem = _factory.Build(data, new NullContentfulResponse());

            subItem.Icon.Should().Be(string.Empty);
        }
    }
}
