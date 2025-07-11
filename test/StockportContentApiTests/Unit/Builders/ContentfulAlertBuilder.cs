﻿namespace StockportContentApiTests.Unit.Builders;

public class ContentfulAlertBuilder
{
    private string _title = "title";
    private string _body = "body";
    private string _severity = "severity";
    private DateTime _sunriseDate = new(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private DateTime _sunsetDate = new(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
    private string _slug = "slug";

    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } },
        Id = "id"
    };

    public ContentfulAlert Build()
        => new()
        {
            Title = _title,
            Body = _body,
            Severity = _severity,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Sys = _sys,
            Slug = _slug
        };
    
    public ContentfulAlertBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulAlertBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulAlertBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    public ContentfulAlertBuilder WithSeverity(string severity)
    {
        _severity = severity;
        return this;
    }

    public ContentfulAlertBuilder WithSunriseDate(DateTime sunriseDate)
    {
        _sunriseDate = sunriseDate;
        return this;
    }

    public ContentfulAlertBuilder WithSunsetDate(DateTime sunsetDate)
    {
        _sunsetDate = sunsetDate;
        return this;
    }
}