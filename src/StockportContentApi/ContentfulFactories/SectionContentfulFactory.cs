using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;

namespace StockportContentApi.ContentfulFactories
{
    public class SectionContentfulFactory : IContentfulFactory<ContentfulSection, Section>
    {
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly IVideoRepository _videoRepository;

        public SectionContentfulFactory(IContentfulFactory<ContentfulProfile, Profile> profileFactory, 
                                        IContentfulFactory<Asset, Document> documentFactory,
                                        IVideoRepository videoRepository)
        {
            _profileFactory = profileFactory;
            _documentFactory = documentFactory;
            _videoRepository = videoRepository;
        }

        public Section ToModel(ContentfulSection entry)
        {
            var profiles = entry.Profiles.Select(profile => _profileFactory.ToModel(profile)).ToList();
            var documents = entry.Documents.Select(document => _documentFactory.ToModel(document)).ToList();
            var body = _videoRepository.Process(entry.Body);

            return new Section(entry.Title, entry.Slug, body, profiles, documents, 
                               entry.SunriseDate, entry.SunsetDate);
        }
    }
}