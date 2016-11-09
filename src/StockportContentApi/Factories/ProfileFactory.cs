using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class ProfileFactory : IFactory<Profile>
    {
        private readonly IBuildContentTypesFromReferences<Crumb> _breadcrumbFactory;

        public ProfileFactory(IBuildContentTypesFromReferences<Crumb> breadcrumbFactory)
        {
            _breadcrumbFactory = breadcrumbFactory;
        }

        public Profile Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            var fields = entry.fields;
            if (fields == null)
                return null;

            string type = fields.type;
            string title = fields.title;
            string slug = fields.slug;
            string subtitle = fields.subtitle;
            string body = fields.body;
            string icon = fields.icon;
            string backgroundImage = contentfulResponse.GetImageUrl(fields.backgroundImage);
            var breadcrumbs = _breadcrumbFactory.BuildFromReferences(fields.breadcrumbs, contentfulResponse);

            return new Profile(type, title, slug, subtitle, body, icon, backgroundImage, breadcrumbs);
        }
    }
}