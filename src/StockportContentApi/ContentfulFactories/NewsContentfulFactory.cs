using System.Linq;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class NewsContentfulFactory : IContentfulFactory<ContentfulNews, News>
    {
        private readonly IVideoRepository _videoRepository;

        public NewsContentfulFactory(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }
        public News ToModel(ContentfulNews entry)
        {
            return new News(entry.Title, entry.Slug, entry.Teaser, entry.Image.File.Url,
                ImageConverter.ConvertToThumbnail(entry.Image.File.Url), _videoRepository.Process(entry.Body),
                entry.SunriseDate, entry.SunsetDate, entry.Breadcrumbs, entry.Alerts, entry.Tags,
                entry.Documents.Select(x => new Document(x.Description,
                    unchecked((int)x.File.Details.Size),
                    x.SystemProperties.UpdatedAt.Value,
                    x.File.Url,
                    x.File.FileName)).ToList(),
                entry.Categories);
        }
    }
}