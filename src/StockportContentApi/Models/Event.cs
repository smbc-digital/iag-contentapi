namespace StockportContentApi.Models;
[ExcludeFromCodeCoverage]
public class Event
{
    public string Title { get; }
    public string Slug { get; }
    public string Teaser { get; }
    public string ImageUrl { get; }
    public string ThumbnailImageUrl { get; }
    public string Description { get; }
    public string Fee { get; }
    public string Location { get; }
    public string SubmittedBy { get; set; }
    public DateTime EventDate { get; }
    public string StartTime { get; }
    public string EndTime { get; }
    public int Occurences { get; }
    public EventFrequency EventFrequency { get; set; }
    public List<Crumb> Breadcrumbs { get; }
    public List<Document> Documents { get; }
    public List<string> Categories { get; }
    public MapPosition MapPosition { get; }
    public string BookingInformation { get; }
    public bool Featured { get; }
    public DateTime? UpdatedAt { get; }
    public List<string> Tags { get; }
    public Group Group { get; set; }
    public List<Alert> Alerts { get; }
    public List<EventCategory> EventCategories { get; }
    public bool? Free { get; }
    public bool? Paid { get; }
    public GeoCoordinate Coord { get; }
    public string AccessibleTransportLink { get; }
    public List<GroupBranding> EventBranding { get; set; } = new();
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string MetaDescription { get; }
    public string Duration { get; set; }
    public string Languages { get; set; }
    public List<Event> RelatedEvents { get; set; }

    public Event(string title,
                string slug,
                string teaser,
                string imageUrl,
                string description,
                string fee,
                string location,
                string submittedBy,
                DateTime eventDate,
                string startTime,
                string endTime,
                int occurences,
                EventFrequency frequency,
                List<Crumb> breadcrumbs,
                string thumbnailImageUrl,
                List<Document> documents,
                List<string> categories,
                MapPosition mapPosition,
                bool featured,
                string bookingInformation,
                DateTime? updatedAt,
                List<string> tags,
                Group group,
                List<Alert> alerts,
                List<EventCategory> eventCategories,
                bool? free,
                bool? paid,
                string accessibleTransportLink,
                List<GroupBranding> eventBranding,
                string phoneNumber,
                string email,
                string website,
                string metaDescription,
                string duration,
                string languages)
    {
        Title = title;
        Slug = slug;
        Teaser = teaser;
        Description = description;
        Fee = fee;
        Location = location;
        SubmittedBy = submittedBy;
        EventDate = eventDate;
        StartTime = startTime;
        EndTime = endTime;
        Occurences = occurences;
        EventFrequency = frequency;
        Breadcrumbs = breadcrumbs;
        ThumbnailImageUrl = thumbnailImageUrl;
        ImageUrl = imageUrl;
        Documents = documents;
        Categories = categories.Select(s => s).ToList();
        MapPosition = mapPosition;
        BookingInformation = bookingInformation;
        Featured = featured;
        UpdatedAt = updatedAt;
        Tags = tags.Select(s => s.ToLower()).ToList();
        Group = group;
        Alerts = alerts;
        EventCategories = eventCategories;
        Paid = paid;
        Free = free;
        Coord = MapPosition is null 
            ? null 
            : new GeoCoordinate(MapPosition.Lat, MapPosition.Lon);
        AccessibleTransportLink = accessibleTransportLink;
        EventBranding = eventBranding;
        PhoneNumber = phoneNumber;
        Email = email;
        Website = website;
        MetaDescription = metaDescription;
        Duration = duration;
        Languages = languages;
    }
}