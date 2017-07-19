using System.Collections.Generic;
using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupContentfulFactory : IContentfulFactory<ContentfulGroup, Group>
    {
        private readonly IContentfulFactory<ContentfulGroupCategory, GroupCategory> _contentfulGroupCategoryFactory;
        private readonly DateComparer _dateComparer;

        public GroupContentfulFactory(IContentfulFactory<ContentfulGroupCategory, GroupCategory> contentfulGroupCategoryFactory, ITimeProvider timeProvider)
        {
            _contentfulGroupCategoryFactory = contentfulGroupCategoryFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public Group ToModel(ContentfulGroup entry)
        {
            var imageUrl = entry.Image != null 
                ? ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) 
                    ? entry.Image.File.Url 
                    : string.Empty
                : string.Empty;

            var categoriesReferences = entry.CategoriesReference != null
                ? entry.CategoriesReference.Where(o => o != null).Select(catogory => _contentfulGroupCategoryFactory.ToModel(catogory)).ToList()
                : new List<GroupCategory>();

            var status = "Published";
            if (!_dateComparer.DateNowIsNotBetweenHiddenRange(entry.DateHiddenFrom, entry.DateHiddenTo))
            {
                status = "Archived";
            }

            var cost = entry.Cost != null && entry.Cost.Any() ? entry.Cost[0] : string.Empty;

            return new Group(entry.Name, entry.Slug, entry.PhoneNumber, entry.Email, entry.Website,
                entry.Twitter, entry.Facebook, entry.Address, entry.Description, imageUrl, ImageConverter.ConvertToThumbnail(imageUrl), 
                categoriesReferences, new List<Crumb> { new Crumb("Find a local group", string.Empty, "groups") }, entry.MapPosition, entry.Volunteering, 
                entry.GroupAdministrators, entry.DateHiddenFrom, entry.DateHiddenTo, status, cost, entry.CostText, entry.AbilityLevel);  
        }
    }
}