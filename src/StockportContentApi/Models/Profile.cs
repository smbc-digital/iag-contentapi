namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Profile
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Subtitle { get; set; }
    public string Teaser { get; set; }
    public List<InlineQuote> InlineQuotes { get; set; }
    public MediaAsset Image { get; set; }
    public string ImageCaption { get; set; }
    public string Body { get; set; }
    public IEnumerable<Crumb> Breadcrumbs { get; set; }
    public List<Alert> InlineAlerts { get; set; }
    public List<Alert> Alerts { get; set; }
    public string TriviaSubheading { get; set; }
    public List<Trivia> TriviaSection { get; set; }
    public string Author { get; set; }
    public string Subject { get; set; }
    public EventBanner EventsBanner { get; set; }
    public EColourScheme Colour { get; set; } = EColourScheme.Teal;
    public Topic ParentTopic { get; set; }

    public Profile()
    { }
}