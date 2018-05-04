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
                Slug = "test-slug",
                Title = "test-title",
                Directorate = "test-directorate",
                ActivitiesAsset = "test-activities-asset",
                TransactionsActivity = "test-transactions-activity",
                Purpose = "test-purpose",
                TypeOfData = "test-type-of-data",
                Legislation = "test-legislation",
                Obtained = "test-obtained",
                ExternallyShared = "test-externally-shared",
                RetentionPeriod = "test-retention-period",
                Conditions = "test-conditions",
                ConditionsSpecial = "test-conditions-special",
                UrlOne = "test-url-1",
                UrlTwo = "test-url-2",
                UrlThree = "test-url-3"
            };
            // Act
            var privacyNotice = _privacyNoticeContentfulFactory.ToModel(contentfulPrivacyNotice);
            // Assert
            privacyNotice.ShouldBeEquivalentTo(contentfulPrivacyNotice);
        }
    }
}
