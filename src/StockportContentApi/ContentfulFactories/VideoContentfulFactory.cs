using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class VideoContentfulFactory : IContentfulFactory<ContentfulVideo, Video>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VideoContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Video ToModel(ContentfulVideo entry)
        {
            return new Video(entry.Heading, entry.Text, entry.VideoEmbedCode).StripData(_httpContextAccessor);
        }
    }
}
