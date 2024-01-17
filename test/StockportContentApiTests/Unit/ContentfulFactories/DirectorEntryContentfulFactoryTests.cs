namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class DirectoryEntryContentfulFactoryTests
    {       
        [Fact]
        public void ShouldCreateADirectorEntryFromAContentfulReference()
        {
            var contentfulReference =
                new DirectoryEntryBuilder()
                .WithSlug("test-directory-entry")
                .WithTitle("Test Directory Entry")
                .WithTeaser("Test entry teaser text")
                .WithDescription("Test entry body text")
                .WithMetaDescription("Test entry meta description")
                .WithPhoneNumber("01625 444 4444")
                .WithEmail("test@email.com")
                .WithWebsite("https://www.test.co.uk")
                .WithTwitter("@test")
                .WithFacebook("TestFacebook")
                .WithAddress("Town Hall, Stockport, SK1 3XE")
                .WithMapPosition(new MapPosition { 
                    Lat = 53.393310,
                    Lon = -2.126633
                })
                .WithFilter("test-filter", "Test Filter", "Test Filter Display", "test-theme")
                .WithFilter("test-filter A", "Test Filter A", "Test Filter A Display", "test-theme")
                .WithFilter("test-filter2", "Test Filter 2", "Test Filter 2 Display", "test-theme2")
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

            var directoryEntry = new DirectoryEntryContentfulFactory(new AlertContentfulFactory(), new DirectoryContentfulFactory(new AlertContentfulFactory(), new CallToActionBannerContentfulFactory(), new TimeProvider()), new GroupBrandingContentfulFactory(), new TimeProvider()).ToModel(contentfulReference);

            directoryEntry.Slug.Should().Be(contentfulReference.Slug);
            directoryEntry.Name.Should().Be(contentfulReference.Name);
            directoryEntry.Teaser.Should().Be(contentfulReference.Teaser);
            directoryEntry.Description.Should().Be(contentfulReference.Description);
            directoryEntry.MetaDescription.Should().Be(contentfulReference.MetaDescription);
            directoryEntry.PhoneNumber.Should().Be(contentfulReference.PhoneNumber);
            directoryEntry.Email.Should().Be(contentfulReference.Email);
            directoryEntry.Website.Should().Be(contentfulReference.Website);
            directoryEntry.Twitter.Should().Be(contentfulReference.Twitter);
            directoryEntry.Facebook.Should().Be(contentfulReference.Facebook);
            directoryEntry.Address.Should().Be(contentfulReference.Address);
            directoryEntry.MapPosition.Lat.Should().Be(contentfulReference.MapPosition.Lat);
            directoryEntry.MapPosition.Lon.Should().Be(contentfulReference.MapPosition.Lon);
            directoryEntry.Themes.Count().Should().Be(2);
            directoryEntry.Alerts.Count().Should().Be(1);
        }
    }
}
