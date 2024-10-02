using Directory = StockportContentApi.Models.Directory;
using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class DirectoryContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory =
        new CallToActionBannerContentfulFactory();
    private readonly IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> _directoryEntryFactory =
        new DirectoryEntryContentfulFactory(new AlertContentfulFactory(), new GroupBrandingContentfulFactory(), new TimeProvider());
    private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory =
        new EventBannerContentfulFactory();
    private readonly IContentfulFactory<ContentfulExternalLink, ExternalLink> _externalLinkFactory = new ExternalLinkContentfulFactory();
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory;
    private readonly ITimeProvider _timeProvider = new TimeProvider();

    public DirectoryContentfulFactoryTests()
    {
        _subItemFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
    }

    [Fact]
    public void ToModel_ShouldCreateADirectoryFromAContentfulReference()
    {
        // Arrange
        ContentfulDirectory ContentfulReference =
            new DirectoryBuilder()
                .WithSlug("test-directory")
                .WithTitle("Test-Directory")
                .WithTeaser("Test teaser text")
                .WithBody("Test Body Text")
                .WithMetaDescription("Test Meta Description")
                .WithId("XXX123456")
                .WithBackgroundImageUrl("//TESTIMAGE.JPG")
                .WithCallToAction(new ContentfulCallToActionBannerBuilder().Build())
                .WithAlert(new ContentfulAlertBuilder()
                    .WithSlug("test-alert")
                    .WithTitle("Test Alert")
                    .WithSubHeading("Test Sub Heading")
                    .WithBody("Test Alert Body")
                    .WithSunriseDate(DateTime.Now.AddDays(-1))
                    .WithSunsetDate(DateTime.Now.AddDays(1))
                    .WithSeverity("Warning")
                    .Build())
                .WithRelatedContent(new()
                {
                    new ContentfulReferenceBuilder().Build()
                })
                .WithExternalLinks(new()
                {
                    new()
                })
                .WithPinnedEntries(new()
                {
                    new DirectoryEntryBuilder().Build()
                })
                .WithSubItems(new()
                {
                    new ContentfulReferenceBuilder().Build()
                })
                .Build();

        SubItem subItem = new("slug1", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image",
            new(), EColourScheme.Teal);
        _subItemFactory.Setup(_ => _.ToModel(ContentfulReference.SubItems.First())).Returns(subItem);

        // Act
        Directory directory = new DirectoryContentfulFactory(_subItemFactory.Object, _externalLinkFactory,
                _alertFactory.Object, _callToActionFactory, _timeProvider, _eventBannerFactory, _directoryEntryFactory)
            .ToModel(ContentfulReference);

        // Assert
        Assert.Equal(ContentfulReference.Slug, directory.Slug);
        Assert.Equal(ContentfulReference.Title, directory.Title);
        Assert.Equal(ContentfulReference.Teaser, directory.Teaser);
        Assert.Equal(ContentfulReference.Body, directory.Body);
        Assert.Equal(ContentfulReference.MetaDescription, directory.MetaDescription);
        Assert.Equal(ContentfulReference.Sys.Id, directory.ContentfulId);
        Assert.NotNull(directory.CallToAction);
        Assert.Equal(ContentfulReference.BackgroundImage.File.Url, directory.BackgroundImage);
        Assert.Equal(2, directory.Alerts.Count());
        Assert.Single(directory.PinnedEntries);
        Assert.Equal(2, directory.SubItems.Count());
    }

    [Fact]
    public void ToModel_ShouldReturnNull_IfNullEntry()
    {
        // Act
        Directory directory = new DirectoryContentfulFactory(_subItemFactory.Object, _externalLinkFactory,
                _alertFactory.Object, _callToActionFactory, _timeProvider, _eventBannerFactory, _directoryEntryFactory)
            .ToModel(null);

        // Assert
        Assert.Null(directory);
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
        Directory result = new DirectoryContentfulFactory(_subItemFactory.Object, _externalLinkFactory,
                _alertFactory.Object, _callToActionFactory, _timeProvider, _eventBannerFactory, _directoryEntryFactory)
            .ToModel(directory);

        // Assert
        Assert.Empty(result.SubItems);
        Assert.Empty(result.Alerts);
        _subItemFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
        _alertFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulAlert>()), Times.Never);
    }
}