using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupContentfulFactory : IContentfulFactory<ContentfulGroup, Group>
    {
        private readonly IContentfulFactory<ContentfulGroupCategory, GroupCategory> _contentfulGroupCategoryFactory;

        public GroupContentfulFactory(IContentfulFactory<ContentfulGroupCategory, GroupCategory> contentfulGroupCategoryFactory)
        {
            _contentfulGroupCategoryFactory = contentfulGroupCategoryFactory;
        }

        public Group ToModel(ContentfulGroup entry)
        {
            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;

            var categoriesReferences = entry.CategoriesReference != null
                ? entry.CategoriesReference.Select(
                    catogory => _contentfulGroupCategoryFactory.ToModel(catogory.Fields)).ToList()
                : new List<GroupCategory>();

            return new Group(entry.Name, entry.Slug, entry.PhoneNumber, entry.Email, entry.Website,
                entry.Twitter, entry.Facebook, entry.Address, entry.Description, imageUrl, ImageConverter.ConvertToThumbnail(imageUrl), categoriesReferences, entry.Breadcrumbs);  
        }
    }
}