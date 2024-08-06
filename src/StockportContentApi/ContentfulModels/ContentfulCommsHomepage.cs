namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulCommsHomepage : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public ContentfulCallToActionBanner CallToActionBanner { get; set; } = null;
    public string LatestNewsHeader { get; set; } = string.Empty;
    public string TwitterFeedHeader { get; set; } = string.Empty;
    public string InstagramFeedTitle { get; set; } = string.Empty;
    public string InstagramLink { get; set; } = string.Empty;
    public string FacebookFeedTitle { get; set; } = string.Empty;
    public List<string> UsefulLinksText { get; set; }
    public List<string> UsefulLinksURL { get; set; }
    public ContentfulEvent WhatsOnInStockportEvent { get; set; } = null;
    public SystemProperties Sys { get; set; } = new SystemProperties();
    public string EmailAlertsTopicId { get; set; }
}