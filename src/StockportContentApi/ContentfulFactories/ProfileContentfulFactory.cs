namespace StockportContentApi.ContentfulFactories;

public class ProfileContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
                                    IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                    IContentfulFactory<ContentfulTrivia, Trivia> triviaFactory,
                                    IContentfulFactory<ContentfulInlineQuote, InlineQuote> inlineQuoteContentfulFactory,
                                    IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory,
                                    IContentfulFactory<ContentfulProfile, Topic> parentTopicFactory) : IContentfulFactory<ContentfulProfile, Profile>
{
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory = crumbFactory;
    private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory = eventBannerFactory;
    private readonly IContentfulFactory<ContentfulInlineQuote, InlineQuote> _inlineQuoteContentfulFactory = inlineQuoteContentfulFactory;
    private readonly IContentfulFactory<ContentfulProfile, Topic> _parentTopicFactory = parentTopicFactory;
    private readonly IContentfulFactory<ContentfulTrivia, Trivia> _triviaFactory = triviaFactory;

    public Profile ToModel(ContentfulProfile entry)
    {
        if (entry is null)
            return null;

        MediaAsset image = new();
        if (entry.Image is not null && entry.Image.File is not null)
        {
            image = new()
            {
                Url = entry.Image.File.Url,
                Description = entry.Image.Description
            };
        }

        return new()
        {
            Alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys))
                        .Where(alert => !alert.Severity.Equals("Condolence"))
                        .Select(_alertFactory.ToModel).ToList(),
            
            Body = entry.Body,
            Breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                            .Select(_crumbFactory.ToModel).ToList(),

            Image = image,    
            ImageCaption = entry.ImageCaption,
            InlineQuotes = entry.InlineQuotes.Select(_inlineQuoteContentfulFactory.ToModel).ToList(),
            
            Teaser = entry.Teaser,
            Slug = entry.Slug,
            Subtitle = entry.Subtitle,
            Title = entry.Title,
            
            TriviaSection = entry.TriviaSection.Where(fact => ContentfulHelpers.EntryIsNotALink(fact.Sys))
                            .Select(_triviaFactory.ToModel).ToList(),
            
            TriviaSubheading = !string.IsNullOrEmpty(entry.TriviaSubheading)
                ? entry.TriviaSubheading
                : string.Empty,

            EventsBanner = _eventBannerFactory.ToModel(entry.EventsBanner),
            Colour = entry.Colour,
            
            InlineAlerts = entry.InlineAlerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys))
                            .Where(alert => !alert.Severity.Equals("Condolence"))
                            .Select(_alertFactory.ToModel).ToList(),
            
            ParentTopic = _parentTopicFactory.ToModel(entry) ?? new NullTopic()
        };
    }
}