using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class CommsHomepage
    {
        public string Title { get; }

        public string MetaDescription { get; set; }

        public CallToActionBanner CallToActionBanner { get; }

        public string LatestNewsHeader { get; }

        public string TwitterFeedHeader { get; }

        public string InstagramFeedTitle { get; }

        public string InstagramLink { get; }

        public string FacebookFeedTitle { get; }

        public IEnumerable<BasicLink> UsefullLinks { get; }

        public Event WhatsOnInStockportEvent { get; }

        public string EmailAlertsTopicId { get; set; }

        public CommsHomepage(
            string title,
            string metaDescription,
            string latestNewsHeader,
            string twitterFeedHeader,
            string instagramFeedTitle,
            string instagramLink,
            string facebookFeedTitle,
            IEnumerable<BasicLink> usefullLinks,
            Event whatsOnInStockportEvent,
            CallToActionBanner callToActionBanner,
            string emailAlertsTopicId
            )
        {
            Title = title;
            MetaDescription = metaDescription;
            LatestNewsHeader = latestNewsHeader;
            TwitterFeedHeader = twitterFeedHeader;
            InstagramFeedTitle = instagramFeedTitle;
            InstagramLink = instagramLink;
            FacebookFeedTitle = facebookFeedTitle;
            UsefullLinks = usefullLinks;
            WhatsOnInStockportEvent = whatsOnInStockportEvent;
            CallToActionBanner = callToActionBanner;
            EmailAlertsTopicId = emailAlertsTopicId;
        }
    }
}
