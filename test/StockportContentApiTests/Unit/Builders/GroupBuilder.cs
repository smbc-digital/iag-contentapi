using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class GroupBuilder
    {
        private string _name = "_name";
        private string _slug = "_slug";
        private string _phoneNumber = "_phoneNumber";
        private string _email = "_email";
        private string _website = "_website";
        private string _twitter = "_twitter";
        private string _facebook = "_facebook";
        private string _address = "_address";
        private string _description = "_description";
        private string _image = "image-url.jpg";
        private string _thumbnail = "thumbnail.jpg";
        private List<GroupCategory> _categoriesReference = new List<GroupCategory>();

        public Group Build()
        {
            return new Group(_name, _slug, _phoneNumber, _email, _website, _twitter, _facebook, _address, _description,
                _image, _thumbnail, _categoriesReference);
        }

        public GroupBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public GroupBuilder CategoriesReference(List<GroupCategory> categoriesReference)
        {
            _categoriesReference = categoriesReference;
            return this;
        }
    }
}
