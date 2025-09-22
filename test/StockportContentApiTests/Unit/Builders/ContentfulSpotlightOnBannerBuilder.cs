namespace StockportContentApiTests.Unit.Builders;

public class ContentfulSpotlightOnBannerBuilder
{
    public ContentfulSpotlightOnBanner Build()
        => new()
        {
            Title = "Spotlight title",
            Image = new Asset()
            {
                File = new File()
                {
                    Url = "ImageUrl"
                }
            },
            Link = "Link",
            Sys = new SystemProperties {
                PublishedAt = DateTime.MinValue,
                UpdatedAt = DateTime.MinValue.AddDays(1)
            }
        };
}