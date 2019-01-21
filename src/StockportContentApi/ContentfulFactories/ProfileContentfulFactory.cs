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

        public ProfileContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory, IHttpContextAccessor httpContextAccessor, IContentfulFactory<ContentfulAlert, Alert> alertFactory)
        {
            _crumbFactory = crumbFactory;
            _httpContextAccessor = httpContextAccessor;
            _alertFactory = alertFactory;
        }

        public Profile ToModel(ContentfulProfile entry)
        {
            var breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys))
                                     .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;
            var backgroundImageUrl = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                ? entry.BackgroundImage.File.Url : string.Empty;

            return new Profile(entry.Type, entry.Title, entry.Slug, entry.LeadParagraph, entry.Teaser, entry.Quote, imageUrl,
                               entry.Body, entry.Icon, backgroundImageUrl, breadcrumbs, alerts).StripData(_httpContextAccessor);
        }
    }

    public class ProfileNewContentfulFactory : IContentfulFactory<ContentfulProfileNew, ProfileNew>
    {
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IContentfulFactory<ContentfulInformationList, InformationList> _informationListFactory;

        public ProfileNewContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory, IHttpContextAccessor httpContextAccessor, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulInformationList, InformationList> informationListFactory)
        {
            _crumbFactory = crumbFactory;
            _httpContextAccessor = httpContextAccessor;
            _alertFactory = alertFactory;
            _informationListFactory = informationListFactory;
        }

        public ProfileNew ToModel(ContentfulProfileNew entry)
        {
            var breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                                .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys))
                                     .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;

            var didYouKnowSubheading = !string.IsNullOrEmpty(entry.DidYouKnowSubheading)
                ? entry.DidYouKnowSubheading
                : "";

            var didYouKnowSection = entry.DidYouKnowSection.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                                    .Select(fact => _informationListFactory.ToModel(fact)).ToList();

            return new ProfileNew(entry.Title, entry.Slug, entry.Subtitle, entry.Quote, imageUrl,
                               entry.Body, breadcrumbs, alerts, didYouKnowSubheading, didYouKnowSection, entry.FieldOrder).StripData(_httpContextAccessor);
        }
    }
}