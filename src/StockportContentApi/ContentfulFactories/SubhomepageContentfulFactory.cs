using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class SubhomepageContentfulFactory : IContentfulFactory<Entry<ContentfulSubhomepage>, Subhomepage>
    {
        private readonly IContentfulFactory<Entry<ContentfulTopic>, Topic> _topicFactory;

        public SubhomepageContentfulFactory(IContentfulFactory<Entry<ContentfulTopic>, Topic> topicFactory)
        {
            _topicFactory = topicFactory;
        }

        public Subhomepage ToModel(Entry<ContentfulSubhomepage> entryContentfulSubhomepage)
        {
            var entry = entryContentfulSubhomepage.Fields;

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

            var featuredTopic =
                entry.FeaturedTopic.Where(topic => ContentfulHelpers.EntryIsNotALink(topic.SystemProperties))
                    .Select(topic => _topicFactory.ToModel(topic)).ToList();

            return new Subhomepage(slug, title, featuredTopic, heroImage, subHeading, teaser);
        }
    }
}