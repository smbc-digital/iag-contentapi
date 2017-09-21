using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class StartPageFactoryContentfulFactory : IContentfulFactory<ContentfulStartPage, StartPage>
    {
        private readonly DateComparer _dateComparer;
        private IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StartPageFactoryContentfulFactory(ITimeProvider timeProvider, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulReference, Crumb> crumbFactory, IHttpContextAccessor httpContextAccessor)
        {
            _dateComparer = new DateComparer(timeProvider);
            _alertFactory = alertFactory;
            _crumbFactory = crumbFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public StartPage ToModel(ContentfulStartPage entry)
        {
            var alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                     .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var breadcrumbs =
                entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                    .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            return new StartPage(entry.Title, entry.Slug, entry.Teaser, entry.Summary, entry.UpperBody,
                entry.FormLinkLabel, entry.FormLink, entry.LowerBody, entry.BackgroundImage, entry.Icon,
                breadcrumbs, alerts).StripData(_httpContextAccessor);
        }
    }
}