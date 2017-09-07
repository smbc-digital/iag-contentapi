using System.Collections.Generic;
using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class OrganisationContentfulFactory : IContentfulFactory<ContentfulOrganisation, Organisation>
    {
        public OrganisationContentfulFactory()
        {
        }

        public Organisation ToModel(ContentfulOrganisation entry)
        {
            var imageUrl = entry.Image != null 
                ? ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) 
                    ? entry.Image.File.Url 
                    : string.Empty
                : string.Empty;

            return new Organisation(entry.Title, entry.Slug, imageUrl, entry.Phone, entry.Email, entry.AboutUs,
                entry.Volunteering, entry.VolunteeringText);  
        }
    }
}