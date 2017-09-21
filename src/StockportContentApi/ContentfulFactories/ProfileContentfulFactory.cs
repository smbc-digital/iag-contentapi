using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class ProfileContentfulFactory : IContentfulFactory<ContentfulProfile, Profile>
    {
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory, IHttpContextAccessor httpContextAccessor)
        {
            _crumbFactory = crumbFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public Profile ToModel(ContentfulProfile entry)
        {       
            var breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;
            var backgroundImageUrl = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties) 
                ? entry.BackgroundImage.File.Url : string.Empty;

            return new Profile(entry.Type, entry.Title, entry.Slug, entry.Subtitle, entry.Teaser, imageUrl, 
                               entry.Body, entry.Icon, backgroundImageUrl, breadcrumbs).StripData(_httpContextAccessor);
        }
    }
}