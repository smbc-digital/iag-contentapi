namespace StockportContentApiTests.Unit.Builders;

public class GroupBuilder
{
    private string _name = "_name";
    private string _slug = "_slug";
    private string _metaDescription = "_metaDescription";
    private string _phoneNumber = "_phoneNumber";
    private string _email = "_email";
    private string _website = "_website";
    private string _twitter = "_twitter";
    private string _facebook = "_facebook";
    private string _address = "_address";
    private string _description = "_description";
    private string _image = "image-url.jpg";
    private string _thumbnail = "thumbnail.jpg";
    private DateTime? _dateHiddenFrom = null;
    private DateTime? _dateHiddenTo = null;
    private List<GroupCategory> _categoriesReference = new List<GroupCategory>();
    private List<GroupSubCategory> _subCategories = new List<GroupSubCategory>();
    private List<Event> _events = new List<Event>();
    private List<Crumb> _crumbs = new List<Crumb> { new Crumb("slug", "title", "type") };
    private MapPosition _mapPosition = new MapPosition() { Lat = 39.0, Lon = 2.0 };
    private bool _volunteering = false;
    private GroupAdministrators _groupAdministrators = new GroupAdministrators();
    private List<string> _cost = new List<string> { "_cost" };
    private string _costText = "_costText";
    private string _abilityLevel = "_abilityLevel";
    private Organisation _organisation = new Organisation();
    private string _additionalInformation = "additional inforamtion";
    private List<Document> _additionalDocuments = new List<Document>();
    private List<string> _suitableFor = new List<string>();
    private List<string> _ageRanges = new List<string>();
    private DateTime? _dateLastModified = new DateTime?();
    private string _donationsText = "_donationsText";
    private string _donationsUrl = "_donationsUrl";
    private List<GroupBranding> _groupBranding = new List<GroupBranding>();
    private List<Alert> _alerts = new List<Alert>
    {
        new Alert("title",
            "subHeading",
            "body",
            "severity",
            new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc),
            "slug", false, string.Empty)
    };

    private List<Alert> _alertsInline = new List<Alert>
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
    {
        return new Group(_name, _slug, _metaDescription, _phoneNumber, _email, _website, _twitter, _facebook, _address, _description,
            _image, _thumbnail, _categoriesReference, _subCategories, _crumbs, _mapPosition, _volunteering, _groupAdministrators, _dateHiddenFrom, _dateHiddenTo, "published", new List<string>(), string.Empty, string.Empty, string.Empty, _organisation, false, string.Empty, _groupBranding, new List<string>(), _additionalInformation, _additionalDocuments, _dateLastModified, _suitableFor, _ageRanges, _donationsText, _donationsUrl, _alerts, _alertsInline);
    }

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

