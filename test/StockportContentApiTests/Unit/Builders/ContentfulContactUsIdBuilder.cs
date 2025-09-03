namespace StockportContentApiTests.Unit.Builders;

public class ContentfulContactUsIdBuilder
{
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };

    public ContentfulContactUsId Build()
        => new()
        {
            Name = "name",
            Slug = "slug",
            EmailAddress = "email@example.com",
            Sys = _sys
        };
}