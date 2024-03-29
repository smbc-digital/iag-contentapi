﻿namespace StockportContentApiTests.Unit.ContentfulFactories;

public class FooterContentfulFactoryTest
{
    [Fact]
    public void ShouldCreateAFooterFromAContentfulReference()
    {
        var factory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
        factory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));
        var socialMediaFactory = new Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>();

        socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

        var ContentfulReference =
            new ContentfulFooterBuilder().Build();

        var footer = new FooterContentfulFactory(factory.Object, socialMediaFactory.Object).ToModel(ContentfulReference);

        footer.Slug.Should().Be(ContentfulReference.Slug);
        footer.Title.Should().Be(ContentfulReference.Title);
    }
}
