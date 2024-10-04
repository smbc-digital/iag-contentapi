namespace StockportContentApi.ContentfulFactories.GroupFactories;

public class GroupHomepageContentfulFactory : IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>
{
    private readonly DateComparer _dateComparer;
    private IContentfulFactory<ContentfulGroup, Group> _groupFactory;
    private IContentfulFactory<ContentfulGroupCategory, GroupCategory> _groupCategoryListFactory;
    private IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> _groupSubCategoryListFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory;

    public GroupHomepageContentfulFactory(IContentfulFactory<ContentfulGroup, Group> groupFactory, IContentfulFactory<ContentfulGroupCategory, GroupCategory> groupCategoryListFactory, IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> groupSubCategoryListFactory, ITimeProvider timeProvider, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory)
    {
        _groupFactory = groupFactory;
        _groupCategoryListFactory = groupCategoryListFactory;
        _groupSubCategoryListFactory = groupSubCategoryListFactory;
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
        _eventBannerFactory = eventBannerFactory;
    }

    public GroupHomepage ToModel(ContentfulGroupHomepage entry)
    {
        string backgroundImage = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
            ? entry.BackgroundImage.File.Url 
            : string.Empty;

        IEnumerable<Group> groups = entry.FeaturedGroups.Select(g => _groupFactory.ToModel(g));
        
        GroupCategory groupCategory = _groupCategoryListFactory.ToModel(entry.FeaturedGroupsCategory);
        
        GroupSubCategory groupSubCategory = _groupSubCategoryListFactory.ToModel(entry.FeaturedGroupsSubCategory);
        
        List<Group> featuredGroup = groups.Where(group => _dateComparer.DateNowIsNotBetweenHiddenRange(group.DateHiddenFrom, group.DateHiddenTo)).ToList();
        
        IEnumerable<Alert> alerts = entry.Alerts.Select(_ => _alertFactory.ToModel(_));

        string bodyHeading = entry.BodyHeading;
        string body = entry.Body;
        string secondaryBodyHeading = entry.SecondaryBodyHeading;
        string secondaryBody = entry.SecondaryBody;

        EventBanner eventBanner = ContentfulHelpers.EntryIsNotALink(entry.EventBanner.Sys)
            ? _eventBannerFactory.ToModel(entry.EventBanner)
            : new NullEventBanner();

        return new GroupHomepage(entry.Title, entry.Slug, entry.MetaDescription, backgroundImage, entry.FeaturedGroupsHeading, featuredGroup, groupCategory, groupSubCategory, alerts, bodyHeading, body, secondaryBodyHeading, secondaryBody, eventBanner);
    }
}
