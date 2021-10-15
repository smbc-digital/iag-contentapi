using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class SocialMediaLinkContentfulFactory : IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>
    {
        public SocialMediaLink ToModel(ContentfulSocialMediaLink link)
        {
            return new SocialMediaLink(link.Title, link.Slug, link.Url, link.Icon, link.AccountName);
        }
    }
}