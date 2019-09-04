using System.Collections.Generic;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class CommsContentfulFactory : IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>
    {
        private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory;
        private readonly IContentfulFactory<ContentfulEvent, Event> _eventFactory;
        private readonly IContentfulFactory<IEnumerable<ContentfulBasicLink>, IEnumerable<BasicLink>> _basicLinkFactory;

        public CommsContentfulFactory(
            IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory,
            IContentfulFactory<ContentfulEvent, Event> eventFactory,
            IContentfulFactory<IEnumerable<ContentfulBasicLink>, IEnumerable<BasicLink>> basicLinkFactory)
        {
            _callToActionFactory = callToActionFactory;
            _eventFactory = eventFactory;
            _basicLinkFactory = basicLinkFactory;
        }

        public CommsHomepage ToModel(ContentfulCommsHomepage model)
        {

            var callToActionBanner = model.CallToActionBanner != null 
                ? _callToActionFactory.ToModel(model.CallToActionBanner)
                : null;
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
                callToActionBanner
                );
        }
    }
}
