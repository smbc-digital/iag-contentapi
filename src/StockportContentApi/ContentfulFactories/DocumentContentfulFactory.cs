namespace StockportContentApi.ContentfulFactories;

public class DocumentContentfulFactory : IContentfulFactory<Asset, Document>
{
    public Document ToModel(Asset entry)
    {
        entry.File ??= new() { Url = string.Empty };

        return new()
        {
            Title = entry.Description,
            Size = unchecked((int)entry.File?.Details.Size),
            LastUpdated = entry.SystemProperties.UpdatedAt.Value,
            Url = entry.File?.Url,
            FileName = entry.File?.FileName,
            AssetId = entry.SystemProperties.Id,
            MediaType = entry.File.ContentType
        };
    }
}