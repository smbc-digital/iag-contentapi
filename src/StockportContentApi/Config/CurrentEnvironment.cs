namespace StockportContentApi.Config;

[ExcludeFromCodeCoverage]
public class CurrentEnvironment(string name)
{
    public string Name { get; } = name;
}
