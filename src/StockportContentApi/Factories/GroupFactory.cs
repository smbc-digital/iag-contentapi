using System.Collections.Generic;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class GroupFactory : IFactory<Group>
    {
        public Group Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            var fields = entry.fields;
            if (fields == null)
                return null;

            string type = fields.type;
            string name = fields.name;
            string slug = fields.slug;
            string phoneNumber = fields.phoneNumber;
            string email = fields.email;
            string website = fields.website;
            string twitter = fields.twitter;
            string facebook = fields.facebook;
            string address = fields.address;
            string description = fields.description;

            return new Group(type, name, slug, phoneNumber, email, website, twitter, facebook, address, description);
        }
    }
}