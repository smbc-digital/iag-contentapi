using System.Collections.Generic;
using System.Linq;

namespace StockportContentApi
{
    public interface IContentfulIncludes
    {
        IEnumerable<dynamic> GetEntriesFor(IEnumerable<dynamic> references);
        IEnumerable<dynamic> GetAssetsFor(IEnumerable<dynamic> references);
        dynamic GetEntryFor(dynamic reference);
        string GetImageUrl(dynamic image);

        IEnumerable<dynamic> GetEntriesAndItemsFor(IEnumerable<dynamic> references);
    }

    public class ContentfulResponse : IContentfulIncludes
    {
        private readonly dynamic _content;
        public readonly IList<dynamic> Items;
        private readonly IEnumerable<dynamic> _assets;
        private readonly IEnumerable<dynamic> _entries;

        public ContentfulResponse(dynamic content)
        {
            _content = content;
            _entries = HasEntries(content) ? content.includes.Entry.ToObject<List<dynamic>>() : new List<dynamic>();
            _assets = HasAssets(content) ? content.includes.Asset.ToObject<List<dynamic>>() : new List<dynamic>();
            Items = HasItems() ? content.items.ToObject<List<dynamic>>() : new List<dynamic>();
        }

        private bool HasReferences()
        {
            return _content != null && _content.includes != null;
        }

        private bool HasAssets(dynamic content)
        {
            return HasReferences() && content.includes.Asset != null;
        }

        private bool HasEntries(dynamic content)
        {
            return HasReferences() && content.includes.Entry != null;
        }

        public bool HasItems()
        {
            return _content != null && _content.items != null && _content.items.Count > 0;
        }

        public IEnumerable<dynamic> GetEntriesFor(IEnumerable<dynamic> references)
        {
            var entries = references
                .Select(reference => GetEntryFor(reference))
                .Where(entry => entry != null);

            return entries;
        }

        public IEnumerable<dynamic> GetAssetsFor(IEnumerable<dynamic> references)
        {
            var assets = references
                .Select(reference => GetAssetFor(reference))
                .Where(asset => asset != null);

            return assets;
        }

        public dynamic GetEntryFor(dynamic reference)
        {
            return _entries.FirstOrDefault(o => o.sys.id == reference.sys.id);
        }

        public dynamic GetAssetFor(dynamic reference)
        {
            return _assets.FirstOrDefault(o => o.sys.id == reference.sys.id);
        }

        public dynamic GetItemFor(dynamic reference)
        {
            return Items.FirstOrDefault(o => o.sys.id == reference.sys.id);
        }

        public dynamic GetFirstItem()
        {
            return HasItems() ? Items[0] : null;
        }

        public IEnumerable<dynamic> GetAllItems()
        {
            return HasItems() ? Items : null;
        }

        public string GetImageUrl(dynamic image)
        {
            if (image == null) return string.Empty;
            var imageObject = GetFieldForReference(image, _assets);
            return imageObject != null ? imageObject.file.url : null;
        }

        private static dynamic GetFieldForReference(dynamic reference, IEnumerable<dynamic> entries)
        {
            var entry = entries.FirstOrDefault(o => o.sys.id == reference.sys.id);
            return entry != null ? entry.fields : null;
        }

        public IEnumerable<dynamic> GetEntriesAndItemsFor(IEnumerable<dynamic> references)
        {
            return references
                .Select(reference => GetEntryOrItemFor(reference))
                .Where(entry => entry != null);
        }

        private dynamic GetEntryOrItemFor(dynamic reference)
        {
            return _entries.FirstOrDefault(o => o.sys.id == reference.sys.id) ??
                   Items.FirstOrDefault(o => o.sys.id == reference.sys.id);
        }
    }

    public class NullContentfulResponse : ContentfulResponse
    {
        public NullContentfulResponse() : base(null)
        {
        }
    }
}