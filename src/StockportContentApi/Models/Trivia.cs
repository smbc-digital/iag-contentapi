namespace StockportContentApi.Model;

[ExcludeFromCodeCoverage]
public class Trivia
{
    public string Name { get; }

    public string Icon { get; }

    public string Body { get; }

    public string Link { get; }

    public Trivia(string name, string icon, string body, string link)
    {
        Name = name;
        Icon = icon;
        Body = body;
        Link = link;
    }
}