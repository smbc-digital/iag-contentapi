using System.Collections.Generic;
using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class CommsContentfulFactory : IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>
    {
        private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory;
        private readonly IContentfulFactory<ContentfulEvent, Event> _eventFactory;

        public CommsContentfulFactory(
            IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory,
            IContentfulFactory<ContentfulEvent, Event> eventFactory)
        {
            _callToActionFactory = callToActionFactory;
            _eventFactory = eventFactory;
        }

        public CommsHomepage ToModel(ContentfulCommsHomepage model)
        {
            List<BasicLink> usefulLinks = new();
            if (model.UsefulLinksText is not null && model.UsefulLinksURL is not null && 
                model.UsefulLinksText.Count.Equals(model.UsefulLinksURL.Count))
            {
                usefulLinks = model.UsefulLinksText.Zip(model.UsefulLinksURL, (text, url) => new BasicLink(url, text)).ToList();
            }

            return new(
                model.Title,
                model.MetaDescription,
                model.LatestNewsHeader,
                model.TwitterFeedHeader,
                model.InstagramFeedTitle,
                model.InstagramLink,
                model.FacebookFeedTitle,
                usefulLinks,
                _eventFactory.ToModel(model.WhatsOnInStockportEvent),
                _callToActionFactory.ToModel(model.CallToActionBanner),
                model.EmailAlertsTopicId
            );
        }
    }
}
