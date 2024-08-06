namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulShowcase : ContentfulReference
{
    public Asset HeroImage { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public string Subheading { get; set; } = string.Empty;
    public string FeaturedItemsSubheading { get; set; } = string.Empty;
    public string SocialMediaLinksSubheading { get; set; } = string.Empty;
    public string EventSubheading { get; set; } = string.Empty;
    public string EventCategory { get; set; } = string.Empty;
    public string EventsReadMoreText { get; set; } = string.Empty;
    public string NewsSubheading { get; set; } = string.Empty;
    public string NewsCategoryTag { get; set; } = string.Empty;
    //public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
    public string BodySubheading { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string ProfileHeading { get; set; } = string.Empty;
    public string ProfileLink { get; set; } = string.Empty;
    public string EmailAlertsTopicId { get; set; } = string.Empty;
    public string EmailAlertsText { get; set; } = string.Empty;
    public string TriviaSubheading { get; set; } = string.Empty;
    public string TypeformUrl { get; set; } = string.Empty;
    public IDictionary<string, dynamic> Content { get; set; } = new Dictionary<string, dynamic>();
    public List<SubItem> SubItems { get; set; } = new();
}