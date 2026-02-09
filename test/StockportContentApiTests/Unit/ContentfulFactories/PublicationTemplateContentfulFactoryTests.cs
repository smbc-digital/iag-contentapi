using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PublicationTemplateContentfulFactoryTests
{
    private readonly ITimeProvider _timeProvider = new TimeProvider();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulPublicationPage, PublicationPage>> _publicationPageFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>> _trustedLogoFactory = new();
    readonly ContentfulPublicationTemplate _contentfulPublicationTemplate = new ContentfulPublicationTemplateBuilder().Build();
    private readonly PublicationTemplateContentfulFactory _publicationTemplateFactory;

    public PublicationTemplateContentfulFactoryTests() =>
            _publicationTemplateFactory = new PublicationTemplateContentfulFactory(_publicationPageFactory.Object,
                                                                _crumbFactory.Object,
                                                                _timeProvider,
                                                                _trustedLogoFactory.Object);

    [Fact]
    public void ToModel_ShouldCreateAPublicationTemplateFromAContentfulPublicationTemplate()
    {
        // Arrange
        Crumb crumb = new("title", "slug", "type");
        _contentfulPublicationTemplate.Breadcrumbs = new List<ContentfulReference>() { new ContentfulReferenceBuilder().Build() };
        _contentfulPublicationTemplate.PublicationPages = new List<ContentfulPublicationPage>() { new ContentfulPublicationPageBuilder().Build() };

        _crumbFactory
            .Setup(crumbFactory => crumbFactory.ToModel(_contentfulPublicationTemplate.Breadcrumbs.First()))
            .Returns(crumb);

        // Act
        PublicationTemplate result = _publicationTemplateFactory.ToModel(_contentfulPublicationTemplate);

        // Assert
        Assert.Equal("slug", result.Slug);
        Assert.Equal("title", result.Title);
        Assert.Equal("meta description", result.MetaDescription);
        Assert.Equal("subtitle", result.Subtitle);
        Assert.Equal("summary", result.Summary);
        Assert.Equal(crumb, result.Breadcrumbs.First());
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(_contentfulPublicationTemplate.Breadcrumbs.First()), Times.Once);
        _publicationPageFactory.Verify(publicationPageFactory => publicationPageFactory.ToModel(_contentfulPublicationTemplate.PublicationPages.First()), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldNotAddBreadcrumbsOrPublicationPages_If_TheyAreLinks()
    {
        // Arrange
        ContentfulPublicationTemplate contentfulPublicationTemplate = new ContentfulPublicationTemplateBuilder().Build();

        contentfulPublicationTemplate.Breadcrumbs.First().Sys.LinkType = "Link";
        contentfulPublicationTemplate.PublicationPages.First().Sys.LinkType = "Link";

        // Act
        PublicationTemplate publicationTemplate = _publicationTemplateFactory.ToModel(contentfulPublicationTemplate);

        // Assert
        Assert.Empty(publicationTemplate.Breadcrumbs);
        Assert.Empty(publicationTemplate.PublicationPages);
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(contentfulPublicationTemplate.Breadcrumbs.First()), Times.Never);
        _publicationPageFactory.Verify(publicationPageFactory => publicationPageFactory.ToModel(contentfulPublicationTemplate.PublicationPages.First()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_IfNullEntry()
    {
        // Act & Assert
        Assert.Null(_publicationTemplateFactory.ToModel(null));
    }

    [Fact]
    public void ToModel_MapsImagesCorrectly()
    {
        // Arrange
        ContentfulPublicationTemplate contentfulPublicationTemplate = new()
        {
            HeaderImage = new Asset
            {
                File = new File { Url = "header-url" },
                Description = "Header description"
            }
        };

        // Act
        PublicationTemplate result = _publicationTemplateFactory.ToModel(contentfulPublicationTemplate);

        // Assert
        Assert.Equal("header-url", result.HeaderImage.Url);
        Assert.Equal("Header description", result.HeaderImage.Description);
    }
}