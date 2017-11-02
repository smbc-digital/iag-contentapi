using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class SmartResultContentfulFactoryTests
    {
        public SmartResultContentfulFactory _smartResultContentfulFactory { get; set; }

        public SmartResultContentfulFactoryTests()
        {
            _smartResultContentfulFactory = new SmartResultContentfulFactory();        
        }

        [Fact]
        public void ToModel_ShouldSetButtonTextIfNotProvided()
        {
            // Arrange
            var contentfulentry = new ContentfulSmartResult
            {
                Slug = "a-slug",
                ButtonLink = "link"
            };

            // Act
            var result = _smartResultContentfulFactory.ToModel(contentfulentry);

            // Assert
            result.ButtonText.Should().Be("Go to Homepage");

        }

        [Fact]
        public void ToModel_ShouldSetButtonLinkIfNotProvided()
        {
            // Arrange
            var contentfulentry = new ContentfulSmartResult
            {
                Slug = "a-slug",
                ButtonText = "Hello"
            };

            // Act
            var result = _smartResultContentfulFactory.ToModel(contentfulentry);

            // Assert
            result.ButtonLink.Should().Be("https://stockport.gov.uk/");

        }

    }
}
