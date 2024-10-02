namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ContentBlockContentfulFactoryTests
{
    private readonly Mock<ITimeProvider> _timeProviderMock = new();
    private readonly ContentBlockContentfulFactory _factory;

    public ContentBlockContentfulFactoryTests() => _factory = new ContentBlockContentfulFactory(_timeProviderMock.Object);

    [Fact]
    public void ToModel_ShouldReturnContentBlock_WithCorrectFields()
    {
        // Arrange
        ContentfulReference contentfulReference = new()
        {
            Slug = "test-slug",
            Title = "Test Title",
            Teaser = "Test Teaser",
            Icon = "test-icon",
            ContentType = "CallToAction",
            Image = new Asset
            {
                File = new File
                {
                    Url = "http://test-url.com"
                },
                SystemProperties = new() { Id = "image-id" }
            },
            SubItems = new List<ContentfulReference>
            {
                new() { ContentType = "callToActionBanner", Slug = "sub-item-1", Title = "Sub Item 1", Sys = new SystemProperties() { ContentType = new ContentType { SystemProperties = new SystemProperties() { Id = "callToActionBanner" } } } }
            },
            Link = "http://link.com",
            ButtonText = "Click Here",
            Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
        };

        // Act
        ContentBlock result = _factory.ToModel(contentfulReference);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-slug", result.Slug);
        Assert.Equal("Test Title", result.Title);
        Assert.Equal("Test Teaser", result.Teaser);
        Assert.Equal("test-icon", result.Icon);
        Assert.Equal("CallToAction", result.ContentType);
        Assert.Equal("http://test-url.com", result.Image);
        Assert.Single(result.SubItems);
        Assert.Equal("Sub Item 1", result.SubItems[0].Title);
        Assert.Equal("http://link.com", result.Link);
        Assert.Equal("Click Here", result.ButtonText);
    }

    [Fact]
    public void ToModel_ShouldExcludeInvalidSubItems_WhenSubItemNotSuitableForParentContentType()
    {
        // Arrange
        ContentfulReference contentfulReference = new()
        {
            ContentType = "CallToAction",
            SubItems = new List<ContentfulReference>
            {
                new() { ContentType = "invalidContentType", Slug = "sub-item-invalid", Sys = new SystemProperties() { ContentType = new ContentType { SystemProperties = new SystemProperties() { Id = "id" } } } },
                new() { ContentType = "callToActionBanner", Slug = "sub-item-valid", Sys = new SystemProperties() { ContentType = new ContentType { SystemProperties = new SystemProperties() { Id = "callToActionBanner" } } } }
            },
            Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
        };

        // Act  
        ContentBlock result = _factory.ToModel(contentfulReference);

        // Assert
        Assert.Single(result.SubItems);
        Assert.Equal("sub-item-valid", result.SubItems[0].Slug);
    }

    [Fact]
    public void ToModel_ShouldReturnEmptySubItems_WhenNoValidSubItems()
    {
        // Arrange
        ContentfulReference contentfulReference = new()
        {
            ContentType = "CallToAction",
            SubItems = new List<ContentfulReference>
            {
                new() { ContentType = "invalidContentType", Slug = "sub-item-invalid", Sys = new SystemProperties() { ContentType = new ContentType { SystemProperties = new SystemProperties() { Id = "id" } } } },
            },
            Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "callToAction" } } }
        };

        // Act
        ContentBlock result = _factory.ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.SubItems);
    }

    [Theory]
    [InlineData("CallToAction", "callToActionBanner", true)]
    [InlineData("EventCards", "events", true)]
    [InlineData("FindOutMoreCards", "article", true)]
    [InlineData("FindOutMoreCards", "invalidContentType", false)]
    [InlineData("NewsBanner", "news", false)]
    [InlineData("ProfileBanner", "profile", true)]
    [InlineData("ProfileBanner", "informationList", false)]
    [InlineData("SocialMedia", "socialMediaLink", true)]
    [InlineData("SocialMedia", "start-page", false)]
    [InlineData("SubscriptionBanner", "directory", false)]
    [InlineData("TriviaList", "callToActionBanner", false)]
    [InlineData("TriviaList", "informationList", true)]
    [InlineData("Video", "topic", false)]
    public void IsSubItemSuitableForContentType_ShouldReturnCorrectResult(string parentContentType, string subItemContentType, bool expectedResult)
    {
        // Act
        bool result = ContentBlockContentfulFactory.IsSubItemSuitableForContentType(parentContentType, subItemContentType);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}