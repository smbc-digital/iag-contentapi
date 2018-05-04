using FluentAssertions;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class PrivacyNoticeContentfulFactoryTest
    {
        private readonly PrivacyNoticeContentfulFactory _privacyNoticeContentfulFactory;

        public PrivacyNoticeContentfulFactoryTest()
        {
            _privacyNoticeContentfulFactory = new PrivacyNoticeContentfulFactory();
        }

        [Fact]
        public void ToModel_ShouldReturnPrivacyNotice()
        {
            // Arrange
            var contentfulPrivacyNotice = new ContentfulPrivacyNotice();
            // Act
            var privacyNotice = _privacyNoticeContentfulFactory.ToModel(contentfulPrivacyNotice);
            // Assert
            privacyNotice.Should().BeOfType<PrivacyNotice>();
        }

        [Fact]
        public void ToModel_ShouldConvertContentfulPrivacyNoticeToPrivacyNotice()
        {
            // Arrange
            var contentfulPrivacyNotice = new ContentfulPrivacyNotice()
            {
                Slug = 
            };
            // Act

            // Assert
        }
    }
}
