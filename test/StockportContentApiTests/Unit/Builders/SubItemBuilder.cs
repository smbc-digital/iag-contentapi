using System;
using System.Collections.Generic;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class SubItemBuilder
    {
        private string _slug = "slug";
        private string _title = "title";
        private string _teaser = "teaser";
        private string _icon = "icon";
        private string _type = "type";
        private DateTime _sunriseDate = DateTime.MinValue;
        private DateTime _sunsetDate = DateTime.MaxValue;
        private string _image = "image";
        private List<SubItem> _subItems = new List<SubItem>();

        public SubItem Build()
        {
            return new SubItem(_slug, _title, _teaser, _icon, _type, _sunriseDate, _sunsetDate, _image, _subItems);
        }     
    }
}
