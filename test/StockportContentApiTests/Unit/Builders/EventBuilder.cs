namespace StockportContentApiTests.Unit.Builders;

public class EventBuilder
{
    private string _slug = "slug";
    private readonly string _title = "title";
    private readonly string _teaser = "teaser";
    private readonly string _image = "image-url.jpg";
    private readonly string _thumbnailImage = "thumb-image-url.jpg";
    private readonly string _description = "description";
    private readonly string _fee = "fee";
    private readonly string _location = "location";
    private readonly string _submittedby = "submittedBy";
    private readonly string _metaDescription = "metaDescription";
    private DateTime _eventDate = new(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
    private readonly string _startTime = "10:00";
    private readonly string _endTime = "17:00";
    private int _occurences = -1;
    private EventFrequency _eventFrequency = EventFrequency.None;
    private readonly List<Crumb> _breadcrumbs = new() { new Crumb("Events", string.Empty, "events") };
    private readonly List<Document> _documents = new() { new DocumentBuilder().Build() };
    private List<string> _categories = new() { "Category 1", "Category 2" };
    private readonly MapPosition _mapPosition = new() { Lat = 53.47, Lon = -2.2 };
    private string _bookingInformation = "booking information";
    private bool _featured = true;
    private readonly DateTime _updatedAt = new(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
    private List<string> _tags = new();
    private readonly Group _group = null;
    private readonly List<Alert> _alerts = new()
    { new Alert("title", "subHeading", "body",
                                                             "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                                             new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc), "slug", false, string.Empty) };
    private List<EventCategory> _eventCategories = new();

    public Event Build()
    {
        return new Event(_title, _slug, _teaser, _image, _description, _fee, _location, _submittedby,
            _eventDate, _startTime, _endTime, _occurences, _eventFrequency, _breadcrumbs, _thumbnailImage,
            _documents, _categories, _mapPosition, _featured, _bookingInformation, _updatedAt, _tags, _group, _alerts, _eventCategories, null, null, null, _metaDescription);
    }

    public EventBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public EventBuilder Occurrences(int occurrences)
    {
        _occurences = occurrences;
        return this;
    }

    public EventBuilder Frequency(EventFrequency frequency)
    {
        _eventFrequency = frequency;
        return this;
    }

    public EventBuilder EventDate(DateTime eventDate)
    {
        _eventDate = eventDate;
        return this;
    }

    public EventBuilder EventCategory(List<string> categoriesList)
    {
        _categories = categoriesList;
        return this;
    }

    public EventBuilder EventCategories(List<EventCategory> categoriesList)
    {
        _eventCategories = categoriesList;
        return this;
    }

    public EventBuilder EventBookingInformation(string bookingInformation)
    {
        _bookingInformation = bookingInformation;
        return this;
    }

    public EventBuilder Featured(bool featured)
    {
        _featured = featured;
        return this;
    }

    public EventBuilder Tags(List<string> tags)
    {
        _tags = tags;
        return this;
    }
}
