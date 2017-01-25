using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulCrumbBuilder
    {
        private string _slug = "slug";
        private string _title = "title";
        private string _name = "name";

        public ContentfulCrumb Build()
        {
            return new ContentfulCrumb
            {
                Title = _title,
                Name = _name,
                Slug = _slug
            };
        }

        public ContentfulCrumbBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public ContentfulCrumbBuilder Title(string title)
        {
            _title = title;
            return this;
        }
    }
}
