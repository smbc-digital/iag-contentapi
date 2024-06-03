namespace StockportContentApi.ContentfulFactories.ArticleFactories;

public class ArticleContentfulFactory : IContentfulFactory<ContentfulArticle, Article>
{
    private readonly IContentfulFactory<ContentfulSection, Section> _sectionFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
    private readonly IContentfulFactory<ContentfulArticle, Topic> _parentTopicFactory;
    private readonly IContentfulFactory<Asset, Document> _documentFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IVideoRepository _videoRepository;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;


    public ArticleContentfulFactory(IContentfulFactory<ContentfulSection, Section> sectionFactory,
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        IContentfulFactory<ContentfulProfile, Profile> profileFactory,
        IContentfulFactory<ContentfulArticle, Topic> parentTopicFactory,
        IContentfulFactory<Asset, Document> documentFactory,
        IVideoRepository videoRepository,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulReference, SubItem> subitemFactory)
    {
        _sectionFactory = sectionFactory;
        _crumbFactory = crumbFactory;
        _profileFactory = profileFactory;
        _documentFactory = documentFactory;
        _videoRepository = videoRepository;
        _parentTopicFactory = parentTopicFactory;
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
        _subitemFactory = subitemFactory;
    }

    public Article ToModel(ContentfulArticle entry)
    {
        var sectionUpdatedAt = entry.Sections
            .Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
            .Where(section => section.Sys.UpdatedAt is not null)
            .Select(section => section.Sys.UpdatedAt.Value)
            .OrderByDescending(section => section)
            .FirstOrDefault();

        return new(){
            Body = !string.IsNullOrEmpty(entry.Body) ? _videoRepository.Process(entry.Body) : string.Empty,
            Slug = entry.Slug,
            Title = entry.Title,
            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            Icon = entry.Icon,
            BackgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                                ? entry.BackgroundImage.File.Url : string.Empty,
            Image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                        ? entry.Image.File.Url : string.Empty,
            Sections = entry.Sections.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                            .Select(section => _sectionFactory.ToModel(section)).ToList(),
            Breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                            .Select(crumb => _crumbFactory.ToModel(crumb)).ToList(),
            Alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                        && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                        .Select(alert => _alertFactory.ToModel(alert)),
            Profiles = entry.Profiles.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                            .Select(profile => _profileFactory.ToModel(profile)).ToList(),
            ParentTopic = _parentTopicFactory.ToModel(entry) ?? new NullTopic(),
            Documents = entry.Documents.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                            .Select(document => _documentFactory.ToModel(document)).ToList(),
            RelatedContent = entry.RelatedContent.Where(rc => ContentfulHelpers.EntryIsNotALink(rc.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(rc.SunriseDate, rc.SunsetDate))
                            .Select(item => _subitemFactory.ToModel(item)).ToList(),
            SunriseDate = entry.SunriseDate,
            SunsetDate = entry.SunsetDate,
            AlertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                .Select(alertInline => _alertFactory.ToModel(alertInline)),
            UpdatedAt = sectionUpdatedAt > entry.Sys.UpdatedAt.Value ? sectionUpdatedAt : entry.Sys.UpdatedAt.Value,
            HideLastUpdated = entry.HideLastUpdated
        };
    }
}