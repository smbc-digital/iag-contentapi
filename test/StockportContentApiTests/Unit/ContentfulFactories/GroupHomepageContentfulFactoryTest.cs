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
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupHomepageContentfulFactoryTest
    {
        private readonly ContentfulGroupHomepage _contentfulGroupHomepage;
        private readonly GroupHomepageContentfulFactory _groupHomepageContentfulFactory;
        private Mock<IContentfulFactory<List<ContentfulGroup>, List<Group>>> _groupListFactory;
        private Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _groupCategoryFactory;
        private Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>> _groupSubCategoryFactory;
        private readonly Mock<ITimeProvider> _mockTimeProvider;

        public GroupHomepageContentfulFactoryTest()
        {
            _groupListFactory = new Mock<IContentfulFactory<List<ContentfulGroup>, List<Group>>>();
            _groupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
            _groupSubCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>();
            _contentfulGroupHomepage = new ContentfulGroupHomepageBuilder().Build();
            _mockTimeProvider = new Mock<ITimeProvider>();

            _groupHomepageContentfulFactory = new GroupHomepageContentfulFactory(_groupListFactory.Object, _groupCategoryFactory.Object, _groupSubCategoryFactory.Object, _mockTimeProvider.Object, HttpContextFake.GetHttpContextFake());
        }

        [Fact]
        public void ShouldReturnGroupHomepage()
        {
            _groupListFactory.Setup(o => o.ToModel(It.IsAny<List<ContentfulGroup>>())).Returns(new List<Group> {new GroupBuilder().Build()});
            _groupCategoryFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroupCategory>())).Returns(new GroupCategory("title", "slug", "icon", "image"));
            _groupSubCategoryFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroupSubCategory>())).Returns(new GroupSubCategory("title","slug"));
            
            var groupHomepage = _groupHomepageContentfulFactory.ToModel(_contentfulGroupHomepage);
            groupHomepage.ShouldBeEquivalentTo(_contentfulGroupHomepage, o => o.Excluding(e => e.BackgroundImage).Excluding(e => e.FeaturedGroups).Excluding(e => e.FeaturedGroupsCategory).Excluding(e => e.FeaturedGroupsSubCategory));
            groupHomepage.BackgroundImage.Should().Be(_contentfulGroupHomepage.BackgroundImage.File.Url);
        }
        
    }
}
