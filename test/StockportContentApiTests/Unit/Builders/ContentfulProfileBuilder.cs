﻿namespace StockportContentApiTests.Unit.Builders;

public class ContentfulProfileBuilder
{
    private string _slug = "slug";
    private readonly List<ContentfulAlert> _alerts = new List<ContentfulAlert> { new ContentfulAlertBuilder().Build() };
    private readonly SystemProperties _sys = new SystemProperties
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }

    };

    public ContentfulProfile Build()
    {
        return new ContentfulProfile
        {
            Title = "title",
            Slug = _slug,
            Subtitle = "subtitle",
            Quote = "quote",
            Image = new ContentfulAssetBuilder().Url("image-url.jpg").Build(),
            Body = "body",
            Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().Build() },
            Sys = _sys,
            Alerts = _alerts,
            Author = "author",
            Subject = "subject",
            TriviaSection = new List<ContentfulTrivia>(),
            TriviaSubheading = "trivia heading"
        };
    }

    public ContentfulProfileBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }
}
