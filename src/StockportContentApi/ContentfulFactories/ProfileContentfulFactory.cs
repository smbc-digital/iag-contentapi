namespace StockportContentApi.ContentfulFactories;

public class ProfileContentfulFactory : IContentfulFactory<ContentfulProfile, Profile>
{
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulReference, Trivia> _triviaFactory;
    private readonly IContentfulFactory<ContentfulInlineQuote, InlineQuote> _inlineQuoteContentfulFactory;
    private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory;
    private readonly IContentfulFactory<ContentfulProfile, Topic> _parentTopicFactory;

    public ProfileContentfulFactory(
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulReference, Trivia> triviaFactory,
        IContentfulFactory<ContentfulInlineQuote, InlineQuote> inlineQuoteContentfulFactory,
        IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory,
        IContentfulFactory<ContentfulProfile, Topic> parentTopicFactory)
    {
        _crumbFactory = crumbFactory;
        _alertFactory = alertFactory;
        _triviaFactory = triviaFactory;
        _inlineQuoteContentfulFactory = inlineQuoteContentfulFactory;
        _eventBannerFactory = eventBannerFactory;
        _parentTopicFactory = parentTopicFactory;
    }

    public Profile ToModel(ContentfulProfile entry)
    {
        if(entry is null)
            return null;

        MediaAsset image = new();
        if (entry.Image is not null && entry.Image.File is not null)
        {
            image = new MediaAsset
            {
                Url = entry.Image.File.Url,
                Description = entry.Image.Description
            };
        }

        return new()
        {
            Alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(alert => _alertFactory.ToModel(alert)).ToList(),
            Author = entry.Author,
            Body = entry.Body,
            Breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                .Select(crumb => _crumbFactory.ToModel(crumb)).ToList(),
            Image = image,
            ImageCaption = entry.ImageCaption,
            InlineQuotes = entry.InlineQuotes.Select(quote => _inlineQuoteContentfulFactory.ToModel(quote)).ToList(),
            Teaser = entry.Teaser,
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
            Colour = entry.Colour,
            InlineAlerts = entry.InlineAlerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(alert => _alertFactory.ToModel(alert)).ToList(),
            ParentTopic = _parentTopicFactory.ToModel(entry) ?? new NullTopic()
        };
    }
}