using System;
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
        private List<string> _cost = new List<string>() {"lots"};
        private string _costText = "";
        private string _abilityLevel = "";
        private string _accessibleTransportLink = "link";
        private string _additionalInformation = "info";
        private Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
        private List<ContentfulGroupCategory> _categoriesReference = new List<ContentfulGroupCategory>
        {
            new ContentfulGroupCategory
            {
                Name = "name",
                Slug = "slug",
                Image = new Asset(),
                Icon = "icon",
                Sys = new SystemProperties()
            }
        };
        private MapPosition _mapPosition = new MapPosition() { Lat = 39, Lon = 2 };
        private SystemProperties _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id"} }
        };
        private GroupAdministrators _groupAdministrators = new GroupAdministrators();
        private DateTime _dateHiddenFrom = DateTime.MinValue;
        private DateTime _dateHiddenTo = DateTime.MinValue;
        private ContentfulOrganisation _organisation = new ContentfulOrganisation();
        private List<string> _suitableFor = new List<string>();
        private List<string> _ageRanges = new List<string>(){"15-20"};
        private bool _volunteering = true;
        private string _volunteeringText = "text";
        private List<Asset> _additionalDocuments = new List<Asset>
        {
            new Asset
            {
                Title = "Document",
                SystemProperties = new SystemProperties
                {
                    Type = "NotALink"
                }
            }
        };

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
                MapPosition = _mapPosition,
                Sys = _sys,
                GroupAdministrators = _groupAdministrators,
                DateHiddenFrom = _dateHiddenFrom,
                DateHiddenTo = _dateHiddenTo,
                Cost = _cost,
                CostText = _costText,
                AbilityLevel = _abilityLevel,
                Organisation = _organisation,
                AgeRange = _ageRanges,
                SuitableFor = _suitableFor,
                AccessibleTransportLink = _accessibleTransportLink,
                AdditionalInformation = _additionalInformation,
                Volunteering = _volunteering,
                VolunteeringText = _volunteeringText,
                AdditionalDocuments = _additionalDocuments
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

        public ContentfulGroupBuilder CategoriesReference(List<ContentfulGroupCategory> categoriesReference)
        {
            _categoriesReference = categoriesReference;
            return this;
        }

        public ContentfulGroupBuilder GroupAdministrators(GroupAdministrators groupAdministrators)
        {
            _groupAdministrators = groupAdministrators;
            return this;
        }

        public ContentfulGroupBuilder DateHiddenFrom(DateTime dateHiddenFrom)
        {
            _dateHiddenFrom = dateHiddenFrom;
            return this;
        }

        public ContentfulGroupBuilder DateHiddenTo(DateTime dateHiddenTo)
        {
            _dateHiddenTo = dateHiddenTo;
            return this;
        }

        public ContentfulGroupBuilder Cost(List<string> cost)
        {
            _cost = cost;
            return this;
        }

        public ContentfulGroupBuilder CostText(string costText)
        {
            _costText = costText;
            return this;
        }

        public ContentfulGroupBuilder AbilityLevel(string abilityLevel)
        {
            _abilityLevel = abilityLevel;
            return this;
        }
    }
}
