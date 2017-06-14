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
        private string _contentSystemId = "id";
        private List<ContentfulSubItem> _subItems = new List<ContentfulSubItem> {
            new ContentfulSubItemBuilder().Slug("sub-slug").Build()};
        private List<ContentfulSubItem> _secondaryItems = new List<ContentfulSubItem> {
            new ContentfulSubItemBuilder().Slug("secondary-slug").Build()};
        private List<ContentfulSubItem> _tertiaryItems = new List<ContentfulSubItem> {
            new ContentfulSubItemBuilder().Slug("tertiary-slug").Build()};

        

        public ContentfulCrumb Build()
        {
            return new ContentfulCrumb
            {
                Title = _title,
                Name = _name,
                Slug = _slug,
                SubItems = _subItems,
                SecondaryItems = _secondaryItems,
                TertiaryItems = _tertiaryItems,
                Sys = new SystemProperties
                {
                    ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentSystemId } }
                }
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

        public ContentfulCrumbBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulCrumbBuilder SubItems(List<ContentfulSubItem> subItems)
        {
            _subItems = subItems;
            return this;
        }

        public ContentfulCrumbBuilder ContentTypeSystemId(string contentTypeSystemId)
        {
            _contentSystemId = contentTypeSystemId;
            return this;
        }
    }
}
