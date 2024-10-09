namespace StockportContentApi.ContentfulFactories;

public class CallToActionBannerContentfulFactory : IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>
{
    public CallToActionBanner ToModel(ContentfulCallToActionBanner entry) =>
        entry is null 
            ? null 
            : new CallToActionBanner
            {
                AltText = entry.AltText,
                ButtonText = entry.ButtonText,
                Image = entry.Image?.File.Url,
                Link = entry.Link,
                Title = entry.Title,
                Teaser = entry.Teaser,
                Colour = entry.Colour
            };
}