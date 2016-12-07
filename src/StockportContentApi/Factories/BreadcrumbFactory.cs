using System.Collections.Generic;
using StockportContentApi.Model;
using System.Linq;

namespace StockportContentApi.Factories
{
    public class BreadcrumbFactory : IBuildContentTypesFromReferences<Crumb>
    {
        public IEnumerable<Crumb> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<Crumb>();
            var breadcrumbEntries = contentfulResponse.GetEntriesFor(references);

            if (breadcrumbEntries == null) return new List<Crumb>();
            return breadcrumbEntries
               .Select(item => BuildCrumb(item))
               .Cast<Crumb>()
               .ToList();
        }

        public Crumb BuildCrumb(dynamic entry)
        {
            string title = entry.fields.title ?? entry.fields.name;
            string slug = entry.fields.slug;
            string contentType = entry.sys.contentType.sys.id;

            return new Crumb(title, slug, contentType);
        }
    }
}
