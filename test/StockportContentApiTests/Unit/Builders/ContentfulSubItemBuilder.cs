using System;
using System.Collections.Generic;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulSubItemBuilder
    {
        private string _slug = "slug";
        private string _title = "title";
        private string _name = "name";
        private string _teaser = "teaser";
        private string _icon = "icon";
        private DateTime _sunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
        private Asset _image = new Asset { File = new File { Url = "image" }, SystemProperties = new SystemProperties { Type = "Asset" } };
        private List<ContentfulSubItem> _subItems = new List<ContentfulSubItem>();
        private List<ContentfulSubItem> _secondaryItems = new List<ContentfulSubItem>();
        private List<ContentfulSubItem> _tertiaryItems = new List<ContentfulSubItem>();
        private string _systemId = "id";
        private string _contentTypeSystemId = "id";

        public ContentfulSubItem Build()
        {
            return new ContentfulSubItem
            {
                Slug = _slug,
                Title = _title,
                Name = _name,
                Teaser = _teaser,
                Icon = _icon,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate,
                Image = _image,
                SubItems = _subItems,
                SecondaryItems = _secondaryItems,
                TertiaryItems = _tertiaryItems,
                Sys = new SystemProperties
                {
                    ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                    Id = _systemId
                }
            };
        }

        public ContentfulSubItemBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulSubItemBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulSubItemBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public ContentfulSubItemBuilder SubItems(List<ContentfulSubItem> subItems)
        {
            _subItems = subItems;
            return this;
        }

        public ContentfulSubItemBuilder SecondaryItems(List<ContentfulSubItem> secondaryItems)
        {
            _secondaryItems = secondaryItems;
            return this;
        }

        public ContentfulSubItemBuilder TertiaryItems(List<ContentfulSubItem> tertiaryItems)
        {
            _tertiaryItems = tertiaryItems;
            return this;
        }

        public ContentfulSubItemBuilder SystemId(string id)
        {
            _systemId = id;
            return this;
        }

        public ContentfulSubItemBuilder SystemContentTypeId(string id)
        {
            _contentTypeSystemId = id;
            return this;
        }
    }
}
