namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class ContactUsId(string name,
                        string slug,
                        string emailAddress)
{
    public string Name { get; } = name;
    public string Slug { get; } = slug;
    public string EmailAddress { get; } = emailAddress;
}