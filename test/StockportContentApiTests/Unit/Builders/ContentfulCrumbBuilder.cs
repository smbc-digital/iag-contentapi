using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulCrumbBuilder
    {
        private string _slug = "slug";
        private string _title = "title";
        private string _name = "name";
        private List<Entry<ContentfulSubItem>> _subItems = new List<Entry<ContentfulSubItem>> {
            new ContentfulEntryBuilder<ContentfulSubItem>().Fields(new ContentfulSubItemBuilder().Slug("sub-slug").Build()).Build() };
        private List<Entry<ContentfulSubItem>> _secondaryItems = new List<Entry<ContentfulSubItem>> {
            new ContentfulEntryBuilder<ContentfulSubItem>().Fields(new ContentfulSubItemBuilder().Slug("secondary-slug").Build()).Build() };
        private List<Entry<ContentfulSubItem>> _tertiaryItems = new List<Entry<ContentfulSubItem>> {
            new ContentfulEntryBuilder<ContentfulSubItem>().Fields(new ContentfulSubItemBuilder().Slug("tertiary-slug").Build()).Build() };

        public ContentfulCrumb Build()
        {
            return new ContentfulCrumb
            {
                Title = _title,
                Name = _name,
                Slug = _slug,
                SubItems = _subItems,
                SecondaryItems = _secondaryItems,
                TertiaryItems = _tertiaryItems
            };
        }

        public ContentfulCrumbBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public ContentfulCrumbBuilder Title(string title)
        {
            _title = title;
            return this;
        }
    }
}
