using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulAssetBuilder
    {
        private string _url = "image-url.jpg";
        private string _title = "title";
        private string _type = "Asset";
        private string _fileName = "fileName";
        private string _description = "description";
        private int _size = int.MaxValue;
        private DateTime _updatedAt;

        public Asset Build()
        {
            return new Asset
            {
                Description = _description,
                File = new File
                {
                    Url = _url,
                    Details = new FileDetails { Size = _size },
                    FileName = _fileName,
                },
                SystemProperties = new SystemProperties
                {
                    Type = _type,
                    UpdatedAt = _updatedAt
                },
                Title = _title
            };
        }

        public ContentfulAssetBuilder Url(string url)
        {
            _url = url;
            return this;
        }

        public ContentfulAssetBuilder Include(int include)
        {
            if (include > 0) { _type = "Link"; }
            return this;
        }

        public ContentfulAssetBuilder FileName(string fileName)
        {
            _fileName = fileName;
            return this;
        }

        public ContentfulAssetBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        public ContentfulAssetBuilder FileSize(int size)
        {
            _size = size;
            return this;
        }

        public ContentfulAssetBuilder UpdatedAt(DateTime updatedAt)
        {
            _updatedAt = updatedAt;
            return this;
        }

        public ContentfulAssetBuilder Title(string title)
        {
            _title = title;
            return this;
        }
    }
}
