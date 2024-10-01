namespace StockportContentApi.ContentfulFactories.NewsFactories;

public class NewsContentfulFactory : IContentfulFactory<ContentfulNews, News>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IContentfulFactory<Asset, Document> _documentFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;

    public NewsContentfulFactory(IVideoRepository videoRepository, IContentfulFactory<Asset, Document> documentFactory, IContentfulFactory<ContentfulAlert, Alert> alertFactory, ITimeProvider timeProvider, IContentfulFactory<ContentfulProfile, Profile> profileFactory)
    {
        _videoRepository = videoRepository;
        _documentFactory = documentFactory;
        _alertFactory = alertFactory;
        _dateComparer = new DateComparer(timeProvider);
        _profileFactory = profileFactory;
    }

    public News ToModel(ContentfulNews entry)
    {
        List<Document> documents = entry.Documents
            .Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
            .Select(document => _documentFactory.ToModel(document))
            .ToList();

        string imageUrl = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ?
            entry.Image.File.Url : string.Empty;

        IEnumerable<Alert> alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                  && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(alert => _alertFactory.ToModel(alert));

        DateTime? updatedAt = entry.Sys.UpdatedAt is not null ? entry.Sys.UpdatedAt : entry.SunriseDate;

        List<Profile> profiles = entry.Profiles.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                     .Select(profile => _profileFactory.ToModel(profile)).ToList();

        return new News(entry.Title, entry.Slug, entry.Teaser, entry.Purpose, imageUrl, ImageConverter.ConvertToThumbnail(imageUrl),
            _videoRepository.Process(entry.Body), entry.SunriseDate, entry.SunsetDate, entry.Sys.UpdatedAt.Value, new List<Crumb> { new("News", string.Empty, "news") },
            alerts.ToList(), entry.Tags, documents, entry.Categories, profiles);
    }
}