using FluentAssertions;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Unit.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using Xunit;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulFactories.GroupFactories;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupHomepageContentfulFactoryTest
    {
        private readonly ContentfulGroupHomepage _contentfulGroupHomepage;
        private readonly GroupHomepageContentfulFactory _groupHomepageContentfulFactory;
        private Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
        private Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _groupCategoryFactory;
        private Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>> _groupSubCategoryFactory;
        private Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
        private readonly Mock<ITimeProvider> _mockTimeProvider;

        public GroupHomepageContentfulFactoryTest()
        {
            _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
            _groupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
            _groupSubCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>();
            _contentfulGroupHomepage = new ContentfulGroupHomepageBuilder().Build();
            _mockTimeProvider = new Mock<ITimeProvider>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();

            _groupHomepageContentfulFactory = new GroupHomepageContentfulFactory(_groupFactory.Object, _groupCategoryFactory.Object, _groupSubCategoryFactory.Object, _mockTimeProvider.Object, HttpContextFake.GetHttpContextFake(), _alertFactory.Object);
        }

        [Fact]
        public void ShouldReturnGroupHomepage()
        {
            _groupFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroup>())).Returns(new GroupBuilder().Build());
            _groupCategoryFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroupCategory>())).Returns(new GroupCategory("title", "slug", "icon", "image"));
            _groupSubCategoryFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroupSubCategory>())).Returns(new GroupSubCategory("title","slug"));
            
            var groupHomepage = _groupHomepageContentfulFactory.ToModel(_contentfulGroupHomepage);
            groupHomepage.ShouldBeEquivalentTo(_contentfulGroupHomepage, o => o.Excluding(e => e.BackgroundImage).Excluding(e => e.FeaturedGroups).Excluding(e => e.FeaturedGroupsCategory).Excluding(e => e.FeaturedGroupsSubCategory));
            groupHomepage.BackgroundImage.Should().Be(_contentfulGroupHomepage.BackgroundImage.File.Url);
        }
        
    }
}
