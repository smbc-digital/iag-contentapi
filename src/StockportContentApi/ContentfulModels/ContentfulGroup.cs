namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulGroup : IContentfulModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public string Facebook { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Asset Image { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public List<ContentfulGroupCategory> CategoriesReference { get; set; } = new();
    public List<ContentfulGroupSubCategory> SubCategories { get; set; } = new();
    public MapPosition MapPosition { get; set; } = new();
    public bool Volunteering { get; set; } = false;
    public bool Donations { get; set; } = false;
    public SystemProperties Sys { get; set; } = new();
    public GroupAdministrators GroupAdministrators { get; set; } = new();
    public DateTime? DateHiddenFrom { get; set; }
    public DateTime? DateHiddenTo { get; set; }
    public List<string> Cost { get; set; } = new();
    public string CostText { get; set; }
    public string AbilityLevel { get; set; }
    public string VolunteeringText { get; set; }
    public ContentfulOrganisation Organisation { get; set; } = new();
    public string AccessibleTransportLink { get; set; } = "/accessibleTransport";
    public string AdditionalInformation { get; set; } = string.Empty;
    public List<Asset> AdditionalDocuments { get; set; } = new();
    public List<string> SuitableFor { get; set; } = new();
    public List<string> AgeRange { get; set; } = new();
    public string DonationsText { get; set; }
    public string DonationsUrl { get; set; }
    public List<ContentfulGroupBranding> GroupBranding { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public List<ContentfulAlert> AlertsInline { get; set; } = new();
}