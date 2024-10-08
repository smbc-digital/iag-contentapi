namespace StockportContentApi.ContentfulFactories.GroupFactories;

public class GroupContentfulFactory : IContentfulFactory<ContentfulGroup, Group>
{
    private readonly IContentfulFactory<Asset, Document> _documentFactory;
    private readonly IContentfulFactory<ContentfulGroupCategory, GroupCategory> _contentfulGroupCategoryFactory;
    private readonly IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> _contentfulGroupSubCategoryFactory;
    private readonly IContentfulFactory<ContentfulOrganisation, Organisation> _contentfulOrganisationFactory;
    private readonly IContentfulFactory<ContentfulGroupBranding, GroupBranding> _contentfulGroupBrandingFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;

    public GroupContentfulFactory(IContentfulFactory<ContentfulOrganisation,
        Organisation> contentfulOrganisationFactory,
        IContentfulFactory<ContentfulGroupCategory,
            GroupCategory> contentfulGroupCategoryFactory,
        IContentfulFactory<ContentfulGroupSubCategory,
            GroupSubCategory> contentfulGroupSubCategoryFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<Asset, Document> documentFactory,
        IContentfulFactory<ContentfulGroupBranding,
        GroupBranding> groupBrandingFactory,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory)
    {
        _contentfulOrganisationFactory = contentfulOrganisationFactory;
        _contentfulGroupCategoryFactory = contentfulGroupCategoryFactory;
        _contentfulGroupSubCategoryFactory = contentfulGroupSubCategoryFactory;
        _dateComparer = new DateComparer(timeProvider);
        _documentFactory = documentFactory;
        _contentfulGroupBrandingFactory = groupBrandingFactory;
        _alertFactory = alertFactory;
    }

    public Group ToModel(ContentfulGroup entry)
    {
        string imageUrl = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) 
            ? entry.Image.File.Url 
            : string.Empty;

        List<GroupCategory> categoriesReferences = entry.CategoriesReference is not null
            ? entry.CategoriesReference.Where(catRef => catRef is not null)
                .Select(catogory => _contentfulGroupCategoryFactory.ToModel(catogory)).ToList()
            : new List<GroupCategory>();

        List<GroupSubCategory> subCategories = entry.SubCategories is not null
            ? entry.SubCategories.Where(o => o is not null)
                .Select(category => _contentfulGroupSubCategoryFactory.ToModel(category)).ToList()
            : new List<GroupSubCategory>();

        List<Document> groupDocuments = entry.AdditionalDocuments.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                            .Select(document => _documentFactory.ToModel(document)).ToList();

        Organisation organisation = entry.Organisation is not null
            ? _contentfulOrganisationFactory.ToModel(entry.Organisation)
            : new Organisation();

        string status = "Published";

        if (!_dateComparer.DateNowIsNotBetweenHiddenRange(entry.DateHiddenFrom, entry.DateHiddenTo))
            status = "Archived";

        GroupAdministrators administrators = entry.GroupAdministrators;

        List<string> cost = entry.Cost is not null && entry.Cost.Any()
            ? entry.Cost
            : new List<string>();

        List<GroupBranding> groupBranding = entry.GroupBranding is not null
            ? entry.GroupBranding.Where(o => o is not null)
                .Select(branding => _contentfulGroupBrandingFactory.ToModel(branding)).ToList()
            : new List<GroupBranding>();

        IEnumerable<Alert> alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                        .Where(alert => !alert.Severity.Equals("Condolence"))
                                        .Select(alert => _alertFactory.ToModel(alert));

        IEnumerable<Alert> alertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                            .Where(alert => !alert.Severity.Equals("Condolence"))
                                            .Select(alert => _alertFactory.ToModel(alert));

        return new Group(entry.Name, entry.Slug, entry.MetaDescription, entry.PhoneNumber, entry.Email, entry.Website,
            entry.Twitter, entry.Facebook, entry.Address, entry.Description, imageUrl, ImageConverter.ConvertToThumbnail(imageUrl),
            categoriesReferences, subCategories, new List<Crumb> { new("Stockport Local", string.Empty, "groups") }, entry.MapPosition, entry.Volunteering,
            administrators, entry.DateHiddenFrom, entry.DateHiddenTo, status, cost, entry.CostText, entry.AbilityLevel, entry.VolunteeringText,
            organisation, entry.Donations, entry.AccessibleTransportLink, groupBranding, entry.Tags, entry.AdditionalInformation, groupDocuments, entry.Sys.UpdatedAt, entry.SuitableFor, entry.AgeRange, entry.DonationsText, entry.DonationsUrl, alerts, alertsInline);
    }
}