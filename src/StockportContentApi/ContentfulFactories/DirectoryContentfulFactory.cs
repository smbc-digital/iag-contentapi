namespace StockportContentApi.ContentfulFactories
{
    public class DirectoryContentfulFactory : IContentfulFactory<ContentfulDirectory, Directory>
    {
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory;
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory;
        
        public DirectoryContentfulFactory(IContentfulFactory<ContentfulAlert, Alert> alertFactory, 
            IContentfulFactory<ContentfulCallToActionBanner,
            CallToActionBanner> callToActionFactory,
            ITimeProvider timeProvider,
            IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory)
        {
            _alertFactory = alertFactory;
            _dateComparer = new DateComparer(timeProvider);
            _callToActionFactory = callToActionFactory;
            _eventBannerFactory = eventBannerFactory;
        }

        public Directory ToModel(ContentfulDirectory entry)
        {
            if (entry is null)
                return null;

            var eventBanner = ContentfulHelpers.EntryIsNotALink(entry.EventBanner.Sys)
                            ? _eventBannerFactory.ToModel(entry.EventBanner) : new NullEventBanner();

            return new()
            {
                Slug = entry.Slug,
                Title = entry.Title,
                Body = entry.Body,
                Teaser = entry.Teaser,
                MetaDescription = entry.MetaDescription,
                Alerts = entry.Alerts?
                            .Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                .Select(alert => _alertFactory.ToModel(alert)),
                CallToAction = entry.CallToAction is null ? null : _callToActionFactory.ToModel(entry.CallToAction),
                BackgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                                ? entry.BackgroundImage.File.Url : string.Empty,
                ContentfulId = entry.Sys.Id,
                ColourScheme = entry.ColourScheme,
                Icon = entry.Icon,
                EventBanner = eventBanner,
                
            };
        }
    }
}