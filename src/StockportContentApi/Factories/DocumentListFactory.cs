using System;
using System.Collections.Generic;
using StockportContentApi.Model;
using System.Linq;

namespace StockportContentApi.Factories
{
    public class DocumentListFactory : IBuildContentTypesFromReferences<Document>
    {
        public IEnumerable<Document> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<Document>();
            var documentEntries = contentfulResponse.GetAssetsFor(references);

            if (documentEntries == null) return new List<Document>();
            return documentEntries
               .Select(item => BuildDocument(item))
               .Cast<Document>()
               .ToList();
        }

        private static Document BuildDocument(dynamic entry)
        {
            var title = (string)entry.fields.description ?? string.Empty;

            int size;
            int.TryParse((string) entry.fields.file.details.size, out size);

            var date = entry.sys.updatedAt.ToString();
            DateTime lastUpdated;
            DateTime.TryParse(date ?? string.Empty, out lastUpdated);

            var url = (string)entry.fields.file.url ?? string.Empty;
            var filename = (string)entry.fields.file.fileName ?? string.Empty;

            return new Document(title, size, lastUpdated, url, filename);
        }
    }
}
