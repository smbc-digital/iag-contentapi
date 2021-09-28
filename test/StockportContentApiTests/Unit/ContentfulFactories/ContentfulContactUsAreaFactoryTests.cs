using System.Collections.Generic;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ContentfulContactUsAreaFactoryTests
    {
        private readonly ContactUsAreaContentfulFactory _factory;
        private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _mockSubitemFactory = new();
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _mockCrumbFactory = new();
        private readonly Mock<ITimeProvider> _mockTimeProvider = new();
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _mockAlertFactory = new();
        private readonly Mock<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>> _mockContactUsCategoryFactory = new();

        public ContentfulContactUsAreaFactoryTests()
        {
            _factory = new ContactUsAreaContentfulFactory(_mockSubitemFactory.Object, 
                _mockCrumbFactory.Object, 
                _mockTimeProvider.Object, 
                _mockAlertFactory.Object,
                _mockContactUsCategoryFactory.Object);
        }

        [Fact]
        public void ShouldCreate_ValidContentfulContactUsAreModel()
        {
            var entry = new ContentfulContactUsAreaBuilder()
                                .Build();

            var result = _factory.ToModel(entry);

            Assert.NotNull(result.Breadcrumbs);
            Assert.NotNull(result.Alerts);
            Assert.NotNull(result.InsetTextTitle);
            Assert.NotNull(result.InsetTextBody);
            Assert.NotNull(result.PrimaryItems);
            Assert.NotNull(result.Slug);
            Assert.NotNull(result.Title);
            Assert.NotNull(result.CategoriesTitle);
            Assert.Equal("title", result.Title);
            Assert.Equal("slug", result.Slug);
            Assert.NotNull(result.ContactUsCategories);
            Assert.Equal("metaDescription", result.MetaDescription);
            Assert.Equal("insetTextTitle", result.InsetTextTitle);
            Assert.Equal("insetTextBody", result.InsetTextBody);
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
            Assert.NotNull(result.InsetTextTitle);
            Assert.NotNull(result.InsetTextBody);
            Assert.Single(result.PrimaryItems);
            Assert.NotNull(result.Slug);
            Assert.NotNull(result.Title);
            Assert.NotNull(result.CategoriesTitle);
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

            var contactUsCategories = new List<ContentfulContactUsCategory>
            {
                new ContentfulContactUsCategory()
            };

            var entry = new ContentfulContactUsAreaBuilder()
                                .PrimaryItems(primaryItems)
                                .Breadcrumbs(breadcrumbs)
                                .Alerts(alerts)
                                .ContentfulContactUsCategories(contactUsCategories)
                                .Build();

            _mockSubitemFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItem());

            var result = _factory.ToModel(entry);

            Assert.Single(result.Breadcrumbs);
            Assert.Single(result.Alerts);
            Assert.Single(result.PrimaryItems);
            Assert.Single(result.ContactUsCategories);
            Assert.NotNull(result.Slug);
            Assert.NotNull(result.Title);
            Assert.NotNull(result.CategoriesTitle);
            Assert.NotNull(result.InsetTextTitle);
            Assert.NotNull(result.InsetTextBody);
        }
    }
}
