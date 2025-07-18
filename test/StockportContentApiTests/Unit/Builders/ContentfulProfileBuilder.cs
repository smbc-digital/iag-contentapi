﻿namespace StockportContentApiTests.Unit.Builders;

public class ContentfulProfileBuilder
{
    private string _slug = "slug";
    private readonly List<ContentfulAlert> _alerts = new() { new ContentfulAlertBuilder().Build() };
    private readonly SystemProperties _sys = new() { ContentType = new() { SystemProperties = new() { Id = "id" } } };

    public ContentfulProfile Build()
        => new()
        {
            Title = "title",
            Slug = _slug,
            Subtitle = "subtitle",
            Teaser = "teaser",
            Image = new ContentfulAssetBuilder().Url("image-url.jpg").Build(),
            Body = "body",
            Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().Build() },
            Sys = _sys,
            Alerts = _alerts,
            Author = "author",
            Subject = "subject",
            TriviaSection = new List<ContentfulTrivia>() {
                new() {
                    Title = "trivia title",
                    Icon = "trivia icon",
                    Link = "trivia link",
                    Statistic = "statistic",
                    StatisticSubHeading = "statistic subheading",
                    Sys = new SystemProperties(),
                    Body = "trivia text"
                }
            },
            TriviaSubheading = "trivia heading",
            InlineQuotes = new List<ContentfulInlineQuote>(){
                new(){
                    Author = "inline quote author",
                    Image = new ContentfulAssetBuilder().Url("inline-quote.jpg").Build(),
                    ImageAltText = "inline quote image alt text",
                    Quote = "inline quote",
                    Slug = "inline-quote-url"
                }
            }
        };

    public ContentfulProfileBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }
}