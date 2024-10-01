namespace StockportContentApiTests.Unit.Builders;

public class ContentfulGroupBuilder
{
    private readonly string _name = "_name";
    private string _slug = "_slug";
    private readonly string _phoneNumber = "_phoneNumber";
    private string _metaDescription = "_metaDescription";
    private readonly string _email = "_email";
    private readonly string _website = "_website";
    private readonly string _twitter = "_twitter";
    private readonly string _facebook = "_facebook";
    private readonly string _address = "_address";
    private readonly string _description = "_description";
    private List<string> _cost = new() { "lots" };
    private string _costText = string.Empty;
    private string _abilityLevel = string.Empty;
    private readonly string _accessibleTransportLink = "link";
    private readonly string _additionalInformation = "info";
    private readonly Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
    private List<ContentfulGroupCategory> _categoriesReference = new()
    {
        new ContentfulGroupCategory
        {
            Name = "name",
            Slug = "slug",
            Image = new Asset
            {
                SystemProperties = new SystemProperties
                {
                    Type = "NotALink"
                },
                File = new File
                {
                    Url = "test-url"
                }
            },
            Icon = "icon",
            Sys = new SystemProperties
            {
                Type = "NotALink"
            }
        }
    };
    private MapPosition _mapPosition = new() { Lat = 39, Lon = 2 };
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } },
        UpdatedAt = DateTime.MinValue
    };
    private GroupAdministrators _groupAdministrators = new()
    {
        Items = new List<GroupAdministratorItems>
        {
            new() {
                Name = "Name",
                Email = "Email",
                Permission = "admin"
            }
        }
    };
    private DateTime _dateHiddenFrom = DateTime.MinValue;
    private DateTime _dateHiddenTo = DateTime.MinValue;
    private readonly ContentfulOrganisation _organisation = new();
    private readonly List<string> _suitableFor = new()
    {
        "people"
    };
    private readonly List<string> _ageRanges = new() { "15-20" };
    private readonly bool _volunteering = true;
    private readonly string _volunteeringText = "text";
    private readonly List<Asset> _additionalDocuments = new()
    {
        new Asset
        {
            Title = "Document",
            SystemProperties = new SystemProperties
            {
                Type = "NotALink",
                UpdatedAt = DateTime.MinValue,
                Id = "id"
            },
            File = new File
            {
                ContentType = "test",
                Details = new FileDetails
                {
                    Size = 200
                },
                Url = "url",
                FileName = "filename"
            },
            Description = "description"
        }
    };
    private readonly string _donationsText = "donText";
    private readonly string _donationsUrl = "donUrl";
    private readonly List<ContentfulGroupSubCategory> _subCategories = new()
    {
        new ContentfulGroupSubCategory
        {
            Name = "name",
            Slug = "slug"
        }
    };

    public ContentfulGroup Build()
    {
        return new ContentfulGroup
        {
            Address = _address,
            Description = _description,
            Email = _email,
            Facebook = _facebook,
            Name = _name,
            PhoneNumber = _phoneNumber,
            Slug = _slug,
            MetaDescription = _metaDescription,
            Twitter = _twitter,
            Website = _website,
            Image = _image,
            CategoriesReference = _categoriesReference,
            MapPosition = _mapPosition,
            Sys = _sys,
            GroupAdministrators = _groupAdministrators,
            DateHiddenFrom = _dateHiddenFrom,
            DateHiddenTo = _dateHiddenTo,
            Cost = _cost,
            CostText = _costText,
            AbilityLevel = _abilityLevel,
            Organisation = _organisation,
            AgeRange = _ageRanges,
            SuitableFor = _suitableFor,
            AccessibleTransportLink = _accessibleTransportLink,
            AdditionalInformation = _additionalInformation,
            Volunteering = _volunteering,
            VolunteeringText = _volunteeringText,
            AdditionalDocuments = _additionalDocuments,
            DonationsText = _donationsText,
            DonationsUrl = _donationsUrl,
            SubCategories = _subCategories

        };
    }

    public ContentfulGroupBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulGroupBuilder MetaDescription(string metaDescription)
    {
        _metaDescription = metaDescription;
        return this;
    }

    public ContentfulGroupBuilder MapPosition(MapPosition mapPosition)
    {
        _mapPosition = mapPosition;
        return this;
    }

    public ContentfulGroupBuilder CategoriesReference(List<ContentfulGroupCategory> categoriesReference)
    {
        _categoriesReference = categoriesReference;
        return this;
    }

    public ContentfulGroupBuilder GroupAdministrators(GroupAdministrators groupAdministrators)
    {
        _groupAdministrators = groupAdministrators;
        return this;
    }

    public ContentfulGroupBuilder DateHiddenFrom(DateTime dateHiddenFrom)
    {
        _dateHiddenFrom = dateHiddenFrom;
        return this;
    }

    public ContentfulGroupBuilder DateHiddenTo(DateTime dateHiddenTo)
    {
        _dateHiddenTo = dateHiddenTo;
        return this;
    }

    public ContentfulGroupBuilder Cost(List<string> cost)
    {
        _cost = cost;
        return this;
    }

    public ContentfulGroupBuilder CostText(string costText)
    {
        _costText = costText;
        return this;
    }

    public ContentfulGroupBuilder AbilityLevel(string abilityLevel)
    {
        _abilityLevel = abilityLevel;
        return this;
    }
}
