namespace StockportContentApi.ContentfulModels;

public class ContentfulOrganisation : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AboutUs { get; set; } = string.Empty;
    public Asset Image { get; set; } = new Asset { File = new File { Url = "" }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public bool Volunteering { get; set; } = false;
    public bool Donations { get; set; } = false;
    public SystemProperties Sys { get; set; } = new SystemProperties();
    public string VolunteeringText { get; set; }
    public string DonationsText { get; set; }
    public string DonationsUrl { get; set; }
}
