namespace StockportContentApi.Model;

public class GroupCategory
{
    public string Name { get; }
    public string Slug { get; }
    public string Icon { get; }
    public string ImageUrl { get; }

    public GroupCategory(string name, string slug, string icon, string imageUrl)
    {
        Name = name;
        Slug = slug;
        Icon = icon;
        ImageUrl = imageUrl;
    }
}


