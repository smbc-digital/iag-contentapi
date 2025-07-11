namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class ContentBlock
{
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Teaser { get; set; }
    public string Icon { get; set; }
    public EColourScheme ColourScheme { get; set; } = EColourScheme.Multi;
    public string Type { get; set; }
    public string ContentType { get; set; }
    public string Image { get; set; }
    public string MailingListId { get; set; }
    public string Body { get; set; }
    public string Link { get; set; }
    public string ButtonText { get; set; }
    public List<ContentBlock> SubItems { get; set; }
    public string Statistic { get; set; }
    public string StatisticSubHeading { get; set; }
    public string VideoTitle { get; set; }
    public string VideoToken { get; set; }
    public string VideoPlaceholderPhotoId { get; set; }
    public string AssociatedTagCategory { get; set; }
    public bool UseTag { get; set; }
    public bool IsLatest { get; set; }
    public News NewsArticle { get; set; }
    public Profile Profile { get; set; }
    public List<Event> Events { get; set; }
    public List<News> News { get; set; }
    public string ScreenReader { get; set; }
    public string AccountName { get; set; }
}