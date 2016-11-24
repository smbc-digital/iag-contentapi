using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class StartPageFactory : IFactory<StartPage>
    {
        private readonly IBuildContentTypesFromReferences<Crumb> _breadcrumbFactory;
        private readonly IBuildContentTypesFromReferences<Alert> _alertListFactory;

        public StartPageFactory(IBuildContentTypesFromReferences<Crumb> breadcrumbFactory, IBuildContentTypesFromReferences<Alert> alertListFactory)
        {
            _breadcrumbFactory = breadcrumbFactory;
            _alertListFactory = alertListFactory;
        }

        public StartPage Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry == null || entry.fields == null)
                return new NullStartPage();

            var fields = entry.fields;

            string title = fields.title;
            string slug = fields.slug;
            string teaser = fields.teaser;
            string summary = fields.summary;
            string lowerBody = fields.lowerBody;
            string icon = fields.icon;

            string upperBody = fields.upperBody;
            string formLinkLabel = fields.formLinkLabel;
            string formLink = fields.formLink;

            var crumbs = _breadcrumbFactory.BuildFromReferences(fields.breadcrumbs, contentfulResponse);

            string backgroundImage = contentfulResponse.GetImageUrl(fields.backgroundImage);
            var alerts = _alertListFactory.BuildFromReferences(fields.alerts, contentfulResponse);

            return new StartPage(title, slug, teaser, summary, upperBody, formLinkLabel, formLink, lowerBody, backgroundImage, icon, crumbs, alerts);
        }
    }
}
