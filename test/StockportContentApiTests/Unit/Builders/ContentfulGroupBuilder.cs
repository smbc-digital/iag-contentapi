using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulGroupBuilder
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
        private Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
        private List<Entry<ContentfulGroupCategory>> _categoriesReference = new List<Entry<ContentfulGroupCategory>>();
        private MapPosition _mapPosition = new MapPosition() {Lat=39,Lon= 2};
        private GroupAdministrators _groupAdministrators = new GroupAdministrators();

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
                Website = _website,
                Image = _image,
                CategoriesReference = _categoriesReference,
                GroupAdministrators = _groupAdministrators
            };
        }

        public ContentfulGroupBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulGroupBuilder MapPosition(MapPosition mapPosition)
        {
            _mapPosition = mapPosition;
            return this;
        }

        public ContentfulGroupBuilder CategoriesReference(List<Entry<ContentfulGroupCategory>> categoriesReference)
        {
            _categoriesReference = categoriesReference;
            return this;
        }

        public ContentfulGroupBuilder GroupAdministrators(GroupAdministrators groupAdministrators)
        {
            _groupAdministrators = groupAdministrators;
            return this;
        }
    }
}
