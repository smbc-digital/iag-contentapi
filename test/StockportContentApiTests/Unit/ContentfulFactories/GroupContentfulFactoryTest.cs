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
    }
}
