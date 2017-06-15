using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ProfileContentfulFactory : IContentfulFactory<ContentfulProfile, Profile>
    {
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;

        public ProfileContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory)
        {
            _crumbFactory = crumbFactory;
        }

        public Profile ToModel(ContentfulProfile entry)
        {       
            var breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;
            var backgroundImageUrl = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties) 
                ? entry.BackgroundImage.File.Url : string.Empty;

            return new Profile(entry.Type, entry.Title, entry.Slug, entry.Subtitle, entry.Teaser, imageUrl, 
                               entry.Body, entry.Icon, backgroundImageUrl, breadcrumbs);
        }
    }
}