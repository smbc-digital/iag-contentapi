namespace StockportContentApi.Utils;

[ExcludeFromCodeCoverage]
public class SwaggerFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        string pathStart = "/{businessId}";
        int pathLength = pathStart.Length;

        foreach (KeyValuePair<string, OpenApiPathItem> item in swaggerDoc.Paths)
        {
            string path = item.Key;
            if (path.ToString()[..pathLength].Equals(pathStart))
                swaggerDoc.Paths[path] = null;
        }
    }
}