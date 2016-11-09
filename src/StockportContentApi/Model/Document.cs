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

        public Document(string title, int size, DateTime lastUpdated, string url, string fileName)
        {
            Title = title;
            Size = size;
            Url = url;
            LastUpdated = lastUpdated;
            FileName = fileName;
        }
    }
}
