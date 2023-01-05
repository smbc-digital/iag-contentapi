using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulExpandingLinkBoxBuilder
    {
        private string _title = "title";
        private List<ContentfulReference> _links = new List<ContentfulReference> {
            new ContentfulReferenceBuilder().Slug("sub-slug").Build() };

        public ContentfulExpandingLinkBox Build()
        {
            return new ContentfulExpandingLinkBox
            {
                Title = _title,
                Links = _links
            };
        }

        public ContentfulExpandingLinkBoxBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulExpandingLinkBoxBuilder Links(List<ContentfulReference> expandingLinkBoxs)
        {
            _links = expandingLinkBoxs;
            return this;
        }
    }
}
