using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulGroupBuilder
    {
       private string _name = "name";
        private string _slug = "slug";
        private string _phoneNumber = "phoneNumber";
        private string _email = "email";
        private string _website = "website";
        private string _twitter = "twitter";
        private string _facebook = "facebook";
        private string _address = "address";
        private string _description = "description";

        public ContentfulGroup Build()
        {
            return new ContentfulGroup
            {
                Address = _address,
                Description = _description,
                Email = _email,
                Facebook = _facebook,
                Name = _name,
                PhoneNumber = _phoneNumber,
                Slug = _slug,
                Twitter = _twitter,
                Website = _website
            };
        }

        public ContentfulGroupBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }
    }
}
