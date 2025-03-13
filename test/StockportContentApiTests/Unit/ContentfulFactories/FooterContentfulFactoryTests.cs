namespace StockportContentApiTests.Unit.ContentfulFactories;

public class FooterContentfulFactoryTests
{
    private FooterContentfulFactory _footerFactory;
    private Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>> _socialMediaFactory = new();

    public FooterContentfulFactoryTests()
    {
        _subitemFactory
            .Setup(_subItemFactory => _subItemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem("slug",
                                "title",
                                "teaser",
                                "teaser image",
                                "icon",
                                "type",
                                DateTime.MinValue,
                                DateTime.MaxValue,
                                "image",
                                new List<SubItem>(),
                                EColourScheme.Pink));
        
        _socialMediaFactory
            .Setup(_socialMediaFactory => _socialMediaFactory.ToModel(It.IsAny<ContentfulSocialMediaLink>()))
            .Returns(new SocialMediaLink("sm-link-title",
                                        "sm-link-slug",
                                        "sm-link-icon",
                                        "https://link.url",
                                        "sm-link-accountName",
                                        "sm-link-screenReader"));

        _footerFactory = new(_subitemFactory.Object, _socialMediaFactory.Object);
    }

    [Fact]
    public void ToModel_ShouldCreateFooterFromAContentfulReference()
    {
        // Arrange
        ContentfulFooter ContentfulReference = new ContentfulFooterBuilder().Build();

        // Act
        Footer footer = _footerFactory.ToModel(ContentfulReference);

        // Assert
        Assert.Equal(ContentfulReference.Slug, footer.Slug);
        Assert.Equal(ContentfulReference.Title, footer.Title);
    }
}