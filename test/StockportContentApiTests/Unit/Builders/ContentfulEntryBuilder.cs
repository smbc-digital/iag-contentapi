using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulEntryBuilder<T>
    {
        private T _fields = default(T);
        private string _contentTypeSystemId = "id";
        private string _type = "Entry";
        private string _systemId = "id";

        public Entry<T> Build()
        {
            return new Entry<T>
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

        public ContentfulEntryBuilder<T> Fields(T fields)
        {
            _fields = fields;
            return this;
        }

        public ContentfulEntryBuilder<T> Include(int include)
        {
            if (include > 0) { _type = "Link"; }
            return this;
        }

        public ContentfulEntryBuilder<T> ContentTypeSystemId(string id)
        {
            _contentTypeSystemId = id;
            return this;
        }

        public ContentfulEntryBuilder<T> SystemId(string id)
        {
            _systemId = id;
            return this;
        }
    }
}
