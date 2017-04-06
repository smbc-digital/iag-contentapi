using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupListContentfulFactory : IContentfulFactory<List<ContentfulGroup>, List<Group>>
    {
        private readonly IContentfulFactory<ContentfulGroup, Group> groupFactory;

        public GroupListContentfulFactory(IContentfulFactory<ContentfulGroup, Group> _groupFactory)
        {
            groupFactory = _groupFactory;
        }

        public List<Group> ToModel(List<ContentfulGroup> entries)
        {
            var groupList = new List<Group>();
            foreach (var group in entries)
            {
                var groupItem = groupFactory.ToModel(group);
                groupList.Add(groupItem);
            }
            return groupList;
        }
    }
}