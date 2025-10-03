namespace StockportContentApiTests.Unit.Builders;

public class ContentfulEventBuilder
{
    private readonly List<ContentfulEventCategory> _eventCategories = new()
    {
        new ContentfulEventCategory { Name = "Category 2", Slug = "category-2" },
        new ContentfulEventCategory { Name = "Event Category", Slug = "event-category" }
    };

    private readonly List<ContentfulAlert> _alerts = new()
    {
        new ContentfulAlertBuilder().Build()
    };

    public ContentfulEvent Build()
        => new()
        {
            Title = "title",
            Slug = "slug",
            Teaser = "teaser",
            Image = new ContentfulAssetBuilder().Url("image-url.jpg").Build(),
            ThumbnailImage = new ContentfulAssetBuilder().Url("thumbnailImage-url.jpg").Build(),
            Description = "description",
            Fee = "fee",
            Location = "location",
            SubmittedBy = "submittedBy",
            EventDate = DateTime.MaxValue,
            StartTime = "10:00",
            EndTime = "17:00",
            Occurrences = -1,
            Frequency = EventFrequency.None,
            Documents = new() { new ContentfulDocumentBuilder().Build() },
            MapPosition = new() { Lat = 53.5, Lon = -2.5 },
            BookingInformation = "booking information",
            Featured = false,
            Sys = new(),
            Tags = new() { "tag 1", "tag 2" },
            Alerts = _alerts,
            EventCategories = _eventCategories
        };
}