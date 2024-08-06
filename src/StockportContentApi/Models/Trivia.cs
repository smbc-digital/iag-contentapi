namespace StockportContentApi.Model;

[ExcludeFromCodeCoverage]
public class Trivia
{
    public string Name { get; }

    public string Icon { get; }

    public string Text { get; }

    public string Link { get; }

    public Trivia(string name, string icon, string text, string link)
    {
        Name = name;
        Icon = icon;
        Text = text;
        Link = link;
    }
}