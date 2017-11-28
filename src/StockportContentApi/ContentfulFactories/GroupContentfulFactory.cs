using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupContentfulFactory : IContentfulFactory<ContentfulGroup, Group>
    {
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly IContentfulFactory<ContentfulGroupCategory, GroupCategory> _contentfulGroupCategoryFactory;
        private readonly IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> _contentfulGroupSubCategoryFactory;
        private readonly IContentfulFactory<ContentfulOrganisation, Organisation> _contentfulOrganisationFactory;
        private readonly DateComparer _dateComparer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupContentfulFactory(IContentfulFactory<ContentfulOrganisation, Organisation> contentfulOrganisationFactory, IContentfulFactory<ContentfulGroupCategory, GroupCategory> contentfulGroupCategoryFactory, IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory> contentfulGroupSubCategoryFactory, ITimeProvider timeProvider, IHttpContextAccessor httpContextAccessor, IContentfulFactory<Asset, Document> documentFactory)
        {
            _contentfulOrganisationFactory = contentfulOrganisationFactory;
            _contentfulGroupCategoryFactory = contentfulGroupCategoryFactory;
            _contentfulGroupSubCategoryFactory = contentfulGroupSubCategoryFactory;
            _dateComparer = new DateComparer(timeProvider);
            _httpContextAccessor = httpContextAccessor;
            _documentFactory = documentFactory;
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

            var groupDocuments = entry.AdditionalDocuments.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties)).Select(document => _documentFactory.ToModel(document)).ToList();

            var organisation = entry.Organisation != null ?  _contentfulOrganisationFactory.ToModel(entry.Organisation) : new Organisation();

            var status = "Published";
            if (!_dateComparer.DateNowIsNotBetweenHiddenRange(entry.DateHiddenFrom, entry.DateHiddenTo))
            {
                status = "Archived";
            }

            var administrators = entry.GroupAdministrators;
            administrators.Items = administrators.Items.Select(i => i.StripData(_httpContextAccessor)).ToList();

            var cost = entry.Cost != null && entry.Cost.Any() ? entry.Cost : new List<string>();

            return new Group(entry.Name, entry.Slug, entry.PhoneNumber, entry.Email, entry.Website,
                entry.Twitter, entry.Facebook, entry.Address, entry.Description, imageUrl, ImageConverter.ConvertToThumbnail(imageUrl), 
                categoriesReferences, subCategories, new List <Crumb> { new Crumb("Stockport Local", string.Empty, "groups") }, entry.MapPosition, entry.Volunteering,
                administrators, entry.DateHiddenFrom, entry.DateHiddenTo, status, cost, entry.CostText, entry.AbilityLevel, entry.VolunteeringText, 
                organisation, entry.Donations, entry.AccessibleTransportLink, entry.AdditionalInformation, groupDocuments, entry.Sys.UpdatedAt, entry.SuitableFor, entry.AgeRange).StripData(_httpContextAccessor);
        }
    }
}