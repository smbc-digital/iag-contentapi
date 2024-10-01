namespace StockportContentApi.ContentfulFactories;

public class SubItemContentfulFactory : IContentfulFactory<ContentfulReference, SubItem>
{
    private readonly DateComparer _dateComparer;

    public SubItemContentfulFactory(ITimeProvider timeProvider) => _dateComparer = new DateComparer(timeProvider);

    public SubItem ToModel(ContentfulReference entry)
    {
        string type = GetEntryType(entry);
        string image = GetEntryImage(entry);
        if (string.IsNullOrEmpty(image) && entry.BackgroundImage?.SystemProperties is not null &&
            ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties))
        {
            image = entry.BackgroundImage.File.Url;
        }

        string title = GetEntryTitle(entry);
        List<SubItem> subItems = new();

        if (entry.SubItems is not null)
        {
            foreach (ContentfulReference item in entry.SubItems.Where(EntryIsValid))
            {
                SubItem newItem = new(item.Slug, GetEntryTitle(item), item.Teaser, item.Icon, GetEntryType(item), item.SunriseDate, item.SunsetDate, GetEntryImage(item), new List<SubItem>(), item.ColourScheme);
                subItems.Add(newItem);
            }
        }

        if (entry.SecondaryItems is not null)
        {
            foreach (ContentfulReference item in entry.SecondaryItems.Where(EntryIsValid))
            {
                SubItem newItem = new(item.Slug, GetEntryTitle(item), item.Teaser, item.Icon, GetEntryType(item), item.SunriseDate, item.SunsetDate, GetEntryImage(item), new List<SubItem>(), item.ColourScheme);

                subItems.Add(newItem);
            }
        }

        if (entry.TertiaryItems is not null)
        {
            foreach (ContentfulReference item in entry.TertiaryItems.Where(EntryIsValid))
            {
                SubItem newItem = new(item.Slug, GetEntryTitle(item), item.Teaser, item.Icon, GetEntryType(item), item.SunriseDate, item.SunsetDate, GetEntryImage(item), new List<SubItem>(), item.ColourScheme);

                subItems.Add(newItem);
            }
        }

        if (entry.Sections is not null)
        {
            foreach (ContentfulSection section in entry.Sections.Where(EntryIsValid))
            {
                SubItem newSection = new($"{entry.Slug}/{section.Slug}", section.Title, section.Teaser, section.Icon, GetEntryType(section), section.SunriseDate, section.SunsetDate, GetEntryImage(section), new List<SubItem>(), section.ColourScheme);
                
                subItems.Add(newSection);
            }
        }

        if (string.IsNullOrEmpty(entry.Icon))
            entry.Icon = type.Equals("payment") ? "si-coin" : "si-default";

        string handledSlug = HandleSlugForGroupsHomepage(entry.Sys, entry.Slug);

        return new SubItem(handledSlug, title, entry.Teaser, entry.Icon, type, entry.SunriseDate, entry.SunsetDate, image, subItems, entry.ColourScheme);
    }

    private static string HandleSlugForGroupsHomepage(SystemProperties sys, string entrySlug) =>
        sys.ContentType.SystemProperties.Id.Equals("groupHomepage") ? "groups" : entrySlug;

    private static string GetEntryType(ContentfulReference entry) =>
        entry.Sys.ContentType.SystemProperties.Id.Equals("startPage") ? "start-page" : entry.Sys.ContentType.SystemProperties.Id;

    private static string GetEntryImage(ContentfulReference entry) =>
        entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;

    private static string GetEntryTitle(ContentfulReference entry) =>
        !string.IsNullOrEmpty(entry.NavigationTitle)
            ? entry.NavigationTitle
            : !string.IsNullOrEmpty(entry.Title)
                ? entry.Title
                : entry.Name;

    private bool EntryIsValid(ContentfulReference entry) =>
        ContentfulHelpers.EntryIsNotALink(entry.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(entry.SunriseDate, entry.SunsetDate);
}