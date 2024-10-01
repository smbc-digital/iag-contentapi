namespace StockportContentApiTests.Unit.ContentfulFactories;

public class GroupHomepageContentfulFactoryTests
{
    private readonly ContentfulGroupHomepage _contentfulGroupHomepage;
    private readonly GroupHomepageContentfulFactory _groupHomepageContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
    private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _groupCategoryFactory;
    private readonly Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>> _groupSubCategoryFactory;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private readonly Mock<ITimeProvider> _mockTimeProvider;
    private readonly Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>> _eventBannerFactory;

    public GroupHomepageContentfulFactoryTests()
    {
        _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
        _groupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
        _groupSubCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>();
        _contentfulGroupHomepage = new ContentfulGroupHomepageBuilder().Build();
        _mockTimeProvider = new Mock<ITimeProvider>();
        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _eventBannerFactory = new Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>>();

        _groupHomepageContentfulFactory = new GroupHomepageContentfulFactory(_groupFactory.Object, _groupCategoryFactory.Object, _groupSubCategoryFactory.Object, _mockTimeProvider.Object, _alertFactory.Object, _eventBannerFactory.Object);
    }

    [Fact]
    public void ShouldReturnGroupHomepage()
    {
        // Arrange
        EventBanner eventBanner = new("title", "teaser", "icon", "link", EColourScheme.Teal_Light);
        Group featuredGroup = new GroupBuilder().Build();
        GroupCategory category = new("title", "slug", "icon", "image");
        GroupSubCategory subCategory = new("title", "slug");
        Alert alert = new("title", "subheading", "body", "severity", DateTime.MinValue, DateTime.MinValue,
            "slug", false, string.Empty);

        _groupFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroup>())).Returns(featuredGroup);
        _groupCategoryFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroupCategory>())).Returns(category);
        _groupSubCategoryFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroupSubCategory>())).Returns(subCategory);
        _eventBannerFactory.Setup(o => o.ToModel(_contentfulGroupHomepage.EventBanner)).Returns(eventBanner);
        _alertFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulAlert>())).Returns(alert);

        // Act
        GroupHomepage result = _groupHomepageContentfulFactory.ToModel(_contentfulGroupHomepage);

        // Assert
        result.Title.Should().Be("title");
        result.Slug.Should().Be("slug");
        result.MetaDescription.Should().Be("metaDescription");
        result.BackgroundImage.Should().Be("image-url.jpg");
        result.FeaturedGroupsHeading.Should().Be("heading");

        result.FeaturedGroups.Count.Should().Be(1);
        result.FeaturedGroups.First().Should().BeEquivalentTo(featuredGroup);

        result.FeaturedGroupsCategory.Should().BeEquivalentTo(category);
        result.FeaturedGroupsSubCategory.Should().BeEquivalentTo(subCategory);

        result.Alerts.Count().Should().Be(1);
        result.Alerts.First().Should().BeEquivalentTo(alert);

        result.BodyHeading.Should().Be("bodyheading");
        result.Body.Should().Be("body");
        result.SecondaryBodyHeading.Should().Be("secondaryBodyHeading");
        result.SecondaryBody.Should().Be("secondaryBody");
        result.EventBanner.Should().BeEquivalentTo(eventBanner);
    }
}