namespace StockportContentApiTests.Unit.Builders;

public class ContentfulCallToActionBannerBuilder
{
    private string _title = "title";
    private string _teaser = "teaser";
    private string _link = "link";
    private string _altText = "altText";
    private string _buttonText = "buttonText";
    private string _colour = "colour";
    private Asset _image = new Asset();

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
