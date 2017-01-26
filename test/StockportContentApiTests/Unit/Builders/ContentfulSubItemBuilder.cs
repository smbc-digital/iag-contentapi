using System;
using StockportContentApi.ContentfulModels;

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
                SunsetDate = _sunsetDate
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
    }
}
