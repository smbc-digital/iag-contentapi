namespace StockportContentApiTests.Unit.Utils;

public class UrlBuilderTests
{
    private const string ENTRIES_BASE_URL = "https://test-host.com/spaces/XX/entries?access_token=XX";
    private readonly UrlBuilder _urlBuilder;

    public UrlBuilderTests() =>
        _urlBuilder = new UrlBuilder(ENTRIES_BASE_URL);

    [Fact]
    public void ShouldGetUrlForAtoZRepository()
    {
        // Act
        string result = _urlBuilder.UrlFor(type: "AtoZ", displayOnAtoZ: true);

        // Assert
        Assert.Equal($"{ENTRIES_BASE_URL}&content_type=AtoZ&fields.displayOnAZ=true", result);
    }

    [Fact]
    public void ShouldGetUrlForHomeRepository()
    {
        // Act
        string result = _urlBuilder.UrlFor(type: "home", referenceLevel: 2, slug: "slug");

        // Assert
        Assert.Equal($"{ENTRIES_BASE_URL}&content_type=home&include=2&fields.slug=slug", result);
    }

    [Fact]
    public void ShouldGetUrlForStartPageRepository()
    {
        // Act
        string result = _urlBuilder.UrlFor(type: "startPage", referenceLevel: 2, slug: "slug");

        // Assert
        Assert.Equal($"{ENTRIES_BASE_URL}&content_type=startPage&include=2&fields.slug=slug", result);
    }

    [Fact]
    public void ShouldGetUrlForNewsRepository()
    {
        // Act
        string result = _urlBuilder.UrlFor(type: "news", referenceLevel: 2, slug: "slug");

        // Assert
        Assert.Equal($"{ENTRIES_BASE_URL}&content_type=news&include=2&fields.slug=slug", result);
    }

    [Fact]
    public void ShouldGetUrlForRedirectRepository()
    {
        // Act
        string result = _urlBuilder.UrlFor(type: "redirect");

        // Assert
        Assert.Equal($"{ENTRIES_BASE_URL}&content_type=redirect", result);
    }
}