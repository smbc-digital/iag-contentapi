namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class DirectoryContentfulFactoryTests
    {
        private IContentfulFactory<ContentfulAlert, Alert> _alertFactory = new AlertContentfulFactory() ;
        private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory = new CallToActionBannerContentfulFactory();
        private readonly ITimeProvider _timeProvider = new TimeProvider();

        [Fact]
        public void ShouldCreateADirectoryFromAContentfulReference()
        {
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
                .Build();
                

            var directory = new DirectoryContentfulFactory(_alertFactory, _callToActionFactory, _timeProvider).ToModel(ContentfulReference);

            directory.Slug.Should().Be(ContentfulReference.Slug);
            directory.Title.Should().Be(ContentfulReference.Title);
            directory.Teaser.Should().Be(ContentfulReference.Teaser);
            directory.Body.Should().Be(ContentfulReference.Body);
            directory.MetaDescription.Should().Be(ContentfulReference.MetaDescription);
            directory.ContentfulId.Should().Be(ContentfulReference.Sys.Id);
            directory.CallToAction.Should().NotBeNull();
            directory.BackgroundImage.Should().Be(ContentfulReference.BackgroundImage.File.Url);
            directory.Alerts.Should().NotBeNull();
            directory.Alerts.Count().Should().Be(1);
        }
    }
}
