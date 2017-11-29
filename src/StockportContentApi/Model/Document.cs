using StockportContentApi.Attributes;
using System;

namespace StockportContentApi.Model
{
    public class Document
    {
        [SensitiveData]
        public string Title { get; set; }
        public int Size { get; set; }
        [SensitiveData]
        public string Url { get; set; }
        public DateTime LastUpdated { get; set; }
        [SensitiveData]
        public string FileName { get; set; }
        [SensitiveData]
        public string AssetId { get; set; }
        [SensitiveData]
        public string MediaType { get; set; }

        public Document(string title, int size, DateTime lastUpdated, string url, string fileName, string assetId, string mediaType)
        {
            Title = title;
            Size = size;
            Url = url;
            LastUpdated = lastUpdated;
            FileName = fileName;
            AssetId = assetId;
            MediaType = mediaType;
        }
    }
}
