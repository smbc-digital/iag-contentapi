using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class BasicLinkListContentfulFactory : IContentfulFactory<IEnumerable<ContentfulBasicLink>, IEnumerable<BasicLink>>
    {

        public IEnumerable<BasicLink> ToModel(IEnumerable<ContentfulBasicLink> entry)
        {
            return entry.Select(_ => new BasicLink(_.Url, _.Text));
        }
    }
}
