using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulFactories.GroupFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupContentfulFactoryTest
    {
        private readonly ContentfulGroup _contentfulGroup;
        private readonly GroupContentfulFactory _groupContentfulFactory;
        private readonly ContentfulEventBuilder _contentfulEventBuilder = new ContentfulEventBuilder();

        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
        private readonly Mock<IContentfulFactory<ContentfulOrganisation, Organisation>> _contentfulOrganisationFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _contentfulGroupCategoryFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>> _contentfulGroupSubCategoryFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroupBranding, GroupBranding>> _contentfulGroupBrandingFactory;
        private Mock<ITimeProvider> _timeProvider;
        private Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;

        public GroupContentfulFactoryTest()
        {
            _timeProvider = new Mock<ITimeProvider>();

            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));
            _contentfulOrganisationFactory = new Mock<IContentfulFactory<ContentfulOrganisation, Organisation>>();
            _contentfulGroupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
            _contentfulGroupSubCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>();
            _contentfulGroupBrandingFactory = new Mock<IContentfulFactory<ContentfulGroupBranding, GroupBranding>>();
            _contentfulGroup = new ContentfulGroupBuilder().Build();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _groupContentfulFactory = new GroupContentfulFactory(_contentfulOrganisationFactory.Object, _contentfulGroupCategoryFactory.Object, _contentfulGroupSubCategoryFactory.Object, _timeProvider.Object, _documentFactory.Object, _contentfulGroupBrandingFactory.Object, _alertFactory.Object);
        }

        [Fact]
        public void ShouldCreateAGroupFromAContentfulGroup()
        {
            // Arrange
            var crumb = new Crumb("Stockport Local", string.Empty, "groups");
            var category = new GroupCategory("name", "slug", "icon", "imageUrl");
            var administrators = new GroupAdministrators
            {
                Items = new List<GroupAdministratorItems>
                {
                    new GroupAdministratorItems
                    {
                        Name = "Name",
                        Email = "Email",
                        Permission = "admin"
                    }
                }
            };
            var mapPosition = new MapPosition
            {
                Lat = 39,
                Lon = 2
            };
            var subCategories = new List<GroupSubCategory>
            {
                new GroupSubCategory("name", "slug")
            };
            var organisation = new Organisation
            {
                Title = "Org"
            };
            var suitableFor = new List<string>
            {
                "people"
            };
            var document = new DocumentBuilder().Build();

            _documentFactory.Setup(_ => _.ToModel(It.IsAny<Asset>())).Returns(document);
            _contentfulGroupCategoryFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulGroupCategory>()))
                .Returns(category);
            _contentfulGroupSubCategoryFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulGroupSubCategory>()))
                .Returns(subCategories.First());
            _contentfulOrganisationFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulOrganisation>()))
                .Returns(organisation);

            // Act
            var result = _groupContentfulFactory.ToModel(_contentfulGroup);

            // Assert
            result.AbilityLevel.Should().Be("");
            result.AccessibleTransportLink.Should().Be("link");
            result.AdditionalDocuments.Count.Should().Be(1);
            result.AdditionalDocuments.First().Should().BeEquivalentTo(document);
            result.AdditionalInformation.Should().Be("info");
            result.Address.Should().Be("_address");

            result.AgeRange.Count.Should().Be(1);
            result.AgeRange.First().Should().BeEquivalentTo("15-20");

            result.Breadcrumbs.Count.Should().Be(1);
            result.Breadcrumbs.First().Should().BeEquivalentTo(crumb);

            result.CategoriesReference.Count.Should().Be(1);
            result.CategoriesReference.First().Should().BeEquivalentTo(category);

            result.Cost.Count.Should().Be(1);
            result.CostText.Should().Be("");

            result.DateHiddenFrom.Should().Be(DateTime.MinValue);
            result.DateHiddenTo.Should().Be(DateTime.MinValue);
            result.DateLastModified.Should().Be(DateTime.MinValue);

            result.Email.Should().Be("_email");
            result.Description.Should().Be("_description");
            result.Donations.Should().BeFalse();
            result.DonationsText.Should().Be("donText");
            result.DonationsUrl.Should().Be("donUrl");
            result.ImageUrl.Should().Be("image-url.jpg");
            result.GroupAdministrators.Should().BeEquivalentTo(administrators);
            result.MapPosition.Should().BeEquivalentTo(mapPosition);
            result.Name.Should().Be("_name");
            result.Slug.Should().Be("_slug");
            result.PhoneNumber.Should().Be("_phoneNumber");
            result.Website.Should().Be("_website");
            result.Twitter.Should().Be("_twitter");
            result.Facebook.Should().Be("_facebook");
            result.Volunteering.Should().Be(true);
            result.VolunteeringText.Should().Be("text");
            result.SubCategories.Should().BeEquivalentTo(subCategories);
            result.Status.Should().Be("Published");
            result.Organisation.Should().BeEquivalentTo(organisation);
            result.SuitableFor.Should().BeEquivalentTo(suitableFor);
            result.MetaDescription.Should().Be("_metaDescription");
        }
    }
}
