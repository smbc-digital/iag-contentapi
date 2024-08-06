namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulFooter : ContentfulReference
{
    public new string Title { get; set; } = string.Empty;
    public new string Slug { get; set; } = string.Empty;
    public string CopyrightSection { get; set; } = string.Empty;
    public List<ContentfulReference> Links { get; set; } = new();
    public List<ContentfulSocialMediaLink> SocialMediaLinks { get; set; } = new();
}