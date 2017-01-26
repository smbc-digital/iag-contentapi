using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulEntryBuilder<I>
    {
        private I _fields = default(I);
        private string _contentTypeSystemId = "id";
        private string _type = "Entry";
        private string _systemId = "id";

        public Entry<I> Build()
        {
            return new Entry<I>
            {
                Fields = _fields,
                SystemProperties =
                    new SystemProperties
                    {
                        Id = _systemId,
                        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                        Type = _type
                    }
            };
        }

        public ContentfulEntryBuilder<I> Fields(I fields)
        {
            _fields = fields;
            return this;
        }

        public ContentfulEntryBuilder<I> Include(int include)
        {
            if (include > 0) { _type = "Link"; }
            return this;
        }
    }
}
