namespace StockportContentApiTests.Unit.Builders;

public class ContentfulCallToActionBannerBuilder
{
    private readonly Asset _image = new()
    {
        File = new File
        {
            Url = "//TESTCTAIMAGE.JPG"
        }
    };

    public ContentfulCallToActionBanner Build()
        => new()
        {
            Title = "title",
            Teaser = "teaser",
            Link = "link",
            AltText = "altText",
            ButtonText = "buttonText",
            Colour = EColourScheme.Blue,
            Image = _image
        };
}