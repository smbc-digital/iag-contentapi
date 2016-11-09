using System.Collections.Generic;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class AtoZFactory : IFactory<AtoZ>
    {
        public AtoZ Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            var title = (string)entry.fields.title ?? string.Empty;
            if (string.IsNullOrEmpty(title))
            {
                title = (string)entry.fields.name ?? string.Empty;
            }
            var slug = (string)entry.fields.slug ?? string.Empty;
            var teaser = (string)entry.fields.teaser ?? string.Empty;
            var type = (string)entry.sys.contentType.sys.id ?? string.Empty;
            var alternativeTitles = GetAlternativeTitles(entry);

            return new AtoZ(title, slug, teaser, type, alternativeTitles);
        }

        private static List<string> GetAlternativeTitles(dynamic entry)
        {
            var alternativeTitles = new List<string>();

            if (entry.fields.alternativeTitles == null) return alternativeTitles;

            foreach (var alternativeTitle in entry.fields.alternativeTitles)
            {
                alternativeTitles.Add((string) alternativeTitle);
            }
            return alternativeTitles;
        }
    }
}
