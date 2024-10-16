using Directory = StockportContentApi.Models.Directory;
using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class DirectoryContentfulFactoryTests
{
    private readonly DirectoryContentfulFactory _factory;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>> _directoryEntryFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>> _eventBannerFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulExternalLink, ExternalLink>> _externalLinkFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory = new();
    private readonly ITimeProvider _timeProvider = new TimeProvider();

    public DirectoryContentfulFactoryTests() =>
        _factory = new(_subItemFactory.Object,
                    _externalLinkFactory.Object,
                    _alertFactory.Object,
                    _callToActionFactory.Object,
                    _timeProvider,
                    _eventBannerFactory.Object,
                    _directoryEntryFactory.Object);

    [Fact]
    public void ToModel_ShouldCreateADirectoryFromAContentfulReference()
    {
        // Arrange
        ContentfulDirectory directory = new DirectoryBuilder().Build();

        SubItem subItem = new("slug1", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new(), EColourScheme.Teal);
        _subItemFactory
            .Setup(_ => _.ToModel(directory.SubItems.First()))
            .Returns(subItem);

        // Act
        Directory result = _factory.ToModel(directory);

        // Assert
        Assert.Equal(directory.Slug, result.Slug);
        Assert.Equal(directory.Title, result.Title);
        Assert.Equal(directory.Teaser, result.Teaser);
        Assert.Equal(directory.Body, result.Body);
        Assert.Equal(directory.MetaDescription, result.MetaDescription);
        Assert.Equal(directory.Sys.Id, result.ContentfulId);
        Assert.NotNull(directory.CallToAction);
        Assert.Equal(directory.BackgroundImage.File.Url, result.BackgroundImage);
        Assert.Single(result.Alerts);
        Assert.Single(result.PinnedEntries);
        Assert.Equal(2, result.SubItems.Count());
    }

    [Fact]
    public void ToModel_ShouldReturnNull_IfNullEntry()
    {
        // Act & Assert
        Assert.Null(_factory.ToModel(null));
    }

    [Fact]
    public void ToModel_ShouldNotAddSubItems_Alerts_SubDirectories_RelatedContent_IfTheyAreLinks()
    {
        // Arrange
        ContentfulDirectory directory = new DirectoryBuilder().Build();
        directory.SubItems.First().Sys.LinkType = "Link";
        directory.Alerts.First().Sys.LinkType = "Link";
        directory.SubDirectories.First().Sys.LinkType = "Link";
        directory.RelatedContent.First().Sys.LinkType = "Link";

        // Act
        Directory result = _factory.ToModel(directory);

        // Assert
        Assert.Empty(result.SubItems);
        Assert.Empty(result.Alerts);
        _subItemFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
        _alertFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulAlert>()), Times.Never);
    }
}