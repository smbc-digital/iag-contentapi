using System.Collections.Generic;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Fakes;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ContentfulContactUsAreaFactoryTests
    {
        private readonly ContactUsAreaContentfulFactory _factory;
        private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _mockSubitemFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _mockCrumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
        private readonly Mock<ITimeProvider> _mockTimeProvider = new Mock<ITimeProvider>();
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _mockAlertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        private readonly Mock<IContentfulFactory<ContentfulInsetText, InsetText>> _mockInsetTextFactory = new Mock<IContentfulFactory<ContentfulInsetText, InsetText>>();
        private readonly Mock<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>> _mockContactUsCategoryFactory = new Mock<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>>();

        public ContentfulContactUsAreaFactoryTests()
        {
            _factory = new ContactUsAreaContentfulFactory(_mockSubitemFactory.Object, HttpContextFake.GetHttpContextFake(), _mockCrumbFactory.Object, _mockTimeProvider.Object, _mockAlertFactory.Object, _mockInsetTextFactory.Object, _mockContactUsCategoryFactory.Object);
        }

        [Fact]
        public void ShouldCreate_ValidContentfulContactUsAreModel()
        {
            var entry = new ContentfulContactUsAreaBuilder()
                                .Build();

            var result = _factory.ToModel(entry);

            Assert.NotNull(result.Breadcrumbs);
            Assert.NotNull(result.Alerts);
            Assert.NotNull(result.InsetTexts);
            Assert.NotNull(result.PrimaryItems);
            Assert.NotNull(result.Slug);
            Assert.NotNull(result.Title);
            Assert.Equal("title", result.Title);
            Assert.Equal("slug", result.Slug);
            Assert.NotNull(result.ContactUsCategories);
        }

        [Fact]
        public void ShouldCreate_ValidContentfulContactUsAreModel_WithPrimaryItems()
        {
            var primaryItems = new List<ContentfulReference>
            {
                new ContentfulReference
                {
                }
            };
            var entry = new ContentfulContactUsAreaBuilder()
                                .PrimaryItems(primaryItems)
                                .Build();

            _mockSubitemFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItem());

            var result = _factory.ToModel(entry);

            Assert.NotNull(result.Breadcrumbs);
            Assert.NotNull(result.Alerts);
            Assert.NotNull(result.InsetTexts);
            Assert.Single(result.PrimaryItems);
            Assert.NotNull(result.Slug);
            Assert.NotNull(result.Title);
        }

        [Fact]
        public void ShouldCreate_ValidContentfulContactUsAreModel_WithAllItems()
        {
            var primaryItems = new List<ContentfulReference>
            {
                new ContentfulReference()
            };

            var breadcrumbs = new List<ContentfulReference>
            {
                new ContentfulReference()
            };

            var alerts = new List<ContentfulAlert>
            {
                new ContentfulAlert()
            };

            var insetTexts = new List<ContentfulInsetText>
            {
                new ContentfulInsetText()
            };

            var contactUsCategories = new List<ContentfulContactUsCategory>
            {
                new ContentfulContactUsCategory()
            };

            var entry = new ContentfulContactUsAreaBuilder()
                                .PrimaryItems(primaryItems)
                                .Breadcrumbs(breadcrumbs)
                                .Alerts(alerts)
                                .InsetTexts(insetTexts)
                                .ContentfulContactUsCategories(contactUsCategories)
                                .Build();

            _mockSubitemFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItem());

            var result = _factory.ToModel(entry);

            Assert.Single(result.Breadcrumbs);
            Assert.Single(result.Alerts);
            Assert.Single(result.InsetTexts);
            Assert.Single(result.PrimaryItems);
            Assert.Single(result.ContactUsCategories);
            Assert.NotNull(result.Slug);
            Assert.NotNull(result.Title);
        }
    }
}
