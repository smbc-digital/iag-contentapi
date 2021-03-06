using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class OrganisationContentfulFactory : IContentfulFactory<ContentfulOrganisation, Organisation>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrganisationContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Organisation ToModel(ContentfulOrganisation entry)
        {
            var imageUrl = entry.Image != null 
                ? ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) 
                    ? entry.Image.File.Url 
                    : string.Empty
                : string.Empty;

            return new Organisation(entry.Title, entry.Slug, imageUrl, entry.AboutUs, entry.Phone, entry.Email,
                entry.Volunteering, entry.VolunteeringText, entry.Donations,entry.DonationsText,entry.DonationsUrl).StripData(_httpContextAccessor);  
        }
    }
}