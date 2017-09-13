using System.Collections.Generic;
using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupHomepageContentfulFactory : IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>
    {
        private readonly DateComparer _dateComparer;
        private IContentfulFactory<List<ContentfulGroup>, List<Group>> _groupListFactory;
        private IContentfulFactory<ContentfulGroupCategory, GroupCategory> _groupCategoryListFactory;
        private IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> _groupSubCategoryListFactory;

        public GroupHomepageContentfulFactory(IContentfulFactory<List<ContentfulGroup>, List<Group>> groupListFactory, IContentfulFactory<ContentfulGroupCategory, GroupCategory> groupCategoryListFactory, IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> groupSubCategoryListFactory, ITimeProvider timeProvider)
        {
            _groupListFactory = groupListFactory;
            _groupCategoryListFactory = groupCategoryListFactory;
            _groupSubCategoryListFactory = groupSubCategoryListFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public GroupHomepage ToModel(ContentfulGroupHomepage entry)
        {
            var backgroundImage = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                                           ? entry.BackgroundImage.File.Url : string.Empty;


            var groups = _groupListFactory.ToModel(entry.FeaturedGroups);

            var groupCategory = _groupCategoryListFactory.ToModel(entry.FeaturedGroupsCategory);

            var groupSubCategory = _groupSubCategoryListFactory.ToModel(entry.FeaturedGroupsSubCategory);

            var featuredGroup = groups.Where(group => _dateComparer.DateNowIsNotBetweenHiddenRange(
                group.DateHiddenFrom, group.DateHiddenTo));

            return new GroupHomepage(entry.Title, entry.Slug, backgroundImage, entry.FeaturedGroupsHeading, groups, groupCategory, groupSubCategory);
        }
    }
}
