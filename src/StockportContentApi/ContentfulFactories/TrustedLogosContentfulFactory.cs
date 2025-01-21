namespace StockportContentApi.ContentfulFactories;

public class GroupBrandingContentfulFactory : IContentfulFactory<ContentfulTrustedLogos, TrustedLogos>
{
    public TrustedLogos ToModel(ContentfulTrustedLogos entry)
    {
        MediaAsset file = new();

        if (entry is not null && entry.File is not null && entry.File.File is not null)
        {
            file = new MediaAsset
            {
                Url = entry.File.File.Url,
                Description = entry.File.Description
            };
        }

        return new TrustedLogos(entry.Title, entry.Text, file, entry.Url);
    }
}