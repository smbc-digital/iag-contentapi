namespace StockportContentApi.ContentfulFactories.GroupFactories;

public class TrustedLogoContentfulFactory : IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>
{
    public TrustedLogo ToModel(ContentfulTrustedLogo entry)
    {
        MediaAsset image = new();

        if (entry is not null && entry.Image is not null && entry.Image.File is not null)
        {
            image = new MediaAsset
            {
                Url = entry.Image.File.Url,
                Description = entry.Image.Description
            };
        }

        return new TrustedLogo(entry.Title, entry.Text, image, entry.Link, entry.Websites);
    }
}