using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulParentTopicBuilder
    {
        private string _name = "name";       
        private readonly List<IContentfulSubItem> _subItems = new List<IContentfulSubItem> {
            new ContentfulSubItemBuilder().Slug("sub-slug").Build() };
        private readonly List<IContentfulSubItem> _secondaryItems = new List<IContentfulSubItem> {
            new ContentfulSubItemBuilder().Slug("secondary-slug").Build() };
        private readonly List<IContentfulSubItem> _tertiaryItems = new List<IContentfulSubItem> {
            new ContentfulSubItemBuilder().Slug("tertiary-slug").Build()};
        private string _systemId = "id";
        private string _contentTypeSystemId = "id";

        public ContentfulTopic Build()
        {
            return new ContentfulTopic
            {
                Name = _name,               
                SubItems = _subItems,
                SecondaryItems = _secondaryItems,
                TertiaryItems  = _tertiaryItems,
                Sys = new SystemProperties
                {
                    ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                    Id = _systemId
                }
            };
        }

        public ContentfulParentTopicBuilder SystemId(string id)
        {
            _systemId = id;
            return this;
        }

        public ContentfulParentTopicBuilder SystemContentTypeId(string id)
        {
            _contentTypeSystemId = id;
            return this;
        }
    }
}
