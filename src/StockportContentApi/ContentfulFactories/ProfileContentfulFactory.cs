using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class ProfileContentfulFactory : IContentfulFactory<ContentfulProfile, Profile>
    {
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;

        public ProfileContentfulFactory(IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory)
        {
            _crumbFactory = crumbFactory;
        }

        public Profile ToModel(ContentfulProfile entry)
        {       
            var breadcrumbs = entry.Breadcrumbs.Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            return new Profile(entry.Type, entry.Title, entry.Slug, entry.Subtitle, entry.Teaser, 
                                entry.Image.File.Url, entry.Body, entry.Icon, entry.BackgroundImage.File.Url, 
                                breadcrumbs);
        }
    }
}