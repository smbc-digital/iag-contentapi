using StockportContentApi.Models;

namespace StockportContentApi.Model
{
    public class DirectoryEntry
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Teaser { get; set; }
        public string MetaDescription { get; set; }
        public IEnumerable<FilterTheme> Themes { get; set; }
        public IEnumerable<Directory> Directories { get; set; }


        public DirectoryEntry() { } 

        public DirectoryEntry(ContentfulDirectoryEntry contentfulDirectoryEntry)
        {
            Slug = contentfulDirectoryEntry.Slug;
            Title = contentfulDirectoryEntry.Title;
            Body = contentfulDirectoryEntry.Body;
            Teaser = contentfulDirectoryEntry.Teaser;
            MetaDescription = contentfulDirectoryEntry.MetaDescription;
            
        }
                
    }

}
