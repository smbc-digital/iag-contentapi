namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class DirectoryContentfulFactoryTests
    {
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = new AlertContentfulFactory();
        private readonly IContentfulFactory<ContentfulExternalLink, ExternalLink> _externalLinkFactory = new ExternalLinkContentfulFactory();
        private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory = new SubItemContentfulFactory(new TimeProvider());
        private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory = new CallToActionBannerContentfulFactory();
        private readonly ITimeProvider _timeProvider = new TimeProvider();
        private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory = new EventBannerContentfulFactory();
        private readonly IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> _directoryEntryFactory = new DirectoryEntryContentfulFactory(new AlertContentfulFactory(), new GroupBrandingContentfulFactory(), new TimeProvider());

        [Fact]
        public void ShouldCreateADirectoryFromAContentfulReference()
        {
            // Arrange
            var ContentfulReference =
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
                .WithRelatedContent(new List<ContentfulReference>(){
                    new ContentfulReferenceBuilder().Build()
                })
                .WithExternalLinks(new List<ContentfulExternalLink>(){
                    new()
                })
                .WithPinnedEntries(new List<ContentfulDirectoryEntry>() {
                    new DirectoryEntryBuilder().Build()
                })
                .Build();
                
            // Act
            var directory = new DirectoryContentfulFactory(_subItemFactory, _externalLinkFactory, _alertFactory, _callToActionFactory, _timeProvider, _eventBannerFactory, _directoryEntryFactory).ToModel(ContentfulReference);

            // Assert
            Assert.Equal(ContentfulReference.Slug, directory.Slug);
            Assert.Equal(ContentfulReference.Title, directory.Title);
            Assert.Equal(ContentfulReference.Teaser, directory.Teaser);
            Assert.Equal(ContentfulReference.Body, directory.Body);
            Assert.Equal(ContentfulReference.MetaDescription, directory.MetaDescription);
            Assert.Equal(ContentfulReference.Sys.Id, directory.ContentfulId);
            Assert.NotNull(directory.CallToAction);
            Assert.Equal(ContentfulReference.BackgroundImage.File.Url, directory.BackgroundImage);
            Assert.NotNull(directory.Alerts);
            Assert.NotNull(directory.PinnedEntries);
        }
    }
}
