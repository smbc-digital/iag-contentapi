﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class EventHomepageContentfulFactory : IContentfulFactory<ContentfulEventHomepage, EventHomepage>
    {
        public EventHomepage ToModel(ContentfulEventHomepage entry)
        {
            var tags = new List<string>();

            tags.Add(entry.Tag1);
            tags.Add(entry.Tag2);
            tags.Add(entry.Tag3);
            tags.Add(entry.Tag4);
            tags.Add(entry.Tag5);
            tags.Add(entry.Tag6);
            tags.Add(entry.Tag7);
            tags.Add(entry.Tag8);
            tags.Add(entry.Tag9);
            tags.Add(entry.Tag10);

            return new EventHomepage(tags);
        }
    }
}
