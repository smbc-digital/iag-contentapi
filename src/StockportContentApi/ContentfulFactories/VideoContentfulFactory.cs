namespace StockportContentApi.ContentfulFactories;

public class VideoContentfulFactory : IContentfulFactory<ContentfulVideo, Video>
{
    public Video ToModel(ContentfulVideo entry)
        => new(entry.Heading, entry.Text, entry.VideoEmbedCode);
}