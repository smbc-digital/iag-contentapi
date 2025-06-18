namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class StartPage(string title,
                    string slug,
                    string teaser,
                    string summary,
                    string upperBody,
                    string formLink,
                    string lowerBody,
                    string backgroundImage,
                    string icon,
                    IEnumerable<Crumb> breadcrumbs,
                    List<Alert> alerts,
                    IEnumerable<Alert> inlineAlerts)
{
    public string Title { get; } = title;
    public string Slug { get; } = slug;
    public string Teaser { get; } = teaser;
    public string Summary { get; } = summary;
    public string UpperBody { get; } = upperBody;
    public string FormLink { get; } = formLink;
    public string LowerBody { get; } = lowerBody;
    public string BackgroundImage { get; } = backgroundImage;
    public string Icon { get; } = icon;
    public IEnumerable<Crumb> Breadcrumbs { get; } = breadcrumbs;
    public List<Alert> Alerts { get; } = alerts;
    public IEnumerable<Alert> AlertsInline { get; } = inlineAlerts;
}

[ExcludeFromCodeCoverage]
public class NullStartPage : StartPage
{
    public NullStartPage()
        : base(string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            new List<Crumb>(),
            new List<Alert>(),
            new List<Alert>())
    { }
}