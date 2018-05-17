using FluentAssertions;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class PrivacyNoticeContentfulFactoryTest
    {
        private readonly PrivacyNoticeContentfulFactory _privacyNoticeContentfulFactory;
        private Mock<IContentfulFactory<ContentfulReference, Crumb>> _mockCrumbFactory;
        private Mock<IContentfulFactory<ContentfulPrivacyNotice, Topic>> _mockTopicFactory;


        public PrivacyNoticeContentfulFactoryTest()
        {
            _mockCrumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            _mockTopicFactory = new Mock<IContentfulFactory<ContentfulPrivacyNotice, Topic>>();
            _privacyNoticeContentfulFactory = new PrivacyNoticeContentfulFactory(_mockCrumbFactory.Object, _mockTopicFactory.Object);
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
                Category = "test-category",
                Purpose = "test-purpose",
                TypeOfData = "test-type-of-data",
                Legislation = "test-legislation",
                Obtained = "test-obtained",
                ExternallyShared = "test-externally-shared",
                RetentionPeriod = "test-retention-period",
                OutsideEu = false,
                AutomatedDecision = false,
                UrlOne = "test-url-1",
                UrlTwo = "test-url-2",
                UrlThree = "test-url-3",
                Breadcrumbs= new List<ContentfulReference>()
            };
            // Act
            var privacyNotice = _privacyNoticeContentfulFactory.ToModel(contentfulPrivacyNotice);
            // Assert
            privacyNotice.ShouldBeEquivalentTo(contentfulPrivacyNotice, p => p
                .Excluding(e => e.ParentTopic));
        }
    }
}
