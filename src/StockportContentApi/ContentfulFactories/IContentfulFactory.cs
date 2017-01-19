namespace StockportContentApi.ContentfulFactories
{
    public interface IContentfulFactory<I, O>
    {
        O ToModel(I entry);
    }
}
