using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class VideoContentfulFactory : IContentfulFactory<ContentfulVideo, Video>
    {
        public Video ToModel(ContentfulVideo entry)
        {
            return new Video(entry.Heading, entry.Text, entry.VideoEmbedCode);
        }
    }
}
