namespace StockportContentApi.ContentfulFactories;

public class SubItemContentfulFactory : IContentfulFactory<ContentfulReference, SubItem>
{
    private readonly DateComparer _dateComparer;

    public SubItemContentfulFactory(ITimeProvider timeProvider)
    {
        _dateComparer = new DateComparer(timeProvider);
    }

    public SubItem ToModel(ContentfulReference entry)
    {
        var type = GetEntryType(entry);
        var image = GetEntryImage(entry);
        if (string.IsNullOrEmpty(image) &&
            entry.BackgroundImage?.SystemProperties is not null &&
            ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties))
        {
            image = entry.BackgroundImage.File.Url;
        }

        var title = GetEntryTitle(entry);

        // build all of the sub items (only avaliable for topics)
        List<SubItem> subItems = new();
        if (entry.SubItems is not null)
        {
            foreach (ContentfulReference item in entry.SubItems.Where(EntryIsValid))
            {
                SubItem newItem = new(item.Slug, GetEntryTitle(item), item.Teaser, item.Icon, GetEntryType(item), entry.ContentType, item.SunriseDate, item.SunsetDate, GetEntryImage(item), item.MailingListId, item.Body, new List<SubItem>(), item.Link, item.ColourScheme);
                subItems.Add(newItem);
            }
        }

        if (entry.SecondaryItems != null)
        {
            foreach (ContentfulReference item in entry.SecondaryItems.Where(EntryIsValid))
            {
                SubItem newItem = new(item.Slug, GetEntryTitle(item), item.Teaser, item.Icon, GetEntryType(item), entry.ContentType, item.SunriseDate, item.SunsetDate, GetEntryImage(item), item.MailingListId, item.Body, new List<SubItem>(), item.Link, item.ColourScheme);
                subItems.Add(newItem);
            }
        }

        if (entry.TertiaryItems != null)
        {
            foreach (ContentfulReference item in entry.TertiaryItems.Where(EntryIsValid))
            {
                SubItem newItem = new(item.Slug, GetEntryTitle(item), item.Teaser, item.Icon, GetEntryType(item), entry.ContentType, item.SunriseDate, item.SunsetDate, GetEntryImage(item), item.MailingListId, item.Body, new List<SubItem>(), item.Link, item.ColourScheme);
                subItems.Add(newItem);
            }
        }

        if (entry.Sections != null)
        {
            foreach (ContentfulSection section in entry.Sections.Where(EntryIsValid))
            {
                SubItem newSection = new SubItem($"{entry.Slug}/{section.Slug}", section.Title, section.Teaser, section.Icon, GetEntryType(section), entry.ContentType, section.SunriseDate, section.SunsetDate, GetEntryImage(section), entry.MailingListId, entry.Body, new List<SubItem>(), section.Link, section.ColourScheme);
                subItems.Add(newSection);
            }
        }

        if (string.IsNullOrEmpty(entry.Icon))
        {
            entry.Icon = type == "payment" ? "si-coin" : "si-default";
        }

        var handledSlug = HandleSlugForGroupsHomepage(entry.Sys, entry.Slug);

        return new SubItem(handledSlug, title, entry.Teaser, entry.Icon, type, entry.ContentType, entry.SunriseDate, entry.SunsetDate, image, entry.MailingListId, entry.Body, subItems, entry.Link, entry.ColourScheme);
    }

    private static string HandleSlugForGroupsHomepage(SystemProperties sys, string entrySlug)
    {
        return sys.ContentType.SystemProperties.Id == "groupHomepage" ? "groups" : entrySlug;
    }

    private string GetEntryType(ContentfulReference entry)
    {
        return entry.Sys.ContentType.SystemProperties.Id == "startPage" ? "start-page" : entry.Sys.ContentType.SystemProperties.Id;
    }

    private string GetEntryImage(ContentfulReference entry)
    {
        return entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ?
            entry.Image.File.Url : string.Empty;
    }

    private static string GetEntryTitle(ContentfulReference entry) => 
        !string.IsNullOrEmpty(entry.NavigationTitle) 
            ? entry.NavigationTitle 
            : !string.IsNullOrEmpty(entry.Title) 
                ? entry.Title 
                : entry.Name;

    private bool EntryIsValid(ContentfulReference entry)
    {
        return ContentfulHelpers.EntryIsNotALink(entry.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(entry.SunriseDate, entry.SunsetDate);
    }
}