namespace StockportContentApi.ContentfulFactories;

public class ContentBlockContentfulFactory : IContentfulFactory<ContentfulReference, ContentBlock>
{
    private readonly DateComparer _dateComparer;

    public ContentBlockContentfulFactory(ITimeProvider timeProvider) => _dateComparer = new DateComparer(timeProvider);

    public ContentBlock ToModel(ContentfulReference entry) =>
        new()
        {
            Slug = HandleSlugForGroupsHomepage(entry.Sys, entry.Slug),
            Title = GetEntryTitle(entry),
            Teaser = entry.Teaser,
            Icon = entry.Icon,
            Type = GetEntryType(entry),
            ContentType = entry.ContentType,
            Image = GetEntryImage(entry),
            MailingListId = entry.MailingListId,
            Body = entry.Body,
            SubItems = entry.SubItems?
                .Where(EntryIsValid)
                .Where(item => IsSubItemSuitableForContentType(entry.ContentType, GetEntryType(item)))
                .Select(item => new ContentBlock
                {
                    Slug = item.Slug,
                    Title = GetEntryTitle(item),
                    Teaser = item.Teaser,
                    Icon = item.Icon,
                    Type = GetEntryType(item),
                    ContentType = item.ContentType,
                    Image = GetEntryImage(item),
                    MailingListId = item.MailingListId,
                    Body = item.Body,
                    SubItems = new(),
                    Link = item.Link,
                    ButtonText = item.ButtonText,
                    ColourScheme = item.ColourScheme,
                    Statistic = item.Statistic,
                    StatisticSubHeading = item.StatisticSubheading,
                    VideoTitle = item.VideoTitle,
                    VideoToken = item.VideoToken,
                    VideoPlaceholderPhotoId = item.VideoPlaceholderPhotoId,
                    AssociatedTagCategory = item.AssociatedTagCategory,
                    ScreenReader = item.ScreenReader,
                    AccountName = item.AccountName

                })
                .ToList() ?? new List<ContentBlock>(),
            Link = entry.Link,
            ButtonText = entry.ButtonText,
            ColourScheme = entry.ColourScheme,
            Statistic = entry.Statistic,
            StatisticSubHeading = entry.StatisticSubheading,
            VideoTitle = entry.VideoTitle,
            VideoToken = entry.VideoToken,
            VideoPlaceholderPhotoId = entry.VideoPlaceholderPhotoId,
            AssociatedTagCategory = entry.AssociatedTagCategory
        };

    private static string HandleSlugForGroupsHomepage(SystemProperties sys, string entrySlug) =>
        sys.ContentType.SystemProperties.Id.Equals("groupHomepage") 
            ? "groups" 
            : entrySlug;

    private static string GetEntryType(ContentfulReference entry) =>
        entry.Sys.ContentType.SystemProperties.Id.Equals("startPage") 
            ? "start-page" 
            : entry.Sys.ContentType.SystemProperties.Id;

    private static string GetEntryImage(ContentfulReference entry)
    {
        string image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
            ? entry.Image.File.Url
            : string.Empty;

        if (string.IsNullOrEmpty(image) && entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties))
            image = entry.BackgroundImage.File.Url;

        return image;
    }

    private static string GetEntryTitle(ContentfulReference entry) =>
        !string.IsNullOrEmpty(entry.NavigationTitle)
            ? entry.NavigationTitle
            : !string.IsNullOrEmpty(entry.Title)
                ? entry.Title
                : entry.Name;

    private bool EntryIsValid(ContentfulReference entry) =>
        ContentfulHelpers.EntryIsNotALink(entry.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(entry.SunriseDate, entry.SunsetDate);

    internal static bool IsSubItemSuitableForContentType(string parentContentType, string subItemContentType) =>
        parentContentType switch
        {
            "CallToAction" => subItemContentType.Equals("callToActionBanner"),
            "EventCards" => subItemContentType.Equals("events"),
            "FindOutMoreBanner" or "FindOutMoreCards" or "ImageBannerContentWidth" or "ImageBannerScreenWidth" or
            "StatementBannerContentWidth" or "StatementBannerScreenWidth" => subItemContentType is "article" or "topic" or "directory" or "start-page" or "landingPage",
            "ProfileBanner" or "ProfileCards" => subItemContentType.Equals("profile"),
            "SocialMedia" => subItemContentType.Equals("socialMediaLink"),
            "TriviaBanner" or "TriviaCards" or "TriviaList" => subItemContentType.Equals("informationList") || subItemContentType.Equals("trivia"),
            "ImageContentWidth" or "ImageDividerScreenWidth" or "NewsBanner" or "SubscriptionBanner" or "Video" => false,
            _ => true
        };
}