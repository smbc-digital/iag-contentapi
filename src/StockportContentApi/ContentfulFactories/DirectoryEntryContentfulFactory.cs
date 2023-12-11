using StackExchange.Redis;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Models;
using StockportContentApi.Utils;
using Directory = StockportContentApi.Model.Directory;

namespace StockportContentApi.ContentfulFactories
{
    public class DirectoryEntryContentfulFactory : IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>
    {
        private readonly IContentfulFactory<ContentfulDirectory, Directory> _directoryFactory;

        public DirectoryEntryContentfulFactory(IContentfulFactory<ContentfulDirectory, Directory> directoryFactory)
        {
            _directoryFactory = directoryFactory;
        }

        public DirectoryEntry ToModel(ContentfulDirectoryEntry entry)
        {
            if (entry is null)
                return null;

            var directoryEntry = new DirectoryEntry
            {
                Slug = entry.Slug,
                Title = entry.Title,
                Body = entry.Body,
                Teaser = entry.Teaser,
                MetaDescription = entry.MetaDescription,
                Directories = entry.Directories.Select(contentfulDirectory => _directoryFactory.ToModel(contentfulDirectory))
            };

            var themes = entry.Filters.Select(filter => filter.Theme).Distinct();
            directoryEntry.Themes = themes.Select(theme => new FilterTheme() { 
                Title = theme,
                Filters = entry.Filters
                            .Where(filter => filter.Theme.Equals(theme))
                            .Select(filter => new Filter(filter))
            });
            
            return directoryEntry;
        }
    }
}
