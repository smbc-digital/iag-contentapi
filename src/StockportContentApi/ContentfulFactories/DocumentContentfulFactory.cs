using Contentful.Core.Models;
using Microsoft.AspNetCore.Http;
using StockportContentApi.Utils;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApi.ContentfulFactories
{
    public class DocumentContentfulFactory : IContentfulFactory<Asset, Document>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DocumentContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Document ToModel(Asset entry)
        {
            if (entry.File == null)
            {
                entry.File = new File { Url = "" };
            }

            return new Document(
                    entry.Description,
                    unchecked((int)entry.File?.Details.Size),
                    entry.SystemProperties.UpdatedAt.Value,
                    entry.File?.Url,
                    entry.File?.FileName,
                    entry.SystemProperties.Id,
                    entry.File.ContentType).StripData(_httpContextAccessor);
        }
    }
}