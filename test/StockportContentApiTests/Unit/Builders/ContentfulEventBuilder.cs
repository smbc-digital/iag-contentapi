namespace StockportContentApiTests.Unit.Builders;

public class ContentfulEventBuilder
{
    private string _slug = "slug";
    private readonly string _title = "title";
    private readonly string _teaser = "teaser";
    private readonly Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
    private readonly Asset _thumbnailImage = new ContentfulAssetBuilder().Url("thumbnailImage-url.jpg").Build();
    private readonly string _description = "description";
    private readonly string _fee = "fee";
    private readonly string _location = "location";
    private readonly string _submittedby = "submittedBy";
    private DateTime _eventDate = new(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
    private string _startTime = "10:00";
    private readonly string _endTime = "17:00";
    private int _occurrences = -1;
    private EventFrequency _eventFrequency = EventFrequency.None;
    private readonly List<Asset> _documents = new() { new ContentfulDocumentBuilder().Build() };
    private List<string> _categories = new() { "category 1", "category 2" };
    private List<ContentfulEventCategory> _eventCategories = new() { new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" }, new ContentfulEventCategory { Name = "Event Category", Slug = "event-category" } };
    private MapPosition _mapPosition = new() { Lat = 53.5, Lon = -2.5 };
    private readonly string _bookingInformation = "booking information";
    private bool _featured = false;
    public SystemProperties _sys = new();
    private List<string> _tags = new() { "tag 1", "tag 2" };
    private readonly List<ContentfulAlert> _alerts = new()
    {
        new ContentfulAlertBuilder().Build()};

    public ContentfulEvent Build()
        => new()
        {
            Title = _title,
            Slug = _slug,
            Teaser = _teaser,
            Image = _image,
            ThumbnailImage = _thumbnailImage,
            Description = _description,
            Fee = _fee,
            Location = _location,
            SubmittedBy = _submittedby,
            EventDate = _eventDate,
            StartTime = _startTime,
            EndTime = _endTime,
            Occurrences = _occurrences,
            Frequency = _eventFrequency,
            Documents = _documents,
            MapPosition = _mapPosition,
            BookingInformation = _bookingInformation,
            Featured = _featured,
            Sys = _sys,
            Tags = _tags,
            Alerts = _alerts,
            EventCategories = _eventCategories
        };

    public ContentfulEventBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulEventBuilder Occurrences(int occurrences)
    {
        _occurrences = occurrences;
        return this;
    }

    public ContentfulEventBuilder Frequency(EventFrequency frequency)
    {
        _eventFrequency = frequency;
        return this;
    }

    public ContentfulEventBuilder EventDate(DateTime eventDate)
    {
        _eventDate = eventDate;
        return this;
    }

    public ContentfulEventBuilder EventCategory(List<string> categoriesList)
    {
        _categories = categoriesList;
        return this;
    }

    public ContentfulEventBuilder EventCategoryList(List<ContentfulEventCategory> categoriesList)
    {
        _eventCategories = categoriesList;
        return this;
    }

    public ContentfulEventBuilder MapPosition(MapPosition mapPosition)
    {
        _mapPosition = mapPosition;
        return this;
    }

    public ContentfulEventBuilder Featured(bool featured)
    {
        _featured = featured;
        return this;
    }

    public ContentfulEventBuilder UpdatedAt(DateTime updatedAt)
    {
        _sys.UpdatedAt = updatedAt;
        return this;
    }

    public ContentfulEventBuilder Tags(List<string> tags)
    {
        _tags = tags;
        return this;
    }

    public ContentfulEventBuilder StartTime(string startTime)
    {
        _startTime = startTime;
        return this;
    }
}