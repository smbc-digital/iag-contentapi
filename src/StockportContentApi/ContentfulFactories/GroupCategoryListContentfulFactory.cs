using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupCategoryListContentfulFactory : IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>>
    {
        private readonly IContentfulFactory<ContentfulGroupCategory, GroupCategory> groupCategoryFactory;

        public GroupCategoryListContentfulFactory(IContentfulFactory<ContentfulGroupCategory, GroupCategory> _groupCategoryFactory)
        {
            groupCategoryFactory = _groupCategoryFactory;
        }

        public List<GroupCategory> ToModel(List<ContentfulGroupCategory> entries)
        {
            var groupCategoryList = new List<GroupCategory>();
            foreach (var groupCategory in entries)
            {
                var groupCategoryItem = groupCategoryFactory.ToModel(groupCategory);
                groupCategoryList.Add(groupCategoryItem);
            }
            return groupCategoryList;
        }
    }
}