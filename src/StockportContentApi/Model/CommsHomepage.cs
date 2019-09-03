using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class CommsHomepage
    {
        public string Title { get; }

        public CallToActionBanner CallToActionBanner { get; }

        public string LatestNewsHeader { get; }

        public string TwitterFeedHeader { get; }

        public string InstagramFeedTitle { get; }

        public string InstagramLink { get; }

        public string FacebookFeedTitle { get; }

        public IEnumerable<BasicLink> UsefullLinks { get; }

        public Event WhatsOnInStockportEvent { get; }

        public CommsHomepage(
            string title,
            string latestNewsHeader,
            string twitterFeedHeader,
            string instagramFeedTitle,
            string instagramLink,
            string facebookFeedTitle,
            IEnumerable<BasicLink> usefullLinks,
            Event whatsOnInStockportEvent,
            CallToActionBanner callToActionBanner
            )
        {
            Title = title;
            LatestNewsHeader = latestNewsHeader;
            TwitterFeedHeader = twitterFeedHeader;
            InstagramFeedTitle = instagramFeedTitle;
            InstagramLink = instagramLink;
            FacebookFeedTitle = facebookFeedTitle;
            UsefullLinks = usefullLinks;
            WhatsOnInStockportEvent = whatsOnInStockportEvent;
            CallToActionBanner = callToActionBanner;
        }
    }
}
