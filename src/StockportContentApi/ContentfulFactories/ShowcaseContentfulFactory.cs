using System.Linq;
using Contentful.Core.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ShowcaseContentfulFactory : IContentfulFactory<ContentfulShowcase, Showcase>
    {
        private readonly IContentfulFactory<ContentfulTopic, Topic> _topicFactory;
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;

        public ShowcaseContentfulFactory(IContentfulFactory<ContentfulTopic, Topic> topicFactory, IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory)
        {
            _topicFactory = topicFactory;
            _crumbFactory = crumbFactory;
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
                entry.FeaturedItems.Where(topic => ContentfulHelpers.EntryIsNotALink(topic.SystemProperties))
                    .Select(topic => _topicFactory.ToModel(topic.Fields)).ToList();

            var breadcrumbs =
                entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            return new Showcase(slug, title, featuredItems, heroImage, subHeading, teaser, breadcrumbs);
        }
    }
}