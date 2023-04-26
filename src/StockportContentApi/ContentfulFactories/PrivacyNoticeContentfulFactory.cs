namespace StockportContentApi.ContentfulFactories;

public class PrivacyNoticeContentfulFactory : IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>
{
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly IContentfulFactory<ContentfulPrivacyNotice, Topic> _parentTopicFactory;
    private readonly ILogger<PrivacyNoticeContentfulFactory> _logger;

    public PrivacyNoticeContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory, IContentfulFactory<ContentfulPrivacyNotice, Topic> parentTopicFactory, ILogger<PrivacyNoticeContentfulFactory> logger)
    {
        _crumbFactory = crumbFactory;
        _parentTopicFactory = parentTopicFactory;
        _logger = logger;
    }

    public PrivacyNotice ToModel(ContentfulPrivacyNotice entry)
    {
        var breadcrumbs = new List<Crumb>();
        try
        {
            breadcrumbs = entry.Breadcrumbs
                .Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Could not get breadcrumbs for Privacy Notice: {ex.Message}");
        }

        var topic = _parentTopicFactory.ToModel(entry) ?? new NullTopic();

        var privacyNotice = new PrivacyNotice(entry.Slug, entry.Title, entry.Category, entry.OutsideEu, entry.AutomatedDecision, entry.Purpose, entry.TypeOfData, entry.Legislation, entry.Obtained, entry.ExternallyShared, entry.RetentionPeriod, entry.UrlOne, entry.UrlTwo, entry.UrlThree, breadcrumbs, topic);

        return privacyNotice;
    }
}
