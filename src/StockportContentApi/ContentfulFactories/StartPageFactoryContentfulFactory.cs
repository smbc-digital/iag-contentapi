using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class StartPageFactoryContentfulFactory : IContentfulFactory<ContentfulStartPage, StartPage>
    {
        private readonly DateComparer _dateComparer;
        private IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;

        public StartPageFactoryContentfulFactory(ITimeProvider timeProvider, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulReference, Crumb> crumbFactory)
        {
            _dateComparer = new DateComparer(timeProvider);
            _alertFactory = alertFactory;
            _crumbFactory = crumbFactory;
        }

        public StartPage ToModel(ContentfulStartPage entry)
        {
            var alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                     .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var breadcrumbs =
                entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                    .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var alertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                       && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                            .Select(alertInline => _alertFactory.ToModel(alertInline));

            return new StartPage(entry.Title, entry.Slug, entry.Teaser, entry.Summary, entry.UpperBody,
                entry.FormLinkLabel, entry.FormLink, entry.LowerBody, entry.BackgroundImage, entry.Icon,
                breadcrumbs, alerts, alertsInline, entry.SunriseDate, entry.SunsetDate);
        }
    }
}