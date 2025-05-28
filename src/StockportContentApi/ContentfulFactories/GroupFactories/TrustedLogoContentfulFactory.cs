namespace StockportContentApi.ContentfulFactories.GroupFactories;

public class TrustedLogoContentfulFactory : IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>
{
    public TrustedLogo ToModel(ContentfulTrustedLogo entry)
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

        return new TrustedLogo(entry.Title, entry.Text, file, entry.Url);
    }
}