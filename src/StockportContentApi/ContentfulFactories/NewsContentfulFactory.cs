using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class NewsContentfulFactory : IContentfulFactory<ContentfulNews, News>
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;

        public NewsContentfulFactory(IVideoRepository videoRepository, IContentfulFactory<Asset, Document> documentFactory)
        {
            _videoRepository = videoRepository;
            _documentFactory = documentFactory;
        }

        public News ToModel(ContentfulNews entry)
        {
            var documents = entry.Documents.Select(document => _documentFactory.ToModel(document)).ToList();

            return new News(entry.Title, entry.Slug, entry.Teaser, entry.Image.File.Url,
                ImageConverter.ConvertToThumbnail(entry.Image.File.Url), _videoRepository.Process(entry.Body),
                entry.SunriseDate, entry.SunsetDate, entry.Breadcrumbs, entry.Alerts, entry.Tags,
                documents, entry.Categories);
        }
    }
}