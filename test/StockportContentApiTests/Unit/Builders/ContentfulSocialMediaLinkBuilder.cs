namespace StockportContentApiTests.Unit.Builders;

public class ContentfulSocialMediaLinkBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private string _link = "link";
    private string _icon = "icon";
    private string _accountName = "account name";
    private string _screenReader = "screen reader";

    public ContentfulSocialMediaLink Build()
        => new()
        {
            Title = _title,
            Slug = _slug,
            Link = _link,
            Icon = _icon,
            AccountName = _accountName,
            ScreenReader = _screenReader
        };

    public ContentfulSocialMediaLinkBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulSocialMediaLinkBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulSocialMediaLinkBuilder WithLink(string link)
    {
        _link = link;
        return this;
    }

    public ContentfulSocialMediaLinkBuilder WithIcon(string icon)
    {
        _icon = icon;
        return this;
    }

    public ContentfulSocialMediaLinkBuilder WithAccountName(string accountName)
    {
        _accountName = accountName;
        return this;
    }

    public ContentfulSocialMediaLinkBuilder WithScreenReader(string screenReader)
    {
        _screenReader = screenReader;
        return this;
    }
}