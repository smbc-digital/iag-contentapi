namespace StockportContentApiTests.Unit.ContentfulFactories;

public class FooterContentfulFactoryTests
{
    [Fact]
    public void ShouldCreateAFooterFromAContentfulReference()
    {
        // Arrange
        Mock<IContentfulFactory<ContentfulReference, SubItem>> factory = new();
        factory.Setup(_subItemFactory => _subItemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>(), EColourScheme.Orange));
       
        Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>> socialMediaFactory = new();
        socialMediaFactory.Setup(_socialMediaFactory => _socialMediaFactory.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

        ContentfulFooter ContentfulReference = new ContentfulFooterBuilder().Build();

        // Act
        Footer footer = new FooterContentfulFactory(factory.Object, socialMediaFactory.Object).ToModel(ContentfulReference);

        // Assert
        Assert.Equal(ContentfulReference.Slug, footer.Slug);
        Assert.Equal(ContentfulReference.Title, footer.Title);
    }
}