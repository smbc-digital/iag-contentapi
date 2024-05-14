namespace StockportContentApi.Model;

public class Group
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string MetaDescription { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string Twitter { get; set; }
    public string Facebook { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string ThumbnailImageUrl { get; set; }
    public List<GroupCategory> CategoriesReference { get; set; }
    public List<GroupSubCategory> SubCategories { get; set; }
    public List<Crumb> Breadcrumbs { get; set; }
    public MapPosition MapPosition { get; set; }
    public bool Volunteering { get; set; }
    public List<Event> Events { get; private set; }
    public GroupAdministrators GroupAdministrators { get; set; }
    public DateTime? DateHiddenFrom { get; set; }
    public DateTime? DateHiddenTo { get; set; }
    public string Status { get; set; }
    public List<string> Cost { get; set; }
    public string CostText { get; set; }
    public string AbilityLevel { get; set; }
    public string VolunteeringText { get; set; }
    public Organisation Organisation { get; set; }
    public List<Group> LinkedGroups { get; private set; }
    public bool Donations { get; set; }
    public string DonationsText { get; set; }
    public string DonationsUrl { get; set; }
    public string AccessibleTransportLink { get; set; }
    public List<GroupBranding> GroupBranding { get; set; }
    public List<string> Tags { get; }

    public string AdditionalInformation { get; set; }
    public List<Document> AdditionalDocuments { get; set; }
    public DateTime? DateLastModified { get; set; }
    public List<string> SuitableFor { get; set; }
    public List<string> AgeRange { get; set; }
    public IEnumerable<Alert> Alerts { get; }
    public IEnumerable<Alert> AlertsInline { get; }
    public Group() { }

    public Group(string name, string slug, string metaDescription, string phoneNumber, string email, string website,
        string twitter, string facebook, string address, string description, string imageUrl,
        string thumbnailImageUrl, List<GroupCategory> categoriesReference, List<GroupSubCategory> subCategories, List<Crumb> breadcrumbs,
        MapPosition mapPosition, bool volunteering, GroupAdministrators groupAdministrators,
        DateTime? dateHiddenFrom, DateTime? dateHiddenTo, string status, List<string> cost, string costText, string abilityLevel, string volunteeringText,
        Organisation organisation, bool donations, string accessibleTransportLink, List<GroupBranding> groupBranding, List<string> tags, string additionalInformation, List<Document> additionalDocuments,
        DateTime? dateLastModified, List<string> suitableFor, List<string> ageRange, string donationsText, string donationsUrl, IEnumerable<Alert> alerts, IEnumerable<Alert> alertsInline)
    {
        Name = name;
        Slug = slug;
        MetaDescription = metaDescription;
        PhoneNumber = phoneNumber;
        Email = email;
        Website = website;
        Twitter = twitter;
        Facebook = facebook;
        Address = address;
        Description = description;
        ImageUrl = imageUrl;
        ThumbnailImageUrl = thumbnailImageUrl;
        CategoriesReference = categoriesReference;
        SubCategories = subCategories;
        Breadcrumbs = breadcrumbs;
        MapPosition = mapPosition;
        Volunteering = volunteering;
        Donations = donations;
        GroupAdministrators = groupAdministrators;
        DateHiddenFrom = dateHiddenFrom;
        DateHiddenTo = dateHiddenTo;
        Status = status;
        Cost = cost;
        CostText = costText;
        AbilityLevel = abilityLevel;
        VolunteeringText = volunteeringText;
        Organisation = organisation;
        AccessibleTransportLink = accessibleTransportLink;
        GroupBranding = groupBranding;
        Tags = tags.Select(s => s.ToLower()).ToList();
        AdditionalInformation = additionalInformation;
        AdditionalDocuments = additionalDocuments;
        DateLastModified = dateLastModified;
        SuitableFor = suitableFor;
        AgeRange = ageRange;
        DonationsText = donationsText;
        DonationsUrl = donationsUrl;
        Alerts = alerts;
        AlertsInline = alertsInline;
    }

    public void SetEvents(List<Event> events)
    {
        Events = events;
    }

    public void SetLinkedGroups(List<Group> linkedGroups)
    {
        LinkedGroups = linkedGroups;
    }
}


