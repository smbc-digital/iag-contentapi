namespace StockportContentApi.ContentfulModels;

/// <summary>
/// Contentful base interface, all shared properties between all models
/// </summary>
public interface IContentfulModel
{
    SystemProperties Sys { get; set; }
}