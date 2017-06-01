using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ShowcaseContentfulFactory : IContentfulFactory<ContentfulShowcase, Showcase>
    {
        private readonly IContentfulFactory<Entry<ContentfulSubItem>, SubItem> _subitemFactory;
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;
        private readonly IContentfulFactory<ContentfulConsultation, Consultation> _consultationFactory;
        private readonly IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> _socialMediaFactory;
        private readonly DateComparer _dateComparer;
        
        public ShowcaseContentfulFactory(IContentfulFactory<Entry<ContentfulSubItem>, SubItem> subitemFactory, IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory, ITimeProvider timeProvider, IContentfulFactory<ContentfulConsultation, Consultation> consultationFactory, IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink> socialMediaFactory)
        {
            _subitemFactory = subitemFactory;
            _crumbFactory = crumbFactory;
            _consultationFactory = consultationFactory;
            _socialMediaFactory = socialMediaFactory;
            _dateComparer = new DateComparer(timeProvider);
        }
        
        public Showcase ToModel(ContentfulShowcase entry)
        {
            var title = !string.IsNullOrEmpty(entry.Title)
                ? entry.Title
                : "";

            var slug = !string.IsNullOrEmpty(entry.Slug)
                ? entry.Slug
                : "";

            var heroImage = ContentfulHelpers.EntryIsNotALink(entry.HeroImage.SystemProperties)
                ? entry.HeroImage.File.Url
                : string.Empty;

            var teaser = !string.IsNullOrEmpty(entry.Teaser)
                ? entry.Teaser
                : "";

            var subHeading = !string.IsNullOrEmpty(entry.Subheading)
                ? entry.Subheading
                : "";

            ////var eventSubheading = !string.IsNullOrEmpty(entry.EventSubheading)
            ////    ? entry.EventSubheading
            ////    : "";

            ////var eventCategory = !string.IsNullOrEmpty(entry.EventCategory)
            ////    ? entry.EventCategory
            ////    : "";

            var featuredItems =
                entry.FeaturedItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.SystemProperties)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))    
                .Select(item => _subitemFactory.ToModel(item)).ToList();

            var breadcrumbs =
                entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var consultations =
                entry.Consultations.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                               .Select(consult => _consultationFactory.ToModel(consult)).ToList();

            var socialMediaLinks = entry.SocialMediaLinks.Where(media => ContentfulHelpers.EntryIsNotALink(media.Sys))
                                               .Select(media => _socialMediaFactory.ToModel(media)).ToList();

            return new Showcase(slug, title, featuredItems, heroImage, subHeading, teaser, breadcrumbs, consultations, socialMediaLinks); ////, eventSubheading, eventCategory);
        }
    }
}