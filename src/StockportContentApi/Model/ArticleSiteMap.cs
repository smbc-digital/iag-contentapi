using System;
using System.Collections.Generic;
using System.Linq;

namespace StockportContentApi.Model
{
    public class ArticleSiteMap
    {
        public string Slug { get; }
       

        public ArticleSiteMap(string slug)
        {
            Slug = slug;
        }       
    }
}