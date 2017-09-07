using System;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupContentfulFactoryTest
    {
        private readonly ContentfulGroup _contentfulGroup;
        private readonly GroupContentfulFactory _groupContentfulFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _contentfulGroupCategoryFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>> _contentfulGroupSubCategoryFactory;
        private Mock<ITimeProvider> _timeProvider;

        public GroupContentfulFactoryTest()
        {
            _timeProvider = new Mock<ITimeProvider>();

            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));
            _contentfulGroupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
            _contentfulGroupSubCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>();
            _contentfulGroup = new ContentfulGroupBuilder().Build();
         
            _groupContentfulFactory = new GroupContentfulFactory(_contentfulGroupCategoryFactory.Object, _contentfulGroupSubCategoryFactory.Object, _timeProvider.Object);
        }

        [Fact]
        public void ShouldCreateAGroupFromAContentfulGroup()
        {
            var group = _groupContentfulFactory.ToModel(_contentfulGroup);
            group.ShouldBeEquivalentTo(_contentfulGroup, o => o.Excluding(e => e.ImageUrl).Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Events).Excluding(e => e.Breadcrumbs).Excluding(e => e.Status).Excluding(e => e.Cost));
        }
    }
}
