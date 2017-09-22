using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;


namespace StockportContentApi.Utils
{
    public class SwaggerFilter : IDocumentFilter
    {

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathStart = "/api/{businessId}";
            var pathLength = pathStart.Length;
            foreach (var item in swaggerDoc.Paths)
            {
                var path = item.Key;
                if (path.ToString().Substring(0, pathLength) == pathStart)
                {
                   swaggerDoc.Paths[path].Get = null;
                }
            } 
        }
    }
}
