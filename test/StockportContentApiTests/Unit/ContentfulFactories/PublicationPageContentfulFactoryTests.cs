using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PublicationPageContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>> _trustedLogoFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulPublicationSection, PublicationSection>> _publicationSectionFactory = new();
    readonly ContentfulPublicationPage _contentfulPublicationPage = new ContentfulPublicationPageBuilder().Build();
    private readonly PublicationPageContentfulFactory _publicationPageFactory;

    public PublicationPageContentfulFactoryTests() =>
            _publicationPageFactory = new PublicationPageContentfulFactory(_publicationSectionFactory.Object, _trustedLogoFactory.Object);

    [Fact]
    public void ToModel_ShouldCreateAPublicationPageFromAContentfulPublicationPage()
    {
        // Act
        PublicationPage result = _publicationPageFactory.ToModel(_contentfulPublicationPage);

        // Assert
        Assert.Equal("slug", result.Slug);
        Assert.Equal("title", result.Title);
        Assert.Equal("meta description", result.MetaDescription);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_IfNullEntry()
    {
        // Act & Assert
        Assert.Null(_publicationPageFactory.ToModel(null));
    }
}