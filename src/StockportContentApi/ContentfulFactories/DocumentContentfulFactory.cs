using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class DocumentContentfulFactory : IContentfulFactory<Asset, Document>
    {
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
                    entry.File?.FileName);
        }
    }
}