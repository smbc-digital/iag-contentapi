using System;
using System.Collections.Generic;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulExpandingLinkBoxBuilder
    {      
        private string _title = "title";               
        private List<Entry<ContentfulSubItem>> _links = new List<Entry<ContentfulSubItem>> {
            new ContentfulEntryBuilder<ContentfulSubItem>().Fields(new ContentfulSubItemBuilder().Slug("sub-slug").Build()).Build() };

        public ContentfulExpandingLinkBox Build()
        {
            return new ContentfulExpandingLinkBox
            {             
                Title = _title,
                Links = _links
            };
        }

        public ContentfulExpandingLinkBoxBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulExpandingLinkBoxBuilder Links(List<Entry<ContentfulSubItem>> expandingLinkBoxs)
        {
            _links = expandingLinkBoxs;
            return this;
        }
    }
}
