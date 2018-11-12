using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class HomepageContentfulFactoryTests
    {
        private readonly HomepageContentfulFactory _homepageContentfulFactory;
        private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
        private readonly Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>> _carouselContentFactory;
        private readonly Mock<ITimeProvider> _timeProvider;

        public HomepageContentfulFactoryTests()
        {
            _subitemFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
            _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _carouselContentFactory = new Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>>();
            _timeProvider = new Mock<ITimeProvider>();

            // mocks
            _groupFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroup>())).Returns(new GroupBuilder().Build());
            _subitemFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItemBuilder().Build());
            _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty));
            _carouselContentFactory.Setup(o => o.ToModel(It.IsAny<ContentfulCarouselContent>())).Returns(new CarouselContent("", "", "", "", DateTime.MinValue, DateTime.MaxValue, ""));
            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _homepageContentfulFactory = new HomepageContentfulFactory(_subitemFactory.Object, 
                _groupFactory.Object, 
                _alertFactory.Object,
                _carouselContentFactory.Object,
                _timeProvider.Object,
                HttpContextFake.GetHttpContextFake());
        }

        [Fact]
        public void ShouldBuildHomepageFromContentfulHomepage()
        {
            var contentfulHomepage = new ContentfulHomepageBuilder().Build();

            var homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

            homepage.FeaturedGroup.Should().NotBeNull();
        }

        [Fact]
        public void ShouldBuildHomepageWithNoContentfulGroup()
        {
            var contentfulHomepage = new ContentfulHomepageBuilder().FeaturedGroups(new List<ContentfulGroup>()).Build();

            var homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

            homepage.FeaturedGroup.Should().BeNull();
        }

        [Fact]
        public void ShouldPickFirstAvaliableFeaturedGroup()
        {
            var contentfulHomepage = new ContentfulHomepageBuilder()
                .FeaturedGroups(new List<ContentfulGroup>()
                {
                    new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(2018, 01, 01)).Build(),
                    new ContentfulGroupBuilder().Slug("a-custom-slug").Build(),
                    new ContentfulGroupBuilder().Build()
                }).Build();

            var homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

            homepage.FeaturedGroup.Should().NotBeNull();
        }

        [Fact]
        public void ShouldNotFailIfNoGroupsCanBeUsed()
        {
            var contentfulHomepage = new ContentfulHomepageBuilder()
                .FeaturedGroups(new List<ContentfulGroup>()
                {
                    new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(2022, 01, 01)).Build(),
                    new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(2022, 01, 01)).Build(),
                    new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(2022, 01, 01)).Build()
                }).Build();

            var homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

            homepage.FeaturedGroup.Should().BeNull();
        }
    }
}
