using System.Collections.Generic;
using System.Linq;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class EventBannerListFactory : IBuildContentTypeFromReference<EventBanner>
    {
        private readonly IFactory<EventBanner> _eventBannerFactory;

        public EventBannerListFactory(IFactory<EventBanner> eventBannerFactory)
        {
            _eventBannerFactory = eventBannerFactory;
        }

        public EventBanner BuildFromReference(dynamic reference,
            IContentfulIncludes contentfulResponse)
        {
            if (reference == null) return new NullEventBanner();;
            var eventBannerEntry = contentfulResponse.GetEntryFor(reference);

            if (eventBannerEntry == null) return new NullEventBanner();
            return _eventBannerFactory.Build(eventBannerEntry, contentfulResponse);
        }
    }
}
