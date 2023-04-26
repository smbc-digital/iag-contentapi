namespace StockportContentApi.ContentfulFactories;

public interface IContentfulFactory<in T, out TO>
{
    TO ToModel(T entry);
}
