using System.Collections.Generic;
using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class ProfileContentfulFactory : IContentfulFactory<ContentfulProfile, Profile>
    {
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IContentfulFactory<ContentfulInformationList, InformationList> _informationListFactory;

        public ProfileContentfulFactory(
            IContentfulFactory<ContentfulReference, Crumb> crumbFactory, 
            IHttpContextAccessor httpContextAccessor, 
            IContentfulFactory<ContentfulAlert, Alert> alertFactory, 
            IContentfulFactory<ContentfulInformationList, InformationList> informationListFactory)
        {
            _crumbFactory = crumbFactory;
            _httpContextAccessor = httpContextAccessor;
            _alertFactory = alertFactory;
            _informationListFactory = informationListFactory;
        }

        public Profile ToModel(ContentfulProfile entry)
        {
            var breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                                .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys))
                                     .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;

            var triviaSubheading = !string.IsNullOrEmpty(entry.TriviaSubheading)
                ? entry.TriviaSubheading
                : "";

            var triviaSection = entry.TriviaSection.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                                    .Select(fact => _informationListFactory.ToModel(fact)).ToList();

            return new Profile(entry.Title, entry.Slug, entry.Subtitle, entry.Quote, imageUrl,
                               entry.Body, breadcrumbs, alerts, triviaSubheading, triviaSection, entry.FieldOrder, entry.Author, entry.Subject).StripData(_httpContextAccessor);
        }
    }
}