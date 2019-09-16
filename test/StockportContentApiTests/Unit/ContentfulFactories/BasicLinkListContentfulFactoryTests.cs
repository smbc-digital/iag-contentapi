using StockportContentApi.ContentfulFactories;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class BasicLinkListContentfulFactoryTests
    {

        [Fact]
        public void ToModel_ShouldReturnLinkList()
        {
            // Arrange
            var linkedList = new List<ContentfulBasicLink>
            {
                new ContentfulBasicLink
                {
                    Url = "12",
                    Text = "12",
                    Sys = new SystemProperties()
                },
                new ContentfulBasicLink
                {
                    Url = "1234",
                    Text = "1234",
                    Sys = new SystemProperties()
                }
            };

            // Act
            var result = new BasicLinkListContentfulFactory().ToModel(linkedList);

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}
