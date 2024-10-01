namespace StockportContentApiTests.Unit.Builders;

public class ContentfulCallToActionBannerBuilder
{
    private readonly string _title = "title";
    private readonly string _teaser = "teaser";
    private readonly string _link = "link";
    private readonly string _altText = "altText";
    private readonly string _buttonText = "buttonText";
    private readonly EColourScheme _colour = EColourScheme.Blue;
    private readonly Asset _image = new()
    {
        File = new File
        {
            Url = "//TESTCTAIMAGE.JPG"
        }
    };

    public ContentfulCallToActionBanner Build()
    {
        return new ContentfulCallToActionBanner
        {
            Title = _title,
            Teaser = _teaser,
            Link = _link,
            AltText = _altText,
            ButtonText = _buttonText,
            Colour = _colour,
            Image = _image
        };
    }

}