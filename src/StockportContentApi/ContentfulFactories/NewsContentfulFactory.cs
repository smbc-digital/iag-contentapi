using System.Collections.Generic;
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
            var documents = entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                           .Select(document => _documentFactory.ToModel(document)).ToList();
            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;

            return new News(entry.Title, entry.Slug, entry.Teaser, imageUrl, ImageConverter.ConvertToThumbnail(imageUrl), 
                _videoRepository.Process(entry.Body), entry.SunriseDate, entry.SunsetDate, new List<Crumb> { new Crumb("News", string.Empty, "news") }, 
                entry.Alerts, entry.Tags, documents, entry.Categories);
        }
    }
}