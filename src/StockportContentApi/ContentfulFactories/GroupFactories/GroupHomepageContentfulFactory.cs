using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories.GroupFactories
{
    public class GroupHomepageContentfulFactory : IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>
    {
        private readonly DateComparer _dateComparer;
        private IContentfulFactory<ContentfulGroup, Group> _groupFactory;
        private IContentfulFactory<ContentfulGroupCategory, GroupCategory> _groupCategoryListFactory;
        private IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> _groupSubCategoryListFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;

        public GroupHomepageContentfulFactory(IContentfulFactory<ContentfulGroup, Group> groupFactory, IContentfulFactory<ContentfulGroupCategory, GroupCategory> groupCategoryListFactory, IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> groupSubCategoryListFactory, ITimeProvider timeProvider, IHttpContextAccessor httpContextAccessor, IContentfulFactory<ContentfulAlert, Alert> alertFactory)
        {
            _groupFactory = groupFactory;
            _groupCategoryListFactory = groupCategoryListFactory;
            _groupSubCategoryListFactory = groupSubCategoryListFactory;
            _dateComparer = new DateComparer(timeProvider);
            _httpContextAccessor = httpContextAccessor;
            _alertFactory = alertFactory;
        }

        public GroupHomepage ToModel(ContentfulGroupHomepage entry)
        {
            var backgroundImage = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                                           ? entry.BackgroundImage.File.Url : string.Empty;


            var groups = entry.FeaturedGroups.Select(g => _groupFactory.ToModel(g));

            var groupCategory = _groupCategoryListFactory.ToModel(entry.FeaturedGroupsCategory);

            var groupSubCategory = _groupSubCategoryListFactory.ToModel(entry.FeaturedGroupsSubCategory);

            var featuredGroup = groups.Where(group => _dateComparer.DateNowIsNotBetweenHiddenRange(
                group.DateHiddenFrom, group.DateHiddenTo)).ToList();

            var alerts = entry.Alerts.Select(_ => _alertFactory.ToModel(_));

            var body = entry.Body;

            var secondaryBody = entry.SecondaryBody;

            return new GroupHomepage(entry.Title, entry.Slug, backgroundImage, entry.FeaturedGroupsHeading, featuredGroup, groupCategory, groupSubCategory, alerts, body, secondaryBody).StripData(_httpContextAccessor);
        }
    }
}
