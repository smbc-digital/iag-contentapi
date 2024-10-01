namespace StockportContentApiTests.Unit.ContentfulFactories;

public class DirectoryEntryContentfulFactoryTests
{
    [Fact]
    public void ShouldCreateADirectorEntryFromAContentfulReference()
    {
        // Arrange
        ContentfulDirectoryEntry contentfulReference =
            new DirectoryEntryBuilder()
            .WithSlug("test-directory-entry")
            .WithTitle("Test Directory Entry")
            .WithProvider("Test Directory")
            .WithTeaser("Test entry teaser text")
            .WithDescription("Test entry body text")
            .WithMetaDescription("Test entry meta description")
            .WithPhoneNumber("01625 444 4444")
            .WithEmail("test@email.com")
            .WithWebsite("https://www.test.co.uk")
            .WithTwitter("@test")
            .WithFacebook("TestFacebook")
            .WithAddress("Town Hall, Stockport, SK1 3XE")
            .WithMapPosition(new MapPosition
            {
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
            .WithBranding(new List<ContentfulGroupBranding>() {
                new() {
                    File = new Asset(),
                    Sys = new SystemProperties(),
                    Text = "test",
                    Title = "test",
                    Url = "test"
                }
            })
            .WithDirectories(new List<ContentfulDirectory>() {
                new DirectoryBuilder()
                .WithSlug("test-alert")
                .WithTitle("Test Alert")
                .WithBody("Test Alert Body")
                .Build()
            })
            .Build();

        // Act
        DirectoryEntry directoryEntry = new DirectoryEntryContentfulFactory(new AlertContentfulFactory(), new GroupBrandingContentfulFactory(), new TimeProvider()).ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Slug, directoryEntry.Slug);
        Assert.Equal(contentfulReference.Name, directoryEntry.Name);
        Assert.Equal(contentfulReference.Provider, directoryEntry.Provider);
        Assert.Equal(contentfulReference.Teaser, directoryEntry.Teaser);
        Assert.Equal(contentfulReference.Description, directoryEntry.Description);
        Assert.Equal(contentfulReference.MetaDescription, directoryEntry.MetaDescription);
        Assert.Equal(contentfulReference.PhoneNumber, directoryEntry.PhoneNumber);
        Assert.Equal(contentfulReference.Email, directoryEntry.Email);
        Assert.Equal(contentfulReference.Website, directoryEntry.Website);
        Assert.Equal(contentfulReference.Twitter, directoryEntry.Twitter);
        Assert.Equal(contentfulReference.Facebook, directoryEntry.Facebook);
        Assert.Equal(contentfulReference.Address, directoryEntry.Address);
        Assert.Equal(contentfulReference.MapPosition.Lat, directoryEntry.MapPosition.Lat);
        Assert.Equal(contentfulReference.MapPosition.Lon, directoryEntry.MapPosition.Lon);
        Assert.Equal(2, directoryEntry.Themes.Count());
        Assert.Single(directoryEntry.Alerts);
        Assert.Single(directoryEntry.Branding);
    }
}