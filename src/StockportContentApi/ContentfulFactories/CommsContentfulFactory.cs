using System.Collections.Generic;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class CommsContentfulFactory : IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>
    {
        private readonly IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner> _spotlightBannerFactory;
        private readonly IContentfulFactory<ContentfulEvent, Event> _eventFactory;
        private readonly IContentfulFactory<IEnumerable<ContentfulBasicLink>, IEnumerable<BasicLink>> _basicLinkFactory;

        public CommsContentfulFactory(
            IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner> spotlightBannerFactory, 
            IContentfulFactory<ContentfulEvent, Event> eventFactory,
            IContentfulFactory<IEnumerable<ContentfulBasicLink>, IEnumerable<BasicLink>> basicLinkFactory)
        {
            _spotlightBannerFactory = spotlightBannerFactory;
            _eventFactory = eventFactory;
            _basicLinkFactory = basicLinkFactory;
        }

        public CommsHomepage ToModel(ContentfulCommsHomepage model)
        {

            var spotlightBanner = _spotlightBannerFactory.ToModel(model.SpotlightBanner);
            var displayEvent = _eventFactory.ToModel(model.WhatsOnInStockportEvent);
            var basicLinks = _basicLinkFactory.ToModel(model.UsefullLinks);

            return new CommsHomepage(
                model.Title,
                model.LatestNewsHeader,
                model.TwitterFeedHeader,
                model.InstagramFeedTitle,
                model.InstagramLink,
                model.FacebookFeedTitle,
                basicLinks,
                displayEvent,
                spotlightBanner
                );
        }
    }
}
