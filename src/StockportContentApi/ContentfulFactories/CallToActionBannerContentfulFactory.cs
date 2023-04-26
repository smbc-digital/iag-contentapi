namespace StockportContentApi.ContentfulFactories;

public class CallToActionBannerContentfulFactory : IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>
{
    public CallToActionBanner ToModel(ContentfulCallToActionBanner entry)
    {
        if (entry is null)
            return null;

        return new CallToActionBanner()
        {
            AltText = entry.AltText,
            ButtonText = entry.ButtonText,
            Image = entry.Image.File.Url,
            Link = entry.Link,
            Title = entry.Title
        };
    }
}
