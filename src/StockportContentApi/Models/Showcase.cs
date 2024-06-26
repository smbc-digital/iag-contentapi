namespace StockportContentApi.Model;

public class Showcase
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string HeroImageUrl { get; set; }
    public string Teaser { get; set; }
    public string MetaDescription { get; set; }
    public string Subheading { get; set; }
    public string FeaturedItemsSubheading { get; set; }
    public string SocialMediaLinksSubheading { get; set; }
    public string EventSubheading { get; set; }
    public string EventCategory { get; set; }
    public string EventsCategoryOrTag { get; set; }
    public string EventsReadMoreText { get; set; }
    public string NewsSubheading { get; set; }
    public string NewsCategory { get; set; }
    public string NewsCategoryTag { get; set; }
    public string BodySubheading { get; set; }
    public string Body { get; set; }
    public string ProfileHeading { get; set; }
    public string ProfileLink { get; set; }
    public string EmailAlertsTopicId { get; set; }
    public string EmailAlertsText { get; set; }
    public string Icon { get; set; }
    public string TriviaSubheading { get; set; }
    public string TypeformUrl { get; set; }
    public IEnumerable<SubItem> SubItems { get; set; }
    public IDictionary<string, dynamic> Content { get; set; }

}

public class Data
{
    public ContentfulReference Target { get; set; }
}

public class ContentItem
{
    public Data Data { get; set; }
}