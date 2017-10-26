using System;

namespace StockportContentApi.Model
{
    public class Document
    {     
        public string Title { get; }
        public int Size { get; }
        public string Url { get; }
        public DateTime LastUpdated { get; }
        public string FileName { get; }
        public string AssetId { get; set; }

        public Document(string title, int size, DateTime lastUpdated, string url, string fileName, string assetId)
        {
            Title = title;
            Size = size;
            Url = url;
            LastUpdated = lastUpdated;
            FileName = fileName;
            AssetId = assetId;
        }
    }
}
