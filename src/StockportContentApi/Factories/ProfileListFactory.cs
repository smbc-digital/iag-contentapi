using System.Collections.Generic;
using StockportContentApi.Model;
using System.Linq;

namespace StockportContentApi.Factories
{
    public class ProfileListFactory : IBuildContentTypesFromReferences<Profile>
    {
        public IEnumerable<Profile> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<Profile>();
            var profileEntries = contentfulResponse.GetEntriesFor(references);

            if (profileEntries == null) return new List<Profile>();
            return profileEntries
               .Select(item => BuildProfile(item, contentfulResponse))
               .Cast<Profile>()
               .ToList();
        }

        public Profile BuildProfile(dynamic entry, IContentfulIncludes response)
        {
            string type = entry.fields.type;
            string title = entry.fields.title;
            string slug = entry.fields.slug;
            string subtitle = entry.fields.subtitle;
            string teaser = entry.fields.teaser;
            string image = response.GetImageUrl(entry.fields.image);
            return new Profile(type, title, slug, subtitle, teaser, image);
        }
    }
}
