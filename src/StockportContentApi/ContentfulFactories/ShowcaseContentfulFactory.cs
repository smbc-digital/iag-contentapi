using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ShowcaseContentfulFactory : IContentfulFactory<ContentfulShowcase, Showcase>
    {
        private readonly IContentfulFactory<Entry<ContentfulTopic>, Topic> _topicFactory;

        public ShowcaseContentfulFactory(IContentfulFactory<Entry<ContentfulTopic>, Topic> topicFactory)
        {
            _topicFactory = topicFactory;
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

            var test = _topicFactory.ToModel(entry.FeaturedTopic[0]);

            var featuredTopic =
                entry.FeaturedTopic.Where(topic => ContentfulHelpers.EntryIsNotALink(topic.SystemProperties))
                    .Select(topic => _topicFactory.ToModel(topic)).ToList();

            return new Showcase(slug, title, featuredTopic, heroImage, subHeading, teaser);
        }
    }
}