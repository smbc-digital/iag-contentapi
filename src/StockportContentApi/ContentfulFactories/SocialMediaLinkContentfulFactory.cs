namespace StockportContentApi.ContentfulFactories;

public class SocialMediaLinkContentfulFactory : IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>
{
    public SocialMediaLink ToModel(ContentfulSocialMediaLink link)
        => new(link.Title, link.Slug, link.Link, link.Icon, link.AccountName, link.ScreenReader);
}