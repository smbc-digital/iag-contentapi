namespace StockportContentApi.ContentfulModels;

public class ContentfulGroupHomepage : ContentfulReference
{
    public string FeaturedGroupsHeading { get; set; } = string.Empty;
    public List<ContentfulGroup> FeaturedGroups { get; set; } = new();
    public ContentfulGroupCategory FeaturedGroupsCategory { get; set; } = new();
    public ContentfulGroupSubCategory FeaturedGroupsSubCategory { get; set; } = new();
    public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    public string BodyHeading { get; set; } = string.Empty;
    public string SecondaryBodyHeading { get; set; } = string.Empty;
    public string SecondaryBody { get; set; } = string.Empty;
    public ContentfulEventBanner EventBanner { get; set; } = new()
    {
        Sys = new() { Type = "Entry" }
    };
}