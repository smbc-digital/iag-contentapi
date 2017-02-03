using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupContentfulFactory : IContentfulFactory<ContentfulGroup, Group>
    {
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;

        public GroupContentfulFactory(IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory)
        {
            _crumbFactory = crumbFactory;
        }

        public Group ToModel(ContentfulGroup entry)
        {
            return new Group(entry.Name, entry.Slug, entry.PhoneNumber, entry.Email, entry.Website,
                entry.Twitter, entry.Facebook, entry.Address, entry.Description);  
        }
    }
}