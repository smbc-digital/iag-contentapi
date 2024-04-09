namespace StockportContentApiTests.Unit.Builders
{
    public class DirectoryBuilder
    {
        string Slug { get; set; }
        string Title { get; set; }
        string Body { get; set; }
        string Teaser { get; set; }
        string MetaDescription { get; set; }
        string Id { get; set; }
        string BackgroundImageUrl { get; set; }
        List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        ContentfulCallToActionBanner CallToActionBanner { get; set; }
        private List<ContentfulReference> _relatedContent = new() {
            new ContentfulReferenceBuilder().Slug("sub-slug").Build()
        };
        private List<ContentfulExternalLink> _externalLinks = new() {
            new ContentfulExternalLink()
        };

        private List<ContentfulDirectoryEntry> _pinnedEntries = new() {
            new ContentfulDirectoryEntry()
        };
        public ContentfulDirectory Build() => new()
        {
            Slug = Slug,
            Title = Title,
            Body = Body,
            MetaDescription = MetaDescription,
            Teaser = Teaser,
            Sys = new SystemProperties()
            {
                Id = Id
            },
            BackgroundImage = new Asset()
            {
                File = new File
                {
                    Url = BackgroundImageUrl
                },
                SystemProperties = new SystemProperties()
                {
                    Type = "Image"
                }
            },
            CallToAction = CallToActionBanner,
            Alerts = Alerts,
            RelatedContent = _relatedContent,
            ExternalLinks = _externalLinks,
            PinnedEntries = _pinnedEntries,
        };

        public DirectoryBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }

        public DirectoryBuilder WithSlug(string slug)
        {
            Slug = slug;
            return this;
        }

        public DirectoryBuilder WithBody(string body)
        {
            Body = body;
            return this;
        }

        public DirectoryBuilder WithTeaser(string teaser)
        {
            Teaser = teaser;
            return this;
        }

        public DirectoryBuilder WithMetaDescription(string metaDescription)
        {
            MetaDescription = metaDescription;
            return this;
        }

        public DirectoryBuilder WithId(string id)
        {
            Id = id;
            return this;
        }

        public DirectoryBuilder WithBackgroundImageUrl(string _backgroundImageUrl)
        {
            BackgroundImageUrl = _backgroundImageUrl;
            return this;
        }

        public DirectoryBuilder WithCallToAction(ContentfulCallToActionBanner banner)
        {
            CallToActionBanner = banner;
            return this;
        }

        public DirectoryBuilder WithAlert(ContentfulAlert alert) //string slug, string body,string title)
        {
            Alerts.Add(alert);
            return this;
        }

        public DirectoryBuilder WithRelatedContent(List<ContentfulReference> relatedContent)
        {
            _relatedContent = relatedContent;
            return this;
        }

        public DirectoryBuilder WithExternalLinks(List<ContentfulExternalLink> externalLinks)
        {
            _externalLinks = externalLinks;
            return this;
        }

        public DirectoryBuilder WithPinnedEntries(List<ContentfulDirectoryEntry> pinnedEntries)
        {
            _pinnedEntries = pinnedEntries;
            return this;
        }
    }
}
