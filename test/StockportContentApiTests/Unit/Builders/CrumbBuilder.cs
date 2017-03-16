using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class CrumbBuilder
    {
        private string _slug = "slug";
        private string _title = "title";
        private string _name = "name";      

        public Crumb Build()
        {
            return new Crumb(_title, _slug, _name);
        }     
    }
}
