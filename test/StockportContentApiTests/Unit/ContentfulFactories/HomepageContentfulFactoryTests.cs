namespace StockportContentApiTests.Unit.ContentfulFactories;

public class HomepageContentfulFactoryTests
{
    private readonly HomepageContentfulFactory _homepageContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemFactory;
    private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private readonly Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>> _carouselContentFactory;
    private readonly Mock<ITimeProvider> _timeProvider;
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory;

    public HomepageContentfulFactoryTests()
    {
        _subitemFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
        _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _carouselContentFactory = new Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>>();
        _timeProvider = new Mock<ITimeProvider>();
        _callToActionFactory = new Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>();

        // mocks
        _groupFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroup>())).Returns(new GroupBuilder().Build());
        _subitemFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItemBuilder().Build());
        _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false, string.Empty));
        _carouselContentFactory.Setup(o => o.ToModel(It.IsAny<ContentfulCarouselContent>())).Returns(new CarouselContent("", "", "", "", DateTime.MinValue, DateTime.MaxValue, ""));
        _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));
        _callToActionFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(new CallToActionBanner());

        _homepageContentfulFactory = new HomepageContentfulFactory(_subitemFactory.Object,
            _groupFactory.Object,
            _alertFactory.Object,
            _carouselContentFactory.Object,
            _timeProvider.Object,
            _callToActionFactory.Object);
    }

    [Fact]
    public void ShouldBuildHomepageFromContentfulHomepage()
    {
        var contentfulHomepage = new ContentfulHomepageBuilder().Build();

        var homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

        homepage.FeaturedGroup.Should().NotBeNull();
        homepage.MetaDescription.Should().BeEquivalentTo("meta description");
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
                new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(3000, 01, 01)).Build(),
                new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(3000, 01, 01)).Build(),
                new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(3000, 01, 01)).Build()
            }).Build();

        var homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

        homepage.FeaturedGroup.Should().BeNull();
    }
}
