using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupContentfulFactoryTest
    {
        private readonly ContentfulGroup _contentfulGroup;
        private readonly Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>> _crumbFactory;
        private readonly GroupContentfulFactory _groupContentfulFactory;

        public GroupContentfulFactoryTest()
        {
            _contentfulGroup = new ContentfulGroupBuilder().Build();
         
            _groupContentfulFactory = new GroupContentfulFactory();
        }

        [Fact]
        public void ShouldCreateAGroupFromAContentfulGroup()
        {
            var group = _groupContentfulFactory.ToModel(_contentfulGroup);

            group.ShouldBeEquivalentTo(_contentfulGroup);
        }

        //[Fact]
        //public void ShouldNotAddBreadcrumbsOrImageIfTheyAreLinks()
        //{
        //    _contentfulGroup.Image.SystemProperties.Type = "Link";
        //    _contentfulGroup.BackgroundImage.SystemProperties.Type = "Link";
        //    _contentfulGroup.Breadcrumbs.First().SystemProperties.Type = "Link";

        //    var group = _groupContentfulFactory.ToModel(_contentfulGroup);

        //    _crumbFactory.Verify(o => o.ToModel(It.IsAny<Entry<ContentfulCrumb>>()), Times.Never);
        //    group.Breadcrumbs.Count().Should().Be(0);
        //    group.BackgroundImage.Should().BeEmpty();
        //    group.Image.Should().BeEmpty();
        //}
    }
}
