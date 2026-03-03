using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PublicationSectionContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>> _trustedLogoFactory = new();
    readonly ContentfulPublicationSection _contentfulPublicationSection = new ContentfulPublicationSectionBuilder().Build();
    private readonly PublicationSectionContentfulFactory _publicationSectionFactory;

    public PublicationSectionContentfulFactoryTests() =>
            _publicationSectionFactory = new PublicationSectionContentfulFactory(_trustedLogoFactory.Object);

    [Fact]
    public void ToModel_ShouldCreateAPublicationSectionFromAContentfulPublicationSection()
    {
        // Act
        PublicationSection result = _publicationSectionFactory.ToModel(_contentfulPublicationSection);

        // Assert
        Assert.Equal("slug", result.Slug);
        Assert.Equal("title", result.Title);
        Assert.Equal("meta description", result.MetaDescription);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_IfNullEntry()
    {
        // Act & Assert
        Assert.Null(_publicationSectionFactory.ToModel(null));
    }
}