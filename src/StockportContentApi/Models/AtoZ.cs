namespace StockportContentApi.Models;

public class AtoZ
{
    public string Title { get; private set; }
    public string Slug { get; }
    public string Teaser { get; }
    public string Type { get; }

    [JsonIgnore]
    public List<string> AlternativeTitles { get; }

    public AtoZ(string title, string slug, string teaser, string type, List<string> alternativeTitles)
    {
        AlternativeTitles = alternativeTitles;
        Title = title;
        Slug = slug;
        Teaser = teaser;
        Type = type;
    }

    public List<AtoZ> SetTitleStartingWithLetter(string letter)
    {
        List<AtoZ> matchingItems = new();
        string letterToLower = letter.ToLower();

        if (Title.ToLower().StartsWith(letterToLower)) matchingItems.Add(this);

        if (AlternativeTitles is not null)
        {
            foreach (string atozAlternativeTitle in AlternativeTitles)
            {
                if (atozAlternativeTitle.ToLower().StartsWith(letterToLower))
                    matchingItems.Add(new AtoZ(atozAlternativeTitle, Slug, Teaser, Type, AlternativeTitles));
            }
        }

        return matchingItems;
    }
}