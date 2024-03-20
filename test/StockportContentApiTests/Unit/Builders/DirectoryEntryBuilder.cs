namespace StockportContentApiTests.Unit.Builders
{
    public class DirectoryEntryBuilder
    {
        string Slug { get; set; }
        string Title { get; set; }
        string Provider { get; set; }
        string Description { get; set; }
        string Teaser { get; set; }
        string MetaDescription { get; set; }
        string PhoneNumber { get; set; }
        string Email { get; set; }
        string Website { get; set; }
        string Twitter { get; set; }
        string Facebook { get; set; }
        string Address { get; set; }
        MapPosition MapPosition { get; set; }
        List<ContentfulAlert> Alerts { get; set; }=  new();
        List<ContentfulFilter> Filters { get; set; } = new();
        List<ContentfulGroupBranding> Branding { get; set; } = new();
        List<ContentfulDirectory> Directories { get; set; } = new();
        public ContentfulDirectoryEntry Build() => new()
        {
            Slug = this.Slug,
            Title = this.Title,
            Provider = this.Provider,
            Description = this.Description,
            MetaDescription = this.MetaDescription,
            Teaser = this.Teaser,
            PhoneNumber = this.PhoneNumber,
            Email = this.Email,
            Website = this.Website,
            Twitter = this.Twitter,
            Facebook = this.Facebook,   
            Address = this.Address,
            Filters = this.Filters,
            Alerts = this.Alerts,
            MapPosition = this.MapPosition,
            GroupBranding = Branding,
            Directories = Directories,
        };

        public DirectoryEntryBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }

        public DirectoryEntryBuilder WithProvider(string provider)
        {
            Provider = provider;
            return this;
        }


        public DirectoryEntryBuilder WithSlug(string slug)
        {
            Slug = slug;
            return this;
        }

        public DirectoryEntryBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public DirectoryEntryBuilder WithTeaser(string teaser)
        {
            Teaser = teaser;
            return this;
        }

        public DirectoryEntryBuilder WithMetaDescription(string metaDescription)
        {
            MetaDescription = metaDescription;
            return this;
        }

        public DirectoryEntryBuilder WithPhoneNumber(string phoneNumber) 
        {
            PhoneNumber = phoneNumber;
            return this;
        }

        public DirectoryEntryBuilder WithEmail(string email) 
        {
            Email = email;
            return this;
        }

        public DirectoryEntryBuilder WithWebsite(string website) 
        {
            Website = website;
            return this;
        }

        public DirectoryEntryBuilder WithTwitter(string twitter) 
        {
            Twitter = twitter;
            return this;
        }

        public DirectoryEntryBuilder WithFacebook(string facebook) 
        {
            Facebook = facebook;
            return this;
        }

        public DirectoryEntryBuilder WithAddress(string address) 
        {
            Address = address;
            return this;
        }

        public DirectoryEntryBuilder WithMapPosition(MapPosition mapPosition)
        {
            MapPosition = mapPosition;
            return this;
        }

        public DirectoryEntryBuilder WithFilter(string slug, string title, string displayName, string theme)
        {
            Filters.Add(new ContentfulFilter
            {
                Slug = slug,
                Title = title,
                DisplayName = displayName,
                Theme = theme
            });

            return this;
        }

        public DirectoryEntryBuilder WithAlert(ContentfulAlert alert)
        {
            Alerts.Add(alert);
            return this;
        }

        public DirectoryEntryBuilder WithBranding(List<ContentfulGroupBranding> branding)
        {
            Branding = branding;
            return this;
        }

        public DirectoryEntryBuilder WithDirectories(List<ContentfulDirectory> directories)
        {
            Directories = directories;
            return this;
        }
    }
}
