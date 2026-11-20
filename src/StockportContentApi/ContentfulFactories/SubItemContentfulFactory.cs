using Microsoft.AspNetCore.Http.Features;

namespace StockportContentApi.ContentfulFactories;

public interface ISubItemContentfulFactory : IContentfulFactory<ContentfulReference, SubItem>
{
    SubItem ToCacheModel(ContentfulReference entry);
    SubItem FilterForCurrentDate(SubItem entry);
}

public class SubItemContentfulFactory(ITimeProvider timeProvider) : ISubItemContentfulFactory
{
    private readonly DateComparer _dateComparer = new(timeProvider);

    public SubItem ToModel(ContentfulReference entry) =>
        ToModel(entry, includeInactiveItems: false);

    public SubItem ToCacheModel(ContentfulReference entry) =>
        ToModel(entry, includeInactiveItems: true);

    public SubItem FilterForCurrentDate(SubItem entry) =>
        new(entry.Slug,
            entry.Title,
            entry.Teaser,
            entry.TeaserImage,
            entry.Icon,
            entry.Type,
            entry.SunriseDate,
            entry.SunsetDate,
            entry.Image,
            entry.SubItems?
                .Where(item => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(item.SunriseDate, item.SunsetDate))
                .Select(FilterForCurrentDate)
                .ToList() ?? new(),
            entry.ColourScheme);

    private SubItem ToModel(ContentfulReference entry, bool includeInactiveItems)
    {
        string type = GetEntryType(entry);
        string image = GetEntryImage(entry);

        if (string.IsNullOrEmpty(image) && entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties))
            image = entry.BackgroundImage.File.Url;

        string title = GetEntryTitle(entry);
        List<SubItem> subItems = new();

        if (entry.SubItems is not null)
        {
            foreach (ContentfulReference item in entry.SubItems.Where(item => EntryIsValid(item, includeInactiveItems)))
            {
                SubItem newItem = new(item.Slug,
                                    GetEntryTitle(item),
                                    item.Teaser,
                                    GetTeaserImage(item),
                                    item.Icon,
                                    GetEntryType(item),
                                    item.SunriseDate,
                                    item.SunsetDate,
                                    GetEntryImage(item),
                                    new List<SubItem>(),
                                    item.ColourScheme);
                
                subItems.Add(newItem);
            }
        }

        if (entry.SecondaryItems is not null)
        {
            foreach (ContentfulReference item in entry.SecondaryItems.Where(item => EntryIsValid(item, includeInactiveItems)))
            {
                SubItem newItem = new(item.Slug,
                                    GetEntryTitle(item),
                                    item.Teaser,
                                    GetTeaserImage(item),
                                    item.Icon,
                                    GetEntryType(item),
                                    item.SunriseDate,
                                    item.SunsetDate,
                                    GetEntryImage(item),
                                    new List<SubItem>(),
                                    item.ColourScheme);

                subItems.Add(newItem);
            }
        }

        if (entry.TertiaryItems is not null)
        {
            foreach (ContentfulReference item in entry.TertiaryItems.Where(item => EntryIsValid(item, includeInactiveItems)))
            {
                SubItem newItem = new(item.Slug,
                                    GetEntryTitle(item),
                                    item.Teaser,
                                    GetTeaserImage(item),
                                    item.Icon,
                                    GetEntryType(item),
                                    item.SunriseDate,
                                    item.SunsetDate,
                                    GetEntryImage(item),
                                    new List<SubItem>(),
                                    item.ColourScheme);

                subItems.Add(newItem);
            }
        }

        if (entry.Sections is not null)
        {
            foreach (ContentfulSection section in entry.Sections.Where(section => EntryIsValid(section, includeInactiveItems)))
            {
                SubItem newSection = new($"{entry.Slug}/{section.Slug}",
                                        section.Title,
                                        section.Teaser,
                                        GetTeaserImage(section),
                                        section.Icon,
                                        GetEntryType(section),
                                        section.SunriseDate,
                                        section.SunsetDate,
                                        GetEntryImage(section),
                                        new List<SubItem>(),
                                        section.ColourScheme);

                subItems.Add(newSection);
            }
        }

        if (string.IsNullOrEmpty(entry.Icon))
            entry.Icon = type.Equals("payment") 
                ? "si-coin" 
                : "si-default";

        return new SubItem(entry.Slug,
                        title,
                        entry.Teaser,
                        GetTeaserImage(entry),
                        entry.Icon,
                        type,
                        entry.SunriseDate,
                        entry.SunsetDate,
                        image,
                        subItems,
                        entry.ColourScheme);
    }

    private static string GetEntryType(ContentfulReference entry) =>
        entry.Sys.ContentType.SystemProperties.Id.Equals("startPage")
            ? "start-page"
            : entry.Sys.ContentType.SystemProperties.Id;

    private static string GetEntryImage(ContentfulReference entry) =>
        entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
            ? entry.Image.File.Url
            : string.Empty;

    private static string GetTeaserImage(ContentfulReference entry) =>
        entry.TeaserImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.TeaserImage.SystemProperties)
            ? entry.TeaserImage.File.Url
            : string.Empty;
    
    private static string GetEntryTitle(ContentfulReference entry) =>
        !string.IsNullOrEmpty(entry.NavigationTitle)
            ? entry.NavigationTitle
            : !string.IsNullOrEmpty(entry.Title)
                ? entry.Title
                : entry.Name;

    private bool EntryIsValid(ContentfulReference entry, bool includeInactiveItems) =>
        ContentfulHelpers.EntryIsNotALink(entry.Sys)
        && (includeInactiveItems || _dateComparer.DateNowIsWithinSunriseAndSunsetDates(entry.SunriseDate, entry.SunsetDate));
}