using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulGroupHomepageBuilder
    {
        private string _title = "title";
        private string _slug = "slug";       
        private Asset _backgroundImage = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
        private SystemProperties _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
        };

        public ContentfulGroupHomepage Build()
        {
            return new ContentfulGroupHomepage
            {
                Slug = _slug,
                Title = _title,
                BackgroundImage = _backgroundImage
            };
        }

        public ContentfulGroupHomepageBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulGroupHomepageBuilder Title(string title)
        {
            _title = title;
            return this;
        }
       
    }
}
