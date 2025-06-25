namespace StockportContentApi.ContentfulFactories;

public class PrivacyNoticeContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
                                            IContentfulFactory<ContentfulPrivacyNotice, Topic> parentTopicFactory) : IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>
{
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory = crumbFactory;
    private readonly IContentfulFactory<ContentfulPrivacyNotice, Topic> _parentTopicFactory = parentTopicFactory;

    public PrivacyNotice ToModel(ContentfulPrivacyNotice entry)
    {
        if (entry is null)
            return null;

        List<Crumb> breadcrumbs = entry.Breadcrumbs
                                    .Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                    .Select(_crumbFactory.ToModel).ToList();

        Topic topic = _parentTopicFactory.ToModel(entry) ?? new NullTopic();

        PrivacyNotice privacyNotice = new()
        {
            Slug = entry.Slug,
            Title = entry.Title,
            Category = entry.Category,
            OutsideEu = entry.OutsideEu,
            AutomatedDecision = entry.AutomatedDecision,
            Purpose = entry.Purpose,
            TypeOfData = entry.TypeOfData,
            Legislation = entry.Legislation,
            Obtained = entry.Obtained,
            ExternallyShared = entry.ExternallyShared,
            RetentionPeriod = entry.RetentionPeriod,
            Breadcrumbs = breadcrumbs,
            ParentTopic = topic
        };

        return privacyNotice;
    }
}