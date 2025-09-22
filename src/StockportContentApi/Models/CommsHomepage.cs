namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class CommsHomepage(string title,
                        string metaDescription,
                        string latestNewsHeader,
                        string twitterFeedHeader,
                        string instagramFeedTitle,
                        string instagramLink,
                        string facebookFeedTitle,
                        IEnumerable<BasicLink> usefulLinks,
                        Event whatsOnInStockportEvent,
                        CallToActionBanner callToActionBanner,
                        string emailAlertsTopicId)
{
    public string Title { get; } = title;
    public string MetaDescription { get; set; } = metaDescription;
    public CallToActionBanner CallToActionBanner { get; } = callToActionBanner;
    public string LatestNewsHeader { get; } = latestNewsHeader;
    public string TwitterFeedHeader { get; } = twitterFeedHeader;
    public string InstagramFeedTitle { get; } = instagramFeedTitle;
    public string InstagramLink { get; } = instagramLink;
    public string FacebookFeedTitle { get; } = facebookFeedTitle;
    public IEnumerable<BasicLink> UsefulLinks { get; } = usefulLinks;
    public Event WhatsOnInStockportEvent { get; } = whatsOnInStockportEvent;
    public string EmailAlertsTopicId { get; set; } = emailAlertsTopicId;
}