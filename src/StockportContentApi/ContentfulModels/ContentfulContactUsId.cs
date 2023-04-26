namespace StockportContentApi.ContentfulModels;

public class ContentfulContactUsId : IContentfulModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string SuccessPageButtonText { get; set; } = string.Empty;
    public string SuccessPageReturnUrl { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new SystemProperties();
}
