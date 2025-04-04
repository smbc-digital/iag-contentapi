﻿namespace StockportContentApiTests.Unit.ContentfulFactories;

public class SiteHeaderContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _mockSubItemFactory = new();

    private readonly SiteHeaderContentfulFactory _siteHeaderContentfulFactory;

    public SiteHeaderContentfulFactoryTests()
    {
        _mockSubItemFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem
            {
                Slug = "subItemSlug",
                Title = "subItemTitle",
                Teaser = "subItemTeaser",
                Type = "subItemType",
                SunriseDate = DateTime.MinValue,
                SunsetDate = DateTime.MaxValue,
                Image = "subItemImage",
                SubItems = new List<SubItem>(),
                ColourScheme = EColourScheme.Purple
            });

        _siteHeaderContentfulFactory = new SiteHeaderContentfulFactory(_mockSubItemFactory.Object);
    }

    [Fact]
    public void ToModel_ShouldCreateASiteHeaderFromAContentfulSiteHeader()
    {
        // Arrange
        ContentfulSiteHeader contentfulSiteHeader = new ContentfulSiteHeaderBuilder().Build();

        // Act
        SiteHeader siteHeader = _siteHeaderContentfulFactory.ToModel(contentfulSiteHeader);
        SubItem siteHeaderSubItem = siteHeader.Items.FirstOrDefault();

        // Assert
        Assert.Equal(contentfulSiteHeader.Title, siteHeader.Title);
        Assert.NotNull(siteHeaderSubItem);
        Assert.Equal(siteHeaderSubItem.Slug, siteHeaderSubItem.Slug);
        Assert.Equal(siteHeaderSubItem.Title, siteHeaderSubItem.Title);
        Assert.Equal(siteHeaderSubItem.Teaser, siteHeaderSubItem.Teaser);
        Assert.Equal(siteHeaderSubItem.Type, siteHeaderSubItem.Type);
        Assert.Equal(siteHeaderSubItem.Image, siteHeaderSubItem.Image);
        Assert.Equal(contentfulSiteHeader.Logo.File.Url, siteHeader.Logo);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulSiteHeader contentfulSiteHeader = new ContentfulSiteHeaderBuilder().Build();
        contentfulSiteHeader.Title = string.Empty;
        contentfulSiteHeader.Logo.File.Url = string.Empty;

        // Act
        SiteHeader siteHeader = _siteHeaderContentfulFactory.ToModel(contentfulSiteHeader);
        SubItem siteHeaderSubItem = siteHeader.Items.FirstOrDefault();

        // Assert
        Assert.Equal(contentfulSiteHeader.Title, siteHeader.Title);
        Assert.Equal(contentfulSiteHeader.Logo.File.Url, siteHeader.Logo);
    }
}
