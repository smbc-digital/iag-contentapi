namespace StockportContentApi.Model;

public class News
{
    public string Title { get; }
    public string Slug { get; }
    public string Teaser { get; }
    public string Purpose { get; set; }
    public string Image { get; }
    public string ThumbnailImage { get; }
    public string Body { get; }
    public DateTime SunriseDate { get; }
    public DateTime SunsetDate { get; }
    public DateTime UpdatedAt { get; }
    public List<Crumb> Breadcrumbs { get; }
    public List<string> Tags { get; set; }
    public List<Alert> Alerts { get; }
    public List<Document> Documents { get; }
    public List<string> Categories { get; }

    public News(string title, string slug, string teaser, string purpose, string image, string thumbnailImage, string body, DateTime sunriseDate, DateTime sunsetDate, DateTime updatedAt, List<Crumb> breadcrumbs, List<Alert> alerts, List<string> tags, List<Document> documents, List<string> categories)
    {
        Title = title;
        Slug = slug;
        Teaser = teaser;
        Purpose = purpose;
        Image = image;
        ThumbnailImage = thumbnailImage;
        Body = body;
        SunriseDate = sunriseDate;
        SunsetDate = sunsetDate;
        UpdatedAt = updatedAt;
        Breadcrumbs = breadcrumbs;
        Alerts = alerts;
        Tags = tags;
        Documents = documents;
        Categories = categories;
    }
}