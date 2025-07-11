namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulEvent : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public Asset Image { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public Asset ThumbnailImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public string Description { get; set; } = string.Empty;
    public string Fee { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime EventDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int Occurrences { get; set; } = 0;
    public EventFrequency Frequency { get; set; } = EventFrequency.None;
    public List<Asset> Documents { get; set; } = new();
    public MapPosition MapPosition { get; set; } = new();
    public string BookingInformation { get; set; } = string.Empty;
    public bool Featured { get; set; } = false;
    public SystemProperties Sys { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public List<ContentfulEventCategory> EventCategories { get; set; } = new();
    public bool? Free { get; set; } = null;
    public bool? Paid { get; set; } = null;
    public string LogoAreaTitle { get; set; } = string.Empty;
    public List<ContentfulTrustedLogo> TrustedLogos { get; set; } = new();
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Facebook { get; set; } = string.Empty;
    public string Instagram { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Languages { get; set; } = string.Empty;
    public List<ContentfulCallToActionBanner> CallToActionBanners { get; set; } = new();
}