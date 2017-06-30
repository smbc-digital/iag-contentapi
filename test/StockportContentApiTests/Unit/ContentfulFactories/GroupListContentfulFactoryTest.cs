using System;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
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
    public class GroupListContentfulFactoryTest
    {
        private readonly List<ContentfulGroup> _contentfulGroupList;
        private readonly GroupListContentfulFactory _groupListContentfulFactory;
        private readonly IContentfulFactory<ContentfulGroup, Group> _contentfulGroupFactory;
        private readonly Mock<ITimeProvider> _timeProvider;

        public GroupListContentfulFactoryTest()
        {
            _timeProvider = new Mock<ITimeProvider>();

            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));
            _contentfulGroupFactory = new GroupContentfulFactory(new GroupCategoryContentfulFactory(), _timeProvider.Object);
            _contentfulGroupList = new List<ContentfulGroup> { new ContentfulGroupBuilder().Build() };

            _groupListContentfulFactory = new GroupListContentfulFactory(_contentfulGroupFactory);
        }

        [Fact]
        public void ShouldCreateAGroupFromAContentfulGroup()
        {
            // Arrange
            

            // Act
            var groupList = _groupListContentfulFactory.ToModel(_contentfulGroupList);

            // Assert
            groupList[0].ShouldBeEquivalentTo(_contentfulGroupList[0], o => o.Excluding(e => e.ImageUrl).Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Events).Excluding(e => e.Breadcrumbs));
        }
    }
}
