namespace StockportContentApi.ContentfulFactories;

public class ProfileContentfulFactory : IContentfulFactory<ContentfulProfile, Profile>
{
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulTrivia, Trivia> _triviaFactory;
    private readonly IContentfulFactory<ContentfulInlineQuote, InlineQuote> _inlineQuoteContentfulFactory;
    private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory;

    public ProfileContentfulFactory(
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulTrivia, Trivia> triviaFactory,
        IContentfulFactory<ContentfulInlineQuote, InlineQuote> inlineQuoteContentfulFactory,
        IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory)
    {
        _crumbFactory = crumbFactory;
        _alertFactory = alertFactory;
        _triviaFactory = triviaFactory;
        _inlineQuoteContentfulFactory = inlineQuoteContentfulFactory;
        _eventBannerFactory = eventBannerFactory;
    }

    public Profile ToModel(ContentfulProfile entry)
    {
        if(entry is null)
            return null;
        
        return new()
        {
            Alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys))
                                 .Select(alert => _alertFactory.ToModel(alert)).ToList(),
            Author = entry.Author,
            Body = entry.Body,
            Breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                .Select(crumb => _crumbFactory.ToModel(crumb)).ToList(),
            Image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ?
                            entry.Image.File.Url : string.Empty,
            ImageCaption = entry.ImageCaption,
            InlineQuotes = entry.InlineQuotes.Select(quote => _inlineQuoteContentfulFactory.ToModel(quote)).ToList(),
            Quote = entry.Quote,
            Slug = entry.Slug,
            Subject = entry.Subject,
            Subtitle = entry.Subtitle,
            Title = entry.Title,
            TriviaSection = entry.TriviaSection.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                                .Select(fact => _triviaFactory.ToModel(fact)).ToList(),
            TriviaSubheading = !string.IsNullOrEmpty(entry.TriviaSubheading)
                                ? entry.TriviaSubheading
                                : string.Empty,
            EventsBanner = _eventBannerFactory.ToModel(entry.EventsBanner),
            Colour = entry.Colour
        };
    }
}