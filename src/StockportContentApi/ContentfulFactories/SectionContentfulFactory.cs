using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class SectionContentfulFactory : IContentfulFactory<ContentfulSection, Section>
    {
        private IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private IContentfulFactory<Asset, Document> _documentFactory;

        public SectionContentfulFactory(IContentfulFactory<ContentfulProfile, Profile> profileFactory, IContentfulFactory<Asset, Document> documentFactory)
        {
            _profileFactory = profileFactory;
            _documentFactory = documentFactory;
        }

        public Section ToModel(ContentfulSection entry)
        {
            var profiles = entry.Profiles.Select(profile => _profileFactory.ToModel(profile)).ToList();
            var documents = entry.Documents.Select(document => _documentFactory.ToModel(document)).ToList();

            return new Section(entry.Title, entry.Slug, entry.Body, profiles, documents, entry.SunriseDate, entry.SunsetDate);
        }
    }
}