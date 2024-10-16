namespace StockportContentApiTests.Unit.Builders;

public class GroupBuilder
{
    private string _name = "_name";
    private string _slug = "_slug";
    private string _metaDescription = "_metaDescription";
    private readonly string _phoneNumber = "_phoneNumber";
    private readonly string _email = "_email";
    private readonly string _website = "_website";
    private readonly string _twitter = "_twitter";
    private readonly string _facebook = "_facebook";
    private readonly string _address = "_address";
    private readonly string _description = "_description";
    private readonly string _image = "image-url.jpg";
    private readonly string _thumbnail = "thumbnail.jpg";
    private DateTime? _dateHiddenFrom = null;
    private DateTime? _dateHiddenTo = null;
    private List<GroupCategory> _categoriesReference = new();
    private readonly List<GroupSubCategory> _subCategories = new();
    private List<Event> _events = new();
    private readonly List<Crumb> _crumbs = new() { new Crumb("slug", "title", "type") };
    private MapPosition _mapPosition = new() { Lat = 39.0, Lon = 2.0 };
    private bool _volunteering = false;
    private GroupAdministrators _groupAdministrators = new();
    private List<string> _cost = new() { "_cost" };
    private string _costText = "_costText";
    private string _abilityLevel = "_abilityLevel";
    private Organisation _organisation = new();
    private readonly string _additionalInformation = "additional inforamtion";
    private List<Document> _additionalDocuments = new();
    private readonly List<string> _suitableFor = new();
    private readonly List<string> _ageRanges = new();
    private readonly DateTime? _dateLastModified = new DateTime?();
    private string _donationsText = "_donationsText";
    private string _donationsUrl = "_donationsUrl";
    private List<GroupBranding> _groupBranding = new();
    private readonly List<Alert> _alerts = new()
    {
        new Alert("title",
            "subHeading",
            "body",
            "severity",
            new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc),
            "slug", false, string.Empty)
    };

    private readonly List<Alert> _alertsInline = new()
    {
        new Alert("title",
            "subHeading",
            "body",
            "severity",
            new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc),
            "slug", false, string.Empty)
    };

    public Group Build()
        => new(_name,
            _slug,
            _metaDescription,
            _phoneNumber,
            _email,
            _website,
            _twitter,
            _facebook,
            _address,
            _description,
            _image,
            _thumbnail,
            _categoriesReference,
            _subCategories,
            _crumbs,
            _mapPosition,
            _volunteering,
            _groupAdministrators,
            _dateHiddenFrom,
            _dateHiddenTo,
            "published",
            new List<string>(),
            string.Empty,
            string.Empty,
            string.Empty,
            _organisation,
            false,
            string.Empty,
            _groupBranding,
            new List<string>(),
            _additionalInformation,
            _additionalDocuments,
            _dateLastModified,
            _suitableFor,
            _ageRanges,
            _donationsText,
            _donationsUrl,
            _alerts,
            _alertsInline);

    public GroupBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public GroupBuilder MetaDescription(string metaDescription)
    {
        _metaDescription = metaDescription;
        return this;
    }

    public GroupBuilder Organisation(Organisation org)
    {
        _organisation = org;
        return this;
    }

    public GroupBuilder CategoriesReference(List<GroupCategory> categoriesReference)
    {
        _categoriesReference = categoriesReference;
        return this;
    }

    public GroupBuilder MapPosition(MapPosition mapPosition)
    {
        _mapPosition = mapPosition;
        return this;
    }

    public GroupBuilder Volunteering(bool volunteering)
    {
        _volunteering = volunteering;
        return this;
    }

    public GroupBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public GroupBuilder Events(List<Event> events)
    {
        _events = events;
        return this;
    }

    public GroupBuilder GroupAdministrators(GroupAdministrators groupAdministrators)
    {
        _groupAdministrators = groupAdministrators;
        return this;
    }

    public GroupBuilder DateHiddenFrom(DateTime dateHiddenFrom)
    {
        _dateHiddenFrom = dateHiddenFrom;
        return this;
    }

    public GroupBuilder DateHiddenTo(DateTime dateHiddenTo)
    {
        _dateHiddenTo = dateHiddenTo;
        return this;
    }

    public GroupBuilder Cost(List<string> cost)
    {
        _cost = cost;
        return this;
    }

    public GroupBuilder CostText(string costText)
    {
        _costText = costText;
        return this;
    }

    public GroupBuilder AbilityLevel(string abilityLevel)
    {
        _abilityLevel = abilityLevel;
        return this;
    }

    public GroupBuilder AdditionalDocuments(List<Document> additionalDocuments)
    {
        _additionalDocuments = additionalDocuments;
        return this;
    }

    public GroupBuilder DonationsText(string donationsText)
    {
        _donationsText = donationsText;
        return this;
    }
    public GroupBuilder DonationsUrl(string donationsUrl)
    {
        _donationsUrl = donationsUrl;
        return this;

    }

    public GroupBuilder GroupBranding(List<GroupBranding> groupBranding)
    {
        _groupBranding = groupBranding;
        return this;
    }
}