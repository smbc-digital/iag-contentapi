using System.Collections.Generic;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupSubCategoryListContentfulFactory : IContentfulFactory<IEnumerable<ContentfulGroupSubCategory>, List<GroupSubCategory>>
    {
        private readonly IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> groupSubCategoryFactory;

        public GroupSubCategoryListContentfulFactory(IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> _groupSubCategoryFactory)
        {
            groupSubCategoryFactory = _groupSubCategoryFactory;
        }

        public List<GroupSubCategory> ToModel(IEnumerable<ContentfulGroupSubCategory> entries)
        {
            var groupSubCategoryList = new List<GroupSubCategory>();
            foreach (var groupSubCategory in entries)
            {
                var groupSubCategoryItem = groupSubCategoryFactory.ToModel(groupSubCategory);
                groupSubCategoryList.Add(groupSubCategoryItem);
            }

            return groupSubCategoryList;
        }
    }
}