using System;
using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulDocumentBuilder
    {
        private string _description = "documentTitle";
        private string _url = "url.pdf";
        private int _size = 674192;
        private string _fileName = "fileName";
        private DateTime _updatedAt = new DateTime(2016, 10, 05, 00, 00, 00, DateTimeKind.Utc);

        public Asset Build()
        {
            return new Asset
            {
                Description = _description,
                File = new File
                {
                    Details = new FileDetails { Size = _size },
                    Url = _url,
                    FileName = _fileName
                },
                SystemProperties =
                    new SystemProperties { UpdatedAt = _updatedAt }
            };
        }
    }
}
