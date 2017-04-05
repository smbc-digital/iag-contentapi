using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.ContentfulFactories;
using Xunit;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Factories
{
    public class GroupContentfulFactoryTest
    {
        private readonly GroupContentfulFactory _groupContentfulFactory;

        private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>
            _contentfulGroupCategoryFactory;


        public GroupContentfulFactoryTest()
        {
            _contentfulGroupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
            _groupContentfulFactory = new GroupContentfulFactory(_contentfulGroupCategoryFactory.Object);           
        }

        [Fact]
        public void ShouldReturngroup()
        {
            const string slug = "group_slug";
            var contentfulGroup = new ContentfulGroupBuilder().Slug(slug).Build();
            var group = _groupContentfulFactory.ToModel(contentfulGroup);
            group.Name.Should().Be(contentfulGroup.Name);
            group.Address.Should().Be(contentfulGroup.Address);
            group.Email.Should().Be(contentfulGroup.Email);
            group.Facebook.Should().Be(contentfulGroup.Facebook);
            group.PhoneNumber.Should().Be(contentfulGroup.PhoneNumber);
            group.Slug.Should().Be(contentfulGroup.Slug);
            group.Twitter.Should().Be(contentfulGroup.Twitter);
            group.Website.Should().Be(contentfulGroup.Website);
            group.ImageUrl.Should().Be(contentfulGroup.Image.File.Url);
            group.CategoriesReference.Count.Should().Be(contentfulGroup.CategoriesReference.Count);
        }
    }
}
