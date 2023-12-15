using StockportContentApi.Models;
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
                MapPosition = entry.MapPosition,
                PhoneNumber = entry.PhoneNumber,
                Email = entry.Email,
                Website = entry.Website,
                Twitter = entry.Twitter,
                Facebook = entry.Facebook,
                Address = entry.Address,
                Directories = entry.Directories.Select(contentfulDirectory => _directoryFactory.ToModel(contentfulDirectory))
            };

            directoryEntry.Themes = entry
                .Filters?
                .Select(filter => filter.Theme)
                .Distinct()
                .Select(theme => new FilterTheme() { 
                    Title = theme,
                    Filters = entry.Filters
                        .Where(filter => filter.Theme.Equals(theme))
                        .Select(filter => new Filter(filter))
                });
            
            return directoryEntry;
        }
    }
}