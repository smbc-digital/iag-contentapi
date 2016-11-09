namespace StockportContentApi.Factories
{
    public interface IFactory<out T>
    {
        T Build(dynamic entry, IContentfulIncludes contentfulResponse);
    }
}