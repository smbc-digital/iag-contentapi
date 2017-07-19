using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class NewsRoomContentfulFactory : IContentfulFactory<ContentfulNewsRoom, Newsroom>
    {
       public Newsroom ToModel(ContentfulNewsRoom entry)
        {
           return new Newsroom(entry.Alerts, entry.EmailAlerts, entry.EmailAlertsTopicId);
        }
    }
}