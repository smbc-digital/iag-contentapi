namespace StockportContentApi.ContentfulFactories;

public class OrganisationContentfulFactory : IContentfulFactory<ContentfulOrganisation, Organisation>
{
    public Organisation ToModel(ContentfulOrganisation entry)
    {
        string imageUrl = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
            ? entry.Image.File.Url
            : string.Empty;

        return new Organisation(entry.Title, entry.Slug, imageUrl, entry.AboutUs, entry.Phone, entry.Email,
            entry.Volunteering, entry.VolunteeringText, entry.Donations, entry.DonationsText, entry.DonationsUrl);
    }
}