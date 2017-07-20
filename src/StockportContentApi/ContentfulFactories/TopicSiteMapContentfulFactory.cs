using System;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class TopicSiteMapContentfulFactory : IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>
    {
        public TopicSiteMap ToModel(ContentfulTopicForSiteMap entry)
        {
            return new TopicSiteMap(entry.Slug, entry.SunriseDate, entry.SunsetDate);
        }
    }
}