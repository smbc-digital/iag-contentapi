using System.Collections.Generic;

namespace StockportContentApi.Factories
{
    public interface IBuildContentTypesFromReferences<out T>
    {
        IEnumerable<T> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse);
    }


    public interface IBuildContentTypeFromReference<out T>
    {
        T BuildFromReference(dynamic reference, IContentfulIncludes contentfulResponse);
    }
}