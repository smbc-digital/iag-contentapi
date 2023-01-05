using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StockportContentApi.Utils
{
    public class SwaggerFilter : IDocumentFilter
    {

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathStart = "/{businessId}";
            var pathLength = pathStart.Length;
            foreach (var item in swaggerDoc.Paths)
            {
                var path = item.Key;
                if (path.ToString().Substring(0, pathLength) == pathStart)
                {
                    swaggerDoc.Paths[path] = null;
                }
            }
        }
    }
}
