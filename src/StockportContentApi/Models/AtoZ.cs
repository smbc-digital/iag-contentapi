namespace StockportContentApi.Models;

public class AtoZ(string title, string slug, string teaser, string type, List<string> alternativeTitles)
{
    public string Title { get; private set; } = title;
    public string Slug { get; } = slug;
    public string Teaser { get; } = teaser;
    public string Type { get; } = type;

    [JsonIgnore]
    public List<string> AlternativeTitles { get; } = alternativeTitles;

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

    public List<AtoZ> SetTitles()
    {
        List<AtoZ> matchingItems = new()
        {
            this
        };
        
        if (AlternativeTitles is not null)
        {
            foreach (string atozAlternativeTitle in AlternativeTitles)
            {
                matchingItems.Add(new AtoZ(atozAlternativeTitle, Slug, Teaser, Type, AlternativeTitles));
            }
        }

        return matchingItems;
    }
}