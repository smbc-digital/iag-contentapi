using System;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulInsetTextBuilder
    {
        private string _title = "title";
        private string _body = "body";
        private string _colour = "colour";
        private string _subHeading = "subHeading";
        private string _slug = "slug";
        private SystemProperties _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
        };

        public ContentfulInsetText Build()
        {
            return new ContentfulInsetText
            {
                Title = _title,
                Body = _body,
                Colour = _colour,
                SubHeading = _subHeading,
                Sys = _sys,
                Slug = _slug
            };
        }

        public ContentfulInsetTextBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulInsetTextBuilder Body(string body)
        {
            _body = body;
            return this;
        }

        public ContentfulInsetTextBuilder Colour(string colour)
        {
            _colour = colour;
            return this;
        }

        public ContentfulInsetTextBuilder SeverSubHeadingity(string subHeading)
        {
            _subHeading = subHeading;
            return this;
        }

    }
}
