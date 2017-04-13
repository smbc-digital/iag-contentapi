﻿using System.Linq;
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
        private readonly DateComparer _dateComparer;
        
        public ShowcaseContentfulFactory(IContentfulFactory<Entry<ContentfulSubItem>, SubItem> subitemFactory, IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory, ITimeProvider timeProvider)
        {
            _subitemFactory = subitemFactory;
            _crumbFactory = crumbFactory;
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
            
            var featuredItems =
                entry.FeaturedItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.SystemProperties)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))    
                .Select(item => _subitemFactory.ToModel(item)).ToList();

            var breadcrumbs =
                entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            return new Showcase(slug, title, featuredItems, heroImage, subHeading, teaser, breadcrumbs);
        }
    }
}