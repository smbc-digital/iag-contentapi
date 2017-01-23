using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulCrumbBuilder
    {
        private string _slug = "slug";
        private string _title = "title";

        public ContentfulCrumb Build()
        {
            return new ContentfulCrumb
            {
                Title = _title,
                Slug = _slug
            };
        }
    }
}
