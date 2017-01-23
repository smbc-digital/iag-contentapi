using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulProfileBuilder
    {
        private string _title = "title";
        private string _slug = "slug";
        private string _icon = "icon";
        private string _subtitle = "subtitle";
        private string _teaser = "teaser";
        private string _body = "body";
        private string _type = "type";
        private Asset _image = new Asset { File = new File { Url = "image-url.jpg" } };
        private Asset _backgroundImage = new Asset { File = new File { Url = "background-image-url.jpg" } };
        private List<Entry<ContentfulCrumb>> _breadcrumbs = new List<Entry<ContentfulCrumb>>
        {
            new Entry<ContentfulCrumb>() {Fields = new ContentfulCrumbBuilder().Build(),
                SystemProperties = new SystemProperties() { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }}
        };

        public ContentfulProfile Build()
        {
            return new ContentfulProfile
            {
                Type = _type,
                Title = _title,
                Slug = _slug,
                Subtitle = _subtitle,
                Teaser = _teaser,
                Image = _image,
                Body = _body,
                Icon = _icon,
                BackgroundImage = _backgroundImage,
                Breadcrumbs = _breadcrumbs
            };
        }
    }
}
