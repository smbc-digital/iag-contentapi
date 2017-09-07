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
        private readonly IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> _contentfulGroupSubCategoryFactory;
        private readonly IContentfulFactory<ContentfulOrganisation, Organisation> _contentfulOrganisationFactory;
        private readonly DateComparer _dateComparer;

        public GroupContentfulFactory(IContentfulFactory<ContentfulOrganisation, Organisation> contentfulOrganisationFactory, IContentfulFactory<ContentfulGroupCategory, GroupCategory> contentfulGroupCategoryFactory, IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> contentfulGroupSubCategoryFactory, ITimeProvider timeProvider)
        {
            _contentfulOrganisationFactory = contentfulOrganisationFactory;
            _contentfulGroupCategoryFactory = contentfulGroupCategoryFactory;
            _contentfulGroupSubCategoryFactory = contentfulGroupSubCategoryFactory;
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

            var subCategories = entry.SubCategories != null
                ? entry.SubCategories.Where(o => o != null).Select(category => _contentfulGroupSubCategoryFactory.ToModel(category)).ToList()
                : new List<GroupSubCategory>();

            var organisation = _contentfulOrganisationFactory.ToModel(entry.Organisation);

            var status = "Published";
            if (!_dateComparer.DateNowIsNotBetweenHiddenRange(entry.DateHiddenFrom, entry.DateHiddenTo))
            {
                status = "Archived";
            }

            var cost = entry.Cost != null && entry.Cost.Any() ? entry.Cost[0] : string.Empty;

            return new Group(entry.Name, entry.Slug, entry.PhoneNumber, entry.Email, entry.Website,
                entry.Twitter, entry.Facebook, entry.Address, entry.Description, imageUrl, ImageConverter.ConvertToThumbnail(imageUrl), 
                categoriesReferences, subCategories, new List <Crumb> { new Crumb("Our Stockport Local", string.Empty, "groups") }, entry.MapPosition, entry.Volunteering, 
                entry.GroupAdministrators, entry.DateHiddenFrom, entry.DateHiddenTo, status, cost, entry.CostText, entry.AbilityLevel, entry.VolunteeringText, organisation);  
        }
    }
}