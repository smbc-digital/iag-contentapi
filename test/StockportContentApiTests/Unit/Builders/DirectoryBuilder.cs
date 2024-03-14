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


        public ContentfulDirectory Build() => new ContentfulDirectory()
        {
            Slug = this.Slug,
            Title = this.Title,
            Body = this.Body,
            MetaDescription = this.MetaDescription,
            Teaser = this.Teaser,
            Sys = new SystemProperties()
            {
                Id = this.Id
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
            CallToAction = this.CallToActionBanner,
            Alerts = this.Alerts
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
    }
}
